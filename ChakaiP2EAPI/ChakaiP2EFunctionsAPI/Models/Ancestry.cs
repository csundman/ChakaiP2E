using ChakaiP2EFunctionsAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChakaiP2EFunctionsAPI.Models
{
    [Plural("Ancestries")]
    public class Ancestry : BaseModel
    {
        [NoInsert, NoUpdate, Id]
        public int AncestryId { get; set; }
        public string AncestryName { get; set; }
    }
}
