using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Azure;
using ChakaiP2EFunctionsAPI.Models;
using ChakaiP2EFunctionsAPI.Utilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace ChakaiP2EFunctionsAPI.Functions
{
    public class AccountsController : BaseController
    {
        //The environment variable name that contains the registration key needed to create new accounts
        private const string regEnv = "RegistrationKey";

        public AccountsController(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            InitializeFunction<AccountsController>(loggerFactory);
        }

        [Function("Accounts")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", "delete", Route = "accounts/{id?}")] HttpRequestData req, ILogger log, string id)
        {
            InitResponse(req);

            await HandleRequest(req, id, skipAuth: new List<string> { "post" });

            return Response;
        }

        protected override async Task HandleMultiGet()
        {
            //Start the SQL connection
            using (SqlConnection conn = new SqlConnection(SecurityUtil.ConnectionString))
            {
                await conn.OpenAsync();

                try
                {
                    var accounts = await SqlHelper.ExecuteSelect<Account>(conn, null, AccountID);

                    Response.StatusCode = HttpStatusCode.OK;
                    await Response.WriteAsJsonAsync(accounts);
                }
                catch (Exception ex)
                {
                    await WriteErrorToResponse(HttpStatusCode.InternalServerError, ex.Message);
                    return;
                }
            }
        }

        protected override async Task HandlePost(HttpRequestData req)
        {
            //First get the account creation data
            var accountData = await req.ReadFromJsonAsync<Account>();

            var registrationKey = Environment.GetEnvironmentVariable(regEnv);

            /*if (registrationKey != accountData.RegistrationKey)
            {
                await WriteErrorToResponse(HttpStatusCode.Unauthorized, "You do not have a valid registration key.");
                return;
            }*/

            //Start the SQL connection
            using (SqlConnection conn = new SqlConnection(SecurityUtil.ConnectionString))
            {
                await conn.OpenAsync();

                Account result = null;

                try
                {
                    result = await SqlHelper.ExecuteInsert(conn, accountData);
                }
                catch (Exception ex)
                {
                    await WriteErrorToResponse(HttpStatusCode.InternalServerError, $"A Sql Error Occured: {ex.Message}");
                    return;
                }

                //If we got here, we created the account
                Response.StatusCode = HttpStatusCode.Created;
                await Response.WriteAsJsonAsync(new Dictionary<string, object>
                {
                    { "message", "Account Creation Success!" }, { "result", result }
                });
            }
        }

        protected override async Task HandlePut(HttpRequestData req, int id)
        {
            //First get the account creation data
            var accountData = await req.ReadFromJsonAsync<Account>();

            //Start the SQL connection
            using (SqlConnection conn = new SqlConnection(SecurityUtil.ConnectionString))
            {
                await conn.OpenAsync();

                Account result = null;

                try
                {
                    result = await SqlHelper.ExecuteUpdate(conn, accountData, id);
                }
                catch (Exception ex)
                {
                    await WriteErrorToResponse(HttpStatusCode.InternalServerError, $"A Sql Error Occured: {ex.Message}");
                    return;
                }

                Response.StatusCode = HttpStatusCode.OK;
                await Response.WriteAsJsonAsync(new Dictionary<string, object>
                {
                    { "message", "Account Creation Success!" }, {"result", result }
                });
            }
        }
    }
}
