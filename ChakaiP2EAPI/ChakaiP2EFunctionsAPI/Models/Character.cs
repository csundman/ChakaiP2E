using ChakaiP2EFunctionsAPI.Functions;
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
    public class Character : BaseModel
    {
        [NoInsert, NoUpdate, Id]
        public int CharacterId { get; set; }

        [NoSelect, NoUpdate]
        public int AccountId { get; set; }

        public string CharacterName { get; set; }

        public int? Str { get; set; }
        public int? Dex { get; set; }
        public int? Con { get; set; }
        public int? Int { get; set; }
        public int? Wis { get; set; }
        public int? Cha { get; set; }

        public int? CClassId { get; set; }

        [NoInsert, NoUpdate]
        public string ClassName { get; set; }

        public int? AncestryId { get; set; }

        [NoInsert, NoUpdate]
        public string AncestryName { get; set; }

        public int? BackgroundId { get; set; }

        [NoInsert, NoUpdate]
        public string BackgroundName { get; set; }
        [NoInsert]
        public int? PartyId { get; set; }
        [NoInsert, NoUpdate, NoSelect]
        public string PartyInviteCode { get; set; }
        [NoInsert, NoUpdate]
        public string PartyName { get; set; }
        [NoInsert, NoUpdate]
        public string GmName { get; set; }

        [ChildCollection, Xref]
        public List<Action> Actions { get; set; }
        [ChildCollection, Xref]
        public List<Passive> Passives { get; set; }
        [ChildCollection]
        public List<SpellCollection> SpellCollections { get; set; }
        [ChildCollection]
        public List<PreparedSpellsTemplate> PreparedSpellsTemplates { get; set; }
        [ChildCollection]
        public List<SpellSlotsTemplate> SpellSlotsTemplates { get; set; }
    }
}
