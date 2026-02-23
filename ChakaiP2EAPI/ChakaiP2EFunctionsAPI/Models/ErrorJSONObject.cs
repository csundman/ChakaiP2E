using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ChakaiP2EFunctionsAPI.Models
{
    public class ErrorJSONObject
    {
        public List<string> Errors { get; set; } = new List<string>();

        public ErrorJSONObject(string error) 
        {
            Errors.Add(error);
        }
    }
}
