using LuisTrainer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuisTrainer.Services
{
    public interface IAIService
    {
        Task Train(LuisModel model);
        Task Train(IEnumerable<LuisModel> model);
    }
}
