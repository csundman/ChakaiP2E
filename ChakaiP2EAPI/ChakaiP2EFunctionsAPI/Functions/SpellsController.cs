using Azure;
using ChakaiP2EFunctionsAPI.Models;
using ChakaiP2EFunctionsAPI.Utilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ChakaiP2EFunctionsAPI.Functions
{
    public class SpellsController : BaseController
    {
        private readonly ILogger _logger;

        public SpellsController(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            InitializeFunction<SpellsController>(loggerFactory);
        }

        [Function("SpellsController")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "spells/{id?}")] HttpRequestData req, ILogger log, string id)
        {
            InitResponse(req);

            await HandleRequest(req, id);

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
                    var spells = await SqlHelper.ExecuteSelect<Spell>(conn);

                    foreach (var item in spells)
                    {
                        item.ParseHeightenedLevels();
                    }

                    Response.StatusCode = HttpStatusCode.OK;
                    await Response.WriteAsJsonAsync(spells);
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
