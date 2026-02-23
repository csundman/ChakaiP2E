using ChakaiP2EFunctionsAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChakaiP2EFunctionsAPI.Models
{
    [View]
    public class Action : BaseModel
    {
        [NoInsert, NoUpdate, Id]
        public int ActionId { get; set; }
        public string ActionName { get; set; }
        public string ActionDescription { get; set; }
        public int? ActionCostId { get; set; }
        [NoInsert, NoUpdate]
        public string ActionCostName { get; set; }
        public int? ActionCategoryId { get; set; }
        [NoInsert, NoUpdate]
        public string ActionCategoryName { get; set; }
        public int? GameplayModeId { get; set; }
        [NoInsert, NoUpdate]
        public string GameplayModeName { get; set; }
        public int? SourceTypeId { get; set; }
        [NoInsert, NoUpdate]
        public string SourceTypeName { get; set; }
        public bool SystemDefined { get; set; }
        [ChildCollection, Xref]
        public List<Trait> Traits { get; set; }
        [ChildCollection, Xref]
        public List<Roll> Rolls { get; set; }
    }
}
