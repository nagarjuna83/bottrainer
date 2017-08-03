using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuisTrainer.Model;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace LuisTrainer.Services
{
    public class LuisService : IAIService
    {

        public async Task Train(LuisModel model)
        {
            var client = new HttpClient();
            //var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "9f69440c3add42219149bc256245118d");

            var uri = "https://westus.api.cognitive.microsoft.com/luis/v1.0/prog/apps/04808423-d5b5-45fe-87bf-4946b38c14bc/example";

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(model));

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
            }
        }

        public async Task Train(IEnumerable<LuisModel> model)
        {
            foreach (var item in model)
            {
                await this.Train(item);
            }
            await TrainModel();

            //var client = new HttpClient();
            ////var queryString = HttpUtility.ParseQueryString(string.Empty);

            //// Request headers
            //client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "9f69440c3add42219149bc256245118d");

            //var uri = "https://westus.api.cognitive.microsoft.com/luis/v1.0/prog/apps/04808423-d5b5-45fe-87bf-4946b38c14bc/examples";

            //HttpResponseMessage response;

            //// Request body
            //byte[] byteData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(model));

            //using (var content = new ByteArrayContent(byteData))
            //{
            //    content.Headers.ContentType = new MediaTypeHeaderValue("application/json ");
            //    response = await client.PostAsync(uri, content);
            //}
        }
        private async Task TrainModel()
        {
            var client = new HttpClient();
            //var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "9f69440c3add42219149bc256245118d");

            var uri = "https://westus.api.cognitive.microsoft.com/luis/v1.0/prog/apps/04808423-d5b5-45fe-87bf-4946b38c14bc/train";

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{body}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
            }

        }
    }
}
