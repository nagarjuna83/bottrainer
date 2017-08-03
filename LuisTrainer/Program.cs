using LuisTrainer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using LuisTrainer.Services;
using LuisTrainer.NLP;

namespace BotTrainer
{
    class Program
    {
        static QnAService aiService = new QnAService(); //inject dependencies
        static IMLService mlService = new MLService();
        static IMonitoringService appInsightService = new AppInsightsService();

        static void Main(string[] args)
        {
            try
            {
                //var ta = new TextAnalytics();
                Console.WriteLine("Fetchting telemetry from application insights!");
                var messagesToTrainTask = appInsightService.GetMessagesToTrain();
                var aiModels = mlService.GetAIModels(messagesToTrainTask.Result.Take(5).ToList());
                Console.WriteLine("Feeding QnA service..");
                var response2 = aiService.Train(aiModels);
                response2.Wait();
                Console.Write("Training completed");
                Console.ReadLine();
                //Task.WaitAll(Trainresponse);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
