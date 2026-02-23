using Azure;
using ChakaiP2EFunctionsAPI.Models;
using ChakaiP2EFunctionsAPI.Utilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ChakaiP2EFunctionsAPI.Functions
{
    public class ChakaiSchemaController : BaseController
    {
        private readonly ILogger _logger;

        public ChakaiSchemaController(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            InitializeFunction<ChakaiSchemaController>(loggerFactory);
        }

        [Function("ChakaiSchema")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "chakaischema/")] HttpRequestData req)
        {
            InitResponse(req);

            await HandleRequest(req, null, new List<string> { "get" });

            return Response;
        }

        #region MULTIGET

        protected override async Task HandleMultiGet()
        {
            //Start the SQL connection
            using (SqlConnection conn = new SqlConnection(SecurityUtil.ConnectionString))
            {
                await conn.OpenAsync();

                try
                {
                    var cClasses = await SqlHelper.ExecuteSelect<ChakaiSchema>(conn);

                    Response.StatusCode = HttpStatusCode.OK;
                    await Response.WriteAsJsonAsync(cClasses);
                }
                catch (Exception ex)
                {
                    await WriteErrorToResponse(HttpStatusCode.InternalServerError, ex.Message);
                    return;
                }
            }
        }
    }
}

#endregion
