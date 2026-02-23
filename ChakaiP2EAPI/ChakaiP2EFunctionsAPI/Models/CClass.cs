using ChakaiP2EFunctionsAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChakaiP2EFunctionsAPI.Models
{
    [Plural("CClasses")]
    public class CClass : BaseModel
    {
        [NoInsert, Id]
        public int CClassId { get; set; }
        public string ClassName { get; set; }
    }
}
