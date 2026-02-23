using ChakaiP2EFunctionsAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChakaiP2EFunctionsAPI.Models
{
    [View, Plural("ChakaiSchema")]
    internal class ChakaiSchema : BaseModel
    {
        [NoInsert, NoUpdate]
        public int ChakaiSchemaId { get; set; }
        [NoInsert, NoUpdate]
        public string ChakaiSchemaName { get; set; }
        [NoInsert, NoUpdate]
        public string ChakaiSchemaShortName {  get; set; }
        [NoInsert, NoUpdate]
        public string ChakaiSchemaTable { get; set; }

    }
}
