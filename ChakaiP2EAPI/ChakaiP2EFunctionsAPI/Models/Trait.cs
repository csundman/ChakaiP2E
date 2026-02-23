using ChakaiP2EFunctionsAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChakaiP2EFunctionsAPI.Models
{
    public class Trait : BaseModel
    {
        [NoInsert, NoUpdate, Id]
        public int TraitId { get; set; }
        public string TraitName { get; set; }
        public string TraitDescription { get; set; }
    }
}
