using ChakaiP2EFunctionsAPI.Models;
using ChakaiP2EFunctionsAPI.Utilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace ChakaiP2EFunctionsAPI.Functions
{
    public class PartiesController : BaseController
    {
        public PartiesController(ILoggerFactory loggerFactory) : base (loggerFactory)
        {
            InitializeFunction<PartiesController>(loggerFactory);
        }

        [Function("Parties")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", "delete", Route = "parties/{id?}")] HttpRequestData req, ILogger log, string id)
        {
            InitResponse(req);

            await HandleRequest(req, id);

            return Response;
        }

        #region POST

        protected override async Task HandlePost(HttpRequestData req)
        {
            Party PartyData = null;
            try
            {
                PartyData = await req.ReadFromJsonAsync<Party>();
            }
            catch (Exception ex)
            {
                await WriteErrorToResponse(HttpStatusCode.InternalServerError, ex.Message);
                return;
            }

            PartyData.AccountId = AccountID.Value;

            //Start the SQL connection
            using (SqlConnection conn = new SqlConnection(SecurityUtil.ConnectionString))
            {
                await conn.OpenAsync();

                object result;

                try
                {
                    result = await SqlHelper.ExecuteInsert(conn, PartyData);
                }
                catch (Exception ex)
                {
                    await WriteErrorToResponse(HttpStatusCode.InternalServerError, ex.Message);
                    return;
                }

                //If we got here, we created the Party
                Response.StatusCode = HttpStatusCode.Created;
                await Response.WriteAsJsonAsync(new Dictionary<string, object>
                {
                    { "message", "Party Creation Success!" }, { "result", result }
                });
                return;
            }
        }

        #endregion

        #region MULTIGET

        protected override async Task HandleMultiGet()
        {
            //Start the SQL connection
            using (SqlConnection conn = new SqlConnection(SecurityUtil.ConnectionString))
            {
                await conn.OpenAsync();

                try
                {
                    var Parties = await SqlHelper.ExecuteSelect<Party>(conn, null, AccountID);

                    Response.StatusCode = HttpStatusCode.OK;
                    await Response.WriteAsJsonAsync(Parties);
                }
                catch (Exception ex)
                {
                    await WriteErrorToResponse(HttpStatusCode.InternalServerError, ex.Message);
                    return;
                }
            }
        }

        #endregion

        #region DELETE

        protected override async Task HandleDelete(int id)
        {
            //Start the SQL connection
            using (SqlConnection conn = new SqlConnection(SecurityUtil.ConnectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    var found = SqlHelper.ExecuteDelete(conn, "Parties", new Party { PartyId = id, AccountId = AccountID.Value }, AccountID);

                    if (!found)
                    {
                        await WriteErrorToResponse(HttpStatusCode.NotFound, "No Party found with Id " + id);
                    }
                }
                catch (Exception ex)
                {
                    await WriteErrorToResponse(HttpStatusCode.InternalServerError, ex.Message);
                    return;
                }
            }
        }

        #endregion

        #region PUT

        protected override async Task HandlePut(HttpRequestData req, int id)
        {
            Party PartyData = null;
            try
            {
                PartyData = await req.ReadFromJsonAsync<Party>();
            }
            catch (Exception ex)
            {
                await WriteErrorToResponse(HttpStatusCode.InternalServerError, ex.Message);
                return;
            }

            if (PartyData == null)
            {
                await WriteErrorToResponse(HttpStatusCode.BadRequest, "Could not parse JSON");
            }

            PartyData.AccountId = AccountID.Value;

            //Start the SQL connection
            using (SqlConnection conn = new SqlConnection(SecurityUtil.ConnectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    var updatedParty = await SqlHelper.ExecuteUpdate(conn, PartyData, id, AccountID);

                    if (updatedParty == null)
                    {
                        await WriteErrorToResponse(HttpStatusCode.NotFound, "No Party found with Id " + id);
                    }

                    Response.StatusCode = HttpStatusCode.OK;
                    await Response.WriteAsJsonAsync(new Dictionary<string, object>
                    {
                        { "message", "Party Update Success!" }, { "result", updatedParty }
                    });
                    return;
                }
                catch (Exception ex)
                {
                    await WriteErrorToResponse(HttpStatusCode.InternalServerError, ex.Message);
                    return;
                }
            }
        }

        #endregion
    }
}
