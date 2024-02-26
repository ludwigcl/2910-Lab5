using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab5
{
    public class APICallData
    {
        public Dictionary<int, string> PlantNamesWithIDs { get; set; } = new Dictionary<int, string>();
        public Dictionary<string, string> Links { get; set; }
        public string ScientificName { get; set; }
        public string CommonName { get; set; }
        public string FamilyCommonName { get; set; }
        public bool Vegetable { get; set; }
        public string Observations { get; set; }
        public int? Year { get; set; }
        public string Author { get; set; }
        public List<string> NativeDistribution { get; set; } = new List<string>();

        public APICallData()
        {
            PlantNamesWithIDs = new Dictionary<int,string>();
            Links = new Dictionary<string, string>();
        }

    }
}
