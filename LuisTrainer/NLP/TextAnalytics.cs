using DiffMatchPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuisTrainer.NLP
{
    public class TextAnalytics
    {
        public int GetScore(string source, string destination)
        {

            var matcher = new DiffMatchPatch.DiffMatchPatch(2000, 2000, 2000, 2000, 2000, 2000, 2000, 2000);
            var result = matcher.MatchMain(source, destination, 1000);
            return result;
        }
    }
}
