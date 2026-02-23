using ChakaiP2EFunctionsAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChakaiP2EFunctionsAPI.Models
{
    public class BaseModel
    {
        [NoInsert, NoUpdate, NoSelect, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<string> NullFields { get; set; }
        [NoInsert, NoUpdate, NoSelect, JsonIgnore]
        public string FoundryId { get; set; }

        protected static List<int> ParseIntCsv(
        string csv,
        int minValue = int.MinValue,
        int maxValue = int.MaxValue)
        {
            if (string.IsNullOrWhiteSpace(csv))
                return new List<int>();

            return csv
                .Split(',')
                .Select(p => p.Trim())
                .Where(p => int.TryParse(p, out _))
                .Select(int.Parse)
                .Where(v => v >= minValue && v <= maxValue)
                .Distinct()
                .OrderBy(v => v)
                .ToList();
        }

        protected static string BuildIntCsv(
            IEnumerable<int> values,
            int minValue = int.MinValue,
            int maxValue = int.MaxValue)
        {
            if (values == null)
                return null;

            var normalized = values
                .Where(v => v >= minValue && v <= maxValue)
                .Distinct()
                .OrderBy(v => v)
                .ToList();

            return normalized.Count == 0
                ? null
                : string.Join(",", normalized);
        }
    }
}
