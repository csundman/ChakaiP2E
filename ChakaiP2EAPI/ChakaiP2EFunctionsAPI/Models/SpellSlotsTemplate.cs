using ChakaiP2EFunctionsAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChakaiP2EFunctionsAPI.Models
{
    public class SpellSlotsTemplate : BaseModel
    {
        public int SpellSlotsTemplateId { get; set; }
        public string SpellSlotsTemplateName { get; set; }
        public int CharacterId { get; set; }
        public int Level1 { get; set; }
        public int Level2 { get; set; }
        public int Level3 { get; set; }
        public int Level4 { get; set; }
        public int Level5 { get; set; }
        public int Level6 { get; set; }
        public int Level7 { get; set; }
        public int Level8 { get; set; }
        public int Level9 { get; set; }
        public int Level10 { get; set; }
        public int SpellCollectionId { get; set; }
        [JsonIgnore]
        public string RequiredTraitIds { get; set; }
        [NoSelect, NoInsert, NoUpdate]
        public List<int> RequiredTraitIdList { get; set; }

        public void ParseRequiredTraits()
        {
            RequiredTraitIdList = ParseIntCsv(
                RequiredTraitIds,
                minValue: 1,
                maxValue: 10
            );
        }

        public void BuildHeightenedLevels()
        {
            RequiredTraitIds = BuildIntCsv(
                RequiredTraitIdList,
                minValue: 1,
                maxValue: 10
            );
        }
    }
}
