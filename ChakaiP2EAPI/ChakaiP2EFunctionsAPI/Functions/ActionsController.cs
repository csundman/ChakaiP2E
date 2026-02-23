using ChakaiP2EFunctionsAPI.Models;
using ChakaiP2EFunctionsAPI.Utilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System;

namespace ChakaiP2EFunctionsAPI.Functions
{
    public class ActionsController : BaseController
    {
        private readonly ILogger _logger;

        public ActionsController(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            InitializeFunction<ActionsController>(loggerFactory);
        }

        [Function("Actions")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", "delete", Route = "actions/{id?}")] HttpRequestData req, ILogger log, string id)
        {
            InitResponse(req);

            await HandleRequest(req, id);

            return Response;
        }

        #region POST

        protected override async Task HandlePost(HttpRequestData req)
        {
            Models.Action actionData = null;
            try
            {
                actionData = await req.ReadFromJsonAsync<Models.Action>();
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
                    result = await SqlHelper.ExecuteInsert(conn, actionData);
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
                    { "message", "Action Creation Success!" }, { "result", result }
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
                    var actions = await SqlHelper.ExecuteSelect<Models.Action >(conn);

                    Response.StatusCode = HttpStatusCode.OK;
                    await Response.WriteAsJsonAsync(actions);
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

                    var found = SqlHelper.ExecuteDelete(conn, "actions", new Models.Action { ActionId = id });

                    if (!found)
                    {
                        await WriteErrorToResponse(HttpStatusCode.NotFound, "No action found with Id " + id);
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
            Models.Action actionData = null;
            try
            {
                actionData = await req.ReadFromJsonAsync<Models.Action>();
            }
            catch (Exception ex)
            {
                await WriteErrorToResponse(HttpStatusCode.InternalServerError, ex.Message);
                return;
            }

            if (actionData == null)
            {
                await WriteErrorToResponse(HttpStatusCode.BadRequest, "Could not parse JSON");
            }

            //Start the SQL connection
            using (SqlConnection conn = new SqlConnection(SecurityUtil.ConnectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    var updatedAction = await SqlHelper.ExecuteUpdate(conn, actionData, id);

                    if (updatedAction == null)
                    {
                        await WriteErrorToResponse(HttpStatusCode.NotFound, "No action found with Id " + id);
                    }

                    Response.StatusCode = HttpStatusCode.OK;
                    await Response.WriteAsJsonAsync(new Dictionary<string, object>
                    {
                        { "message", "Action Update Success!" }, { "result", updatedAction }
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
