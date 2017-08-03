using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuisTrainer.Services
{
    public interface IMonitoringService
    {
        Task<List<string>> GetMessagesToTrain();
    }
}
