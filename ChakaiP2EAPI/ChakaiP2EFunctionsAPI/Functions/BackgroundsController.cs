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

namespace ChakaiP2EFunctionsAPI.Functions
{
    public class BackgroundsController : BaseController
    {
        private readonly ILogger _logger;

        public BackgroundsController(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            InitializeFunction<BackgroundsController>(loggerFactory);
        }

        [Function("Backgrounds")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", "delete", Route = "backgrounds/{id?}")] HttpRequestData req, ILogger log, string id)
        {
            InitResponse(req);

            await HandleRequest(req, id);

            return Response;
        }

        #region POST

        protected override async Task HandlePost(HttpRequestData req)
        {
            Background backgroundData = null;
            try
            {
                backgroundData = await req.ReadFromJsonAsync<Background>();
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
                    result = await SqlHelper.ExecuteInsert(conn, backgroundData);
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
                    { "message", "Background Creation Success!" }, { "result", result }
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
                    var backgrounds = await SqlHelper.ExecuteSelect<Background>(conn);

                    Response.StatusCode = HttpStatusCode.OK;
                    await Response.WriteAsJsonAsync(backgrounds);
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

                    var found = SqlHelper.ExecuteDelete(conn, "backgrounds", new Background { BackgroundId = id });

                    if (!found)
                    {
                        await WriteErrorToResponse(HttpStatusCode.NotFound, "No background found with Id " + id);
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
    }
}
