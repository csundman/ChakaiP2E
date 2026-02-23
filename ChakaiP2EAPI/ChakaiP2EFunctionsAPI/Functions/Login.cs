using System.Collections.Generic;
using System;
using System.Net;
using System.Threading.Tasks;
using Azure;
using ChakaiP2EFunctionsAPI.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using ChakaiP2EFunctionsAPI.Utilities;
using System.Linq.Expressions;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace ChakaiP2EFunctionsAPI.Functions
{
    public class Login : BaseController
    {
        public Login(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            InitializeFunction<Login>(loggerFactory);
        }

        [Function("Login")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "login/{forgot?}")] HttpRequestData req, ILogger log, string forgot)
        {
            Logger.LogInformation("C# HTTP trigger function processed a request.");

            InitResponse(req);

            //First get the account creation data
            var accountData = await req.ReadFromJsonAsync<Account>();

            if(forgot == "forgot")
            {
                await SendForgotPasswordEmail(accountData.Username);
                return Response;
            }

            try
            {
                //Start the SQL connection
                using (SqlConnection conn = new SqlConnection(SecurityUtil.ConnectionString))
                {
                    await conn.OpenAsync();

                    // Start a local transaction
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        Dictionary<string, object> results;
                        //Create the new account
                        using (SqlCommand command = conn.CreateCommand())
                        {
                            command.Transaction = transaction;

                            command.CommandText = @"
SELECT password_hash, password_salt
FROM accounts 
WHERE username = @username";
                            command.Parameters.AddWithValue("username", accountData.Username);

                            results = GetDbResultSet(command).FirstOrDefault();
                        }

                        if(results == null)
                        {
                            await WriteErrorToResponse(HttpStatusCode.Unauthorized, "Incorrect username or password.");
                            return Response;
                        }

                        //Hash the password with the salt
                        var passwordHash = BCrypt.Net.BCrypt.HashPassword(accountData.Password, results["password_salt"] as string);

                        if (passwordHash != results["password_hash"] as string)
                        {
                            await WriteErrorToResponse(HttpStatusCode.Unauthorized, "Incorrect username or password.");
                            return Response;
                        }

                        // Generate a unique token
                        string token = Guid.NewGuid().ToString();

                        using (SqlCommand command = conn.CreateCommand())
                        {
                            command.Transaction = transaction;

                            command.CommandText = @"
UPDATE accounts
SET api_token = @token, 
token_expiration_datetime = DATEADD(HOUR, 5, GETDATE())
WHERE username = @username";
                            command.Parameters.AddWithValue("token", token);
                            command.Parameters.AddWithValue("username", accountData.Username);

                            command.ExecuteNonQuery();

                        }

                        //If we got here, we created the account
                        Response.StatusCode = HttpStatusCode.OK;
                        await Response.WriteAsJsonAsync(new Dictionary<string, string>
                        {
                            { "token", token }
                        });
                        

                        // If we reached this point, it means all operations succeeded
                        // Commit the transaction
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // If an error occurs, roll back the transaction
                        transaction.Rollback();
                        await WriteErrorToResponse(HttpStatusCode.InternalServerError, $"A Sql Error Occured: {ex.Message}");
                        return Response;
                    }
                }
            }
            catch (Exception ex)
            {
                await WriteErrorToResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return Response;
        }

        public async Task<bool> SendForgotPasswordEmail(string username)
        {
            try
            {
                //Start the SQL connection
                using (SqlConnection conn = new SqlConnection(SecurityUtil.ConnectionString))
                {
                    await conn.OpenAsync();

                    // Start a local transaction
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        Dictionary<string, object> results;
                        //Create the new account
                        using (SqlCommand command = conn.CreateCommand())
                        {
                            command.Transaction = transaction;

                            command.CommandText = @"
SELECT email
FROM accounts 
WHERE username = @username";
                            command.Parameters.AddWithValue("username", username);

                            results = GetDbResultSet(command).FirstOrDefault();
                        }

                        if (results == null)
                        {
                            await WriteErrorToResponse(HttpStatusCode.Unauthorized, "Incorrect username or password.");
                            transaction.Rollback();
                            return false;
                        }

                        if (results["email"] == null || results["email"] == DBNull.Value)
                        {
                            await WriteErrorToResponse(HttpStatusCode.Unauthorized, "Incorrect username or password.");
                            transaction.Rollback();
                            return false;
                        }

                        var password = GenerateTempPassword(8);

                        var subject = "Chakai P2E password has been reset";
                        var body = $"Your password to ChakaiP2E has been reset. You can access the site with this temporary password: { password }. Please login and change your password.";

                        SendEmail(results["email"].ToString(), subject, body);

                        var account = new Account { Password = password };

                        using (SqlCommand command = conn.CreateCommand())
                        {
                            command.Transaction = transaction;

                            command.CommandText = @"
UPDATE accounts 
SET password_hash = @hash, password_salt = @salt
WHERE username = @username";
                            command.Parameters.AddWithValue("username", username);
                            command.Parameters.AddWithValue("hash", account.PasswordHash);
                            command.Parameters.AddWithValue("salt", account.PasswordSalt);

                            int affected = await command.ExecuteNonQueryAsync();

                            if (affected == 0)
                                throw new Exception("No rows were updated.");
                        }

                        // If we reached this point, it means all operations succeeded
                        // Commit the transaction
                        transaction.Commit();

                        //If we got here, we created the account
                        Response.StatusCode = HttpStatusCode.OK;
                        await Response.WriteAsJsonAsync(new Dictionary<string, string>
                        {
                            { "message", "Password has been reset." }
                        });

                        return true;
                    }
                    catch (Exception ex)
                    {
                        // If an error occurs, roll back the transaction
                        transaction.Rollback();
                        await WriteErrorToResponse(HttpStatusCode.InternalServerError, $"A Sql Error Occured: {ex.Message}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                await WriteErrorToResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return false;
        }

        public void SendEmail(string toEmail, string subject, string body)
        {
            var fromAddress = new MailAddress("chakaip2e@gmail.com", "Chakai P2E");
            var toAddress = new MailAddress(toEmail);
            const string fromPassword = "hlbx ffby ocsz neom";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com", // Change this for your SMTP server
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            };

            smtp.Send(message);
        }

        private const string AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

        public static string GenerateTempPassword(int length = 12)
        {
            if (length <= 0) throw new ArgumentException("Password length must be greater than 0.");

            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[length];
                rng.GetBytes(bytes);

                var chars = bytes.Select(b => AllowedChars[b % AllowedChars.Length]);
                return new string(chars.ToArray());
            }
        }
    }
}
