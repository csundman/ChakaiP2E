using ChakaiP2EFunctionsAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChakaiP2EFunctionsAPI.Models
{
    public class Background : BaseModel
    {
        [NoInsert, Id]
        public int BackgroundId { get; set; }
        public string BackgroundName { get; set; }
    }
}
