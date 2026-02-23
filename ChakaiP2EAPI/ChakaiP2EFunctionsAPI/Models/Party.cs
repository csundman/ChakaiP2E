using ChakaiP2EFunctionsAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChakaiP2EFunctionsAPI.Models
{
    [Plural("Parties")]
    internal class Party : BaseModel
    {
        [NoInsert, NoUpdate, Id]
        public int PartyId { get; set; }
        public string PartyName { get; set; }
        public int AccountId { get; set; }
        [NoInsert, Filter]
        public string PartyInviteCode { get; set; }
        [ChildCollection, NoInsert, NoUpdate]
        public List<Character> Characters { get; set; }
    }
}
