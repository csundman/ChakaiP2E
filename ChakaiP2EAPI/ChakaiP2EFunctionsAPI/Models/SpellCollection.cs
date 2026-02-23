using ChakaiP2EFunctionsAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChakaiP2EFunctionsAPI.Models
{
    public class SpellCollection : BaseModel
    {
        [Id, NoInsert, NoUpdate]
        public int SpellCollectionId { get; set; }
        public string SpellCollectionName { get; set; }
        [NoUpdate]
        public int CharacterId { get; set; }
        [JsonIgnore]
        public string RequiredTraitIds { get; set; }
        [NoSelect, NoInsert, NoUpdate]
        public List<int> RequiredTraitIdList { get; set; }

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
