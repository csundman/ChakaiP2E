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
using System.Linq.Expressions;

namespace ChakaiP2EFunctionsAPI.Functions
{
    public class AncestriesController : BaseController
    {
        private readonly ILogger _logger;

        public AncestriesController(ILoggerFactory loggerFactory) : base(loggerFactory) 
        {
            InitializeFunction<AncestriesController>(loggerFactory);
        }

        [Function("Ancestries")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", "put", "delete", Route = "ancestries/{id?}")] HttpRequestData req, ILogger log, string id)
        {
            InitResponse(req);

            await HandleRequest(req, id);

            return Response;
        }

        #region POST

        protected override async Task HandlePost(HttpRequestData req)
        {
            Ancestry ancestryData = null;
            try
            {
                ancestryData = await req.ReadFromJsonAsync<Ancestry>();
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
                    result = await SqlHelper.ExecuteInsert(conn, ancestryData);
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
                    { "message", "Ancestry Creation Success!" }, { "result", result }
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
                    var ancestries = await SqlHelper.ExecuteSelect<Ancestry>(conn);

                    Response.StatusCode = HttpStatusCode.OK;
                    await Response.WriteAsJsonAsync(ancestries);
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

                    var found = SqlHelper.ExecuteDelete(conn, "ancestries", new Ancestry { AncestryId = id });

                    if (!found)
                    {
                        await WriteErrorToResponse(HttpStatusCode.NotFound, "No ancestry found with Id " + id);
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
