using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuisTrainer.Model
{

    public class Rootobject
    {
        public string odatacontext { get; set; }
        public AiMessages[] aimessages { get; set; }
        public Value[] value { get; set; }
    }

    public class AiMessages
    {
        public string code { get; set; }
        public string message { get; set; }
    }

    public class Value
    {
        public Customdimensions customDimensions { get; set; }
    }

    public class Customdimensions
    {
        public string message { get; set; }
    }

}
