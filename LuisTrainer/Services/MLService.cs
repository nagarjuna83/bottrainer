using LuisTrainer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuisTrainer.Services
{
    public class MLService : IMLService
    {
        public AiModel GetAIModel(string message)
        {
            return null;
        }

        public IEnumerable<LuisModel> GetAIModels(List<string> messages)
        {
            return messages.Select(x => new LuisModel { ExampleText = x, Intent = "Urgency" });
        }
    }
}
