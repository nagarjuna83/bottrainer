using LuisTrainer.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LuisTrainer.Services
{
    public class AppInsightsService: IMonitoringService
    {
        public async Task<List<string>>  GetMessagesToTrain()
        {
            Rootobject odataResponse = new Rootobject();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.applicationinsights.io/beta/apps/4e04945f-eed9-40ce-939a-aa93d738d2c9/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("X-Api-Key", "uoyt18cd8e38gphhtqruqkqzd8zsdj0sotmogh1a");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.GetAsync("events/customEvents?$filter=customEvent/name eq 'FailedToAnswer' &timestamp gt now() sub duration'PT1H'?$orderby=timerstamp&$select=customDimensions/message");
                if (response.IsSuccessStatusCode)
                {
                    var messages = await response.Content.ReadAsStringAsync();
                    odataResponse = JsonConvert.DeserializeObject<Rootobject>(messages);
                }
            }

            return odataResponse.value.Select(c => c.customDimensions).Select(x => x.message).ToList();
        }
    }
}
