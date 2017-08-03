using LuisTrainer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuisTrainer.Services
{
    public interface IMLService
    {
        AiModel GetAIModel(string message);
        IEnumerable<LuisModel> GetAIModels(List<string> messages);
    }

}
