using ChakaiP2EFunctionsAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChakaiP2EFunctionsAPI.Models
{
    [View]
    public class Spell : BaseModel
    {
        [Id, NoInsert, NoUpdate]
        public int SpellId { get; set; }
        public string SpellName { get; set; }
        public string SpellDescription { get; set; }
        public int? ActionCostId { get; set; }
        [NoInsert, NoUpdate]
        public string ActionCostName { get; set; }
        public string SpellRange { get; set; }
        public string Defense { get; set; }
        public string SpellTarget { get; set; }
        public string Duration { get; set; }
        public bool? Sustained { get; set; }
        public string Area { get; set; }
        public int? SpellLevel { get; set; }
        [JsonIgnore]
        public string HeightenedLevels { get; set; }
        [NoInsert, NoUpdate, NoSelect]
        public List<int> HeightenedLevelList { get; set; }
        public bool? IsRemaster { get; set; }
        [ChildCollection, Xref]
        public List<Trait> Traits { get; set; }

        public void ParseHeightenedLevels()
        {
            HeightenedLevelList = ParseIntCsv(
                HeightenedLevels,
                minValue: 1,
                maxValue: 10
            );
        }

        public void BuildHeightenedLevels()
        {
            HeightenedLevels = BuildIntCsv(
                HeightenedLevelList,
                minValue: 1,
                maxValue: 10
            );
        }
    }
}
