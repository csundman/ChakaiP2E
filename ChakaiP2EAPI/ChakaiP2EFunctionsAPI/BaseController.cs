using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ChakaiP2EFunctionsAPI.Utilities;
using System.Net;
using System.Threading.Tasks;
using ChakaiP2EFunctionsAPI.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Identity.Client;

namespace ChakaiP2EFunctionsAPI
{
    public class BaseController
    {
        protected ILogger Logger { get; private set; }
        internal HttpResponseData Response { get; set; }
        internal int? AccountID { get; private set; }

        public BaseController(ILoggerFactory loggerFactory)
        {
        }

        public async Task HandleRequest(HttpRequestData req, string id = null, List<string> skipAuth = null)
        {
            Logger.LogInformation("C# HTTP trigger function processed a request.");

            InitResponse(req);

            int? intId = null;

            if (!id.IsNullOrEmpty())
            {
                if (int.TryParse(id, out int result))
                {
                    intId = result;
                }
                else
                {
                    await WriteErrorToResponse(HttpStatusCode.BadRequest, "Id must be a valid integer.");
                    return;
                }
            }
            else if (req.Method == "PUT" || req.Method == "DELETE")
            {
                await WriteErrorToResponse(HttpStatusCode.BadRequest, "Id is required for " + req.Method + " requests.");
                return;
            }

            try
            {
                if (!skipAuth?.Any(s => s.ToLower() == req.Method.ToLower()) ?? true)
                {
                    await Authorize(req);
                    if (!new[] { (int)HttpStatusCode.OK, (int)HttpStatusCode.NoContent }.Contains((int)Response.StatusCode))
                    {
                        return;
                    }
                }

                switch (req.Method)
                {
                    case "GET":
                        {
                            if (intId == null)
                                await HandleMultiGet();
                            else
                                await HandleGet(intId.Value); break;
                        }
                    case "POST": await HandlePost(req); break;
                    case "PUT": await HandlePut(req, intId.Value); break;
                    case "DELETE": await HandleDelete(intId.Value); break;
                }
            }
            catch (Exception ex)
            {
                await WriteErrorToResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        protected virtual async Task HandleDelete(int id)
        {
            throw new NotImplementedException();
        }

        protected virtual async Task HandlePut(HttpRequestData req, int id)
        {
            throw new NotImplementedException();
        }

        protected virtual async Task HandlePost(HttpRequestData req)
        {
            throw new NotImplementedException();
        }

        protected virtual async Task HandleGet(int id)
        {
            throw new NotImplementedException();
        }

        protected virtual async Task HandleMultiGet()
        {
            throw new NotImplementedException();
        }

        public async Task Authorize(HttpRequestData req)
        {
            Logger.LogInformation("Authorizing");
            try
            {
                if (!req.Headers.TryGetValues("Authorization", out var authHeaders))
                {
                    await WriteErrorToResponse(HttpStatusCode.Unauthorized, "No Authorization Header");
                    return;
                }

                var authHeader = authHeaders.FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    await WriteErrorToResponse(HttpStatusCode.Unauthorized, "Bearer token required.");
                    return;
                }

                var token = authHeader.Substring("Bearer ".Length);

                Logger.LogInformation("Connecting to DB: " + SecurityUtil.ConnectionString);

                //Start the SQL connection
                using (SqlConnection conn = new SqlConnection(SecurityUtil.ConnectionString))
                {
                    Logger.LogInformation("Opening Connection");

                    await conn.OpenAsync();

                    Logger.LogInformation("Connected");

                    // Start a local transaction
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        List<Dictionary<string, object>> results;
                        //Authorize
                        using (SqlCommand command = conn.CreateCommand())
                        {
                            command.Transaction = transaction;

                            command.CommandText = @"
SELECT account_id
FROM accounts 
WHERE api_token = @api_token
AND GETDATE() < accounts.token_expiration_datetime";
                            command.Parameters.AddWithValue("api_token", token);

                            Logger.LogInformation("Authorizing with token: " + token);

                            results = GetDbResultSet(command);

                        }

                        if (results == null || results.Count == 0 || results.First().Count == 0)
                        {
                            Logger.LogError("Could not obtain AccountID from query." + results);
                            await WriteErrorToResponse(HttpStatusCode.Unauthorized, "Invalid or expired authorization token.");
                            return;
                        }

                        Logger.LogInformation("Authorization success.");

                        AccountID = results.First()["account_id"] as int?;

                        // If we reached this point, it means all operations succeeded
                        // Commit the transaction
                        transaction.Commit();
                        return;
                    }
                    catch (Exception ex)
                    {
                        // If an error occurs, roll back the transaction
                        transaction.Rollback();
                        await WriteErrorToResponse(HttpStatusCode.InternalServerError, $"A Sql Error Occured: {ex.Message}");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                await WriteErrorToResponse(HttpStatusCode.InternalServerError, ex.Message);
                return;
            }
        }

        public void InitResponse(HttpRequestData requestData)
        {
            // Initialize the response
            Logger.LogInformation("Init Request.");

            Response = requestData.CreateResponse(HttpStatusCode.NoContent);
            Response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            return;
        }

        public void InitializeFunction<T>(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<T>();
            Logger.LogInformation("Fetching Connection String from Environment");
            SecurityUtil.ConnectionString = Environment.GetEnvironmentVariable(SecurityUtil.ConnectionStringEnv);
            SqlHelper.Logger = Logger;
        }

        public async Task WriteErrorToResponse(HttpStatusCode statusCode, string errorMessage)
        {
            Logger.LogError(errorMessage);
            await Response.WriteAsJsonAsync(new ErrorJSONObject(errorMessage), statusCode);
        }

        public List<Dictionary<string, object>> GetDbResultSet(SqlCommand command)
        {
            Logger.LogInformation("Reading query results.");

            var results = new List<Dictionary<string, object>>();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                // Read each row and add it to the dictionary
                while (reader.Read())
                {
                    var resultDictionary = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string columnName = reader.GetName(i);
                        object value = reader[i]; // Get value by column index
                        //Logger.LogInformation("Found column " + columnName + "with value " + value);
                        resultDictionary.Add(columnName, value);
                    }
                    results.Add(resultDictionary);
                }
            }

            Logger.LogInformation("Query Results Read.");

            return results;
        }

        public void CreateInsertWithDictionary(SqlCommand command, string tableName, Dictionary<string, object> dictionary)
        {
            var columnsString = "";
            var valuesString = "";

            foreach (var item in dictionary)
            {
                columnsString += item.Key + ",";
                valuesString += "@" + item.Key + ",";

                command.Parameters.AddWithValue(item.Key, item.Value);
            }

            columnsString = columnsString.Trim(',');
            valuesString = valuesString.Trim(',');

            command.CommandText =
@"INSERT INTO " + tableName + @" 
    (" + columnsString + @")
VALUES 
    (" + valuesString + @")";

            Logger.LogInformation("Created INSERT Query: " + command.CommandText);
        }
    }
}
