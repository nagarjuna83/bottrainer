using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuisTrainer.Model
{

    public class LuisModel :AiModel
    {
        public string ExampleText { get; set; }
        public string SelectedIntentName { get; set; }
        public Entitylabel[] EntityLabels { get; set; }
    }

    public class Entitylabel
    {
        public string EntityType { get; set; }
        public int StartToken { get; set; }
        public int EndToken { get; set; }
    }

}
