using ChakaiP2EFunctionsAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChakaiP2EFunctionsAPI.Models
{
    public class PreparedSpellsTemplate : BaseModel
    {
        [Id, NoInsert, NoUpdate]
        public int PreparedSpellsTemplateId { get; set; }
        public string PreparedSpellsTemplateName { get; set; }
        [NoUpdate]
        public int CharacterId { get; set; }
        [JsonIgnore]
        public string RequiredTraitIds { get; set; }
        [NoSelect, NoInsert, NoUpdate]
        public List<int> RequiredTraitIdList { get; set; }
        public int? ExtraLevel1 { get; set; }
        public int? ExtraLevel2 { get; set; }
        public int? ExtraLevel3 { get; set; }
        public int? ExtraLevel4 { get; set; }
        public int? ExtraLevel5 { get; set; }
        public int? ExtraLevel6 { get; set; }
        public int? ExtraLevel7 { get; set; }
        public int? ExtraLevel8 { get; set; }
        public int? ExtraLevel9 { get; set; }
        public int? ExtraLevel10 { get; set; }

        [ChildCollection, Xref]
        public List<Spell> Spells { get; set; }

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
