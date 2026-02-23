using ChakaiP2EFunctionsAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChakaiP2EFunctionsAPI.Models
{
    [View]
    public class Passive : BaseModel
    {
        [NoInsert, NoUpdate, Id]
        public int PassiveId { get; set; }
        public string PassiveName { get; set; }
        public string PassiveDescription { get; set; }
        public int SourceTypeId { get; set; }
        [NoInsert, NoUpdate]
        public string SourceTypeName { get; set; }
        [ChildCollection, Xref]
        public List<Trait> Traits { get; set; }
        [ChildCollection, Xref]
        public List<Roll> Rolls { get; set; }
        [ChildCollection, Xref]
        public List<Action> Actions { get; set; }
    }
}
