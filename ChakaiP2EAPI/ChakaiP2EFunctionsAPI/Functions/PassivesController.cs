using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Net;
using ChakaiP2EFunctionsAPI.Utilities;
using System.Collections.Generic;
using System;
using Microsoft.Data.SqlClient;

namespace ChakaiP2EFunctionsAPI.Functions
{
    public class PassivesController : BaseController
    {
        private readonly ILogger _logger;

        public PassivesController(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            InitializeFunction<PassivesController>(loggerFactory);
        }

        [Function("Passives")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", "delete", Route = "passives/{id?}")] HttpRequestData req, ILogger log, string id)
        {
            InitResponse(req);

            await HandleRequest(req, id);

            return Response;
        }

        #region POST

        protected override async Task HandlePost(HttpRequestData req)
        {
            Models.Passive passiveData = null;
            try
            {
                passiveData = await req.ReadFromJsonAsync<Models.Passive>();
            }
            catch (Exception ex)
            {
                await WriteErrorToResponse(HttpStatusCode.InternalServerError, ex.Message);
                return;
            }

            //Start the SQL connection
            using (SqlConnection conn = new SqlConnection(SecurityUtil.ConnectionString))
            {
                await conn.OpenAsync();

                object result;

                try
                {
                    result = await SqlHelper.ExecuteInsert(conn, passiveData);
                }
                catch (Exception ex)
                {
                    await WriteErrorToResponse(HttpStatusCode.InternalServerError, "SQL ERROR: " + ex.Message);
                    return;
                }

                //If we got here, we created the item
                Response.StatusCode = HttpStatusCode.Created;
                await Response.WriteAsJsonAsync(new Dictionary<string, object>
                {
                    { "message", "Passive Creation Success!" }, { "result", result }
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
                    var passives = await SqlHelper.ExecuteSelect<Models.Passive>(conn);

                    Response.StatusCode = HttpStatusCode.OK;
                    await Response.WriteAsJsonAsync(passives);
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

                    var found = SqlHelper.ExecuteDelete(conn, "passives", new Models.Passive { PassiveId = id });

                    if (!found)
                    {
                        await WriteErrorToResponse(HttpStatusCode.NotFound, "No passive found with Id " + id);
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
            Models.Passive passiveData = null;
            try
            {
                passiveData = await req.ReadFromJsonAsync<Models.Passive>();
            }
            catch (Exception ex)
            {
                await WriteErrorToResponse(HttpStatusCode.InternalServerError, ex.Message);
                return;
            }

            if (passiveData == null)
            {
                await WriteErrorToResponse(HttpStatusCode.BadRequest, "Could not parse JSON");
            }

            //Start the SQL connection
            using (SqlConnection conn = new SqlConnection(SecurityUtil.ConnectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    var updatedPassive = await SqlHelper.ExecuteUpdate(conn, passiveData, id);

                    if (updatedPassive == null)
                    {
                        await WriteErrorToResponse(HttpStatusCode.NotFound, "No passive found with Id " + id);
                    }

                    Response.StatusCode = HttpStatusCode.OK;
                    await Response.WriteAsJsonAsync(new Dictionary<string, object>
                    {
                        { "message", "Passive Update Success!" }, { "result", updatedPassive }
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
