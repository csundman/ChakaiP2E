using Azure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Net;
using ChakaiP2EFunctionsAPI.Models;
using ChakaiP2EFunctionsAPI.Utilities;
using System.Collections.Generic;
using System;

namespace ChakaiP2EFunctionsAPI.Functions
{
    public class TraitsController : BaseController
    {
        private readonly ILogger _logger;

        public TraitsController(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            InitializeFunction<TraitsController>(loggerFactory);
        }

        [Function("Traits")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", "delete", Route = "traits/{id?}")] HttpRequestData req, ILogger log, string id)
        {
            InitResponse(req);

            await HandleRequest(req, id);

            return Response;
        }

        #region POST

        protected override async Task HandlePost(HttpRequestData req)
        {
            Trait traitData = null;
            try
            {
                traitData = await req.ReadFromJsonAsync<Trait>();
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
                    result = await SqlHelper.ExecuteInsert(conn, traitData);
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
                    { "message", "Trait Creation Success!" }, { "result", result }
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
                    var traits = await SqlHelper.ExecuteSelect<Trait>(conn);

                    Response.StatusCode = HttpStatusCode.OK;
                    await Response.WriteAsJsonAsync(traits);
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
            Trait traitData = null;
            try
            {
                traitData = await req.ReadFromJsonAsync<Trait>();
            }
            catch (Exception ex)
            {
                await WriteErrorToResponse(HttpStatusCode.InternalServerError, ex.Message);
                return;
            }

            if (traitData == null)
            {
                await WriteErrorToResponse(HttpStatusCode.BadRequest, "Could not parse JSON");
            }

            //Start the SQL connection
            using (SqlConnection conn = new SqlConnection(SecurityUtil.ConnectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    var updatedTrait = await SqlHelper.ExecuteUpdate(conn, traitData, id);

                    if (updatedTrait == null)
                    {
                        await WriteErrorToResponse(HttpStatusCode.NotFound, "No trait found with Id " + id);
                    }

                    Response.StatusCode = HttpStatusCode.OK;
                    await Response.WriteAsJsonAsync(new Dictionary<string, object>
                    {
                        { "message", "Trait Update Success!" }, { "result", updatedTrait }
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

        #region DELETE

        protected override async Task HandleDelete(int id)
        {
            //Start the SQL connection
            using (SqlConnection conn = new SqlConnection(SecurityUtil.ConnectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    var found = SqlHelper.ExecuteDelete(conn, "traits", new Trait { TraitId = id });

                    if (!found)
                    {
                        await WriteErrorToResponse(HttpStatusCode.NotFound, "No trait found with Id " + id);
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
