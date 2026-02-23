using ChakaiP2EFunctionsAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChakaiP2EFunctionsAPI.Models
{
    public class Roll : BaseModel
    {
        [NoInsert, NoUpdate, Id]
        public int RollId { get; set; }
    }
}
