using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChakaiP2EFunctionsAPI.Utilities
{
    public static class SecurityUtil
    {
        public static string ConnectionString { get; set; }

        //This is the name of the connection string variable in our Azure Functions Service
        public const string ConnectionStringEnv = "SQLAZURECONNSTR_ChakaiP2EFunctionsConnection";
    }
}
