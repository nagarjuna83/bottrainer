using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuisTrainer.Model;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using QNABot.Models;
using Lucene.Net.Analysis;
using System.Net.Mail;

namespace LuisTrainer.Services
{
    [Serializable]
    public class QnAService : IAIService
    {
        string kbId = "2a4c33cc-b5ab-45c9-b1d3-345ef2f665b2";
        string subscriptionKey = "da6a5d9566ad47dba0fe275c79970bf1";
        public async Task<QnAQuery> QueryQnABot(string Query)
        {
            QnAQuery QnAQueryResult = new QnAQuery();
            using (System.Net.Http.HttpClient client =
                new System.Net.Http.HttpClient())
            {
                string RequestURI = String.Format("{0}{1}{2}{3}{4}",
                    @"https://westus.api.cognitive.microsoft.com/",
                    @"qnamaker/v1.0/",
                    @"knowledgebases/",
                    kbId,
                    @"/generateAnswer");
                var httpContent =
                    new StringContent($"{{\"question\": \"{Query}\"}}",
                    Encoding.UTF8, "application/json");
                httpContent.Headers.Add(
                    "Ocp-Apim-Subscription-Key", subscriptionKey);
                System.Net.Http.HttpResponseMessage msg =
                    await client.PostAsync(RequestURI, httpContent);
                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse =
                        await msg.Content.ReadAsStringAsync();
                    QnAQueryResult =
                        JsonConvert.DeserializeObject<QnAQuery>(JsonDataResponse);
                }
            }
            return QnAQueryResult;
        }

        public async Task<List<QnAQuery>> GetFAQ()
        {
            string strFAQUrl = "";
            string strLine;
            var faqs = new List<QnAQuery>();
            // Get the URL to the FAQ (in .tsv form)
            using (System.Net.Http.HttpClient client =
                new System.Net.Http.HttpClient())
            {
                string RequestURI = String.Format("{0}{1}{2}{3}{4}",
                    @"https://westus.api.cognitive.microsoft.com/",
                    @"qnamaker/v2.0/",
                    @"knowledgebases/",
                    kbId,
                    @"? ");
                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", subscriptionKey);
                System.Net.Http.HttpResponseMessage msg =
                    await client.GetAsync(RequestURI);
                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse =
                        await msg.Content.ReadAsStringAsync();
                    strFAQUrl =
                        JsonConvert.DeserializeObject<string>(JsonDataResponse);
                }
            }
            // Make a web call to get the contents of the
            // .tsv file that contains the database
            var req = WebRequest.Create(strFAQUrl);
            var r = await req.GetResponseAsync().ConfigureAwait(false);
            // Read the response
            using (var responseReader = new StreamReader(r.GetResponseStream()))
            {
                // Read through each line of the response
                while ((strLine = responseReader.ReadLine()) != null)
                {
                    // Write the contents to the StringBuilder object
                    string[] strCurrentLine = strLine.Split('\t');
                    faqs.Add(new QnAQuery
                    {
                        Question = strCurrentLine[0],
                        Answer = strCurrentLine[1]
                    });
                }
            }
            // Return the contents of the StringBuilder object
            return faqs;
        }

        public async Task<string> UpdateQueryQnABot(
            string newQuestion, string newAnswer)
        {
            string strResponse = "";
            // Create the QnAKnowledgeBase that contains the new entry
            QnAKnowledgeBase objQnAKnowledgeBase = new QnAKnowledgeBase();
            QnaPair objQnaPair = new QnaPair();
            objQnaPair.question = newQuestion;
            objQnaPair.answer = newAnswer;

            Add objAdd = new Add();
            objAdd.qnaPairs = new List<QnaPair>();
            objAdd.urls = new List<string>();
            objAdd.urls.Add(@"https://docs.microsoft.com/en-in/azure/cognitive-services/qnamaker/faqs");
            objAdd.qnaPairs.Add(objQnaPair);
            objQnAKnowledgeBase.add = objAdd;

            using (System.Net.Http.HttpClient client =
                new System.Net.Http.HttpClient())
            {
                string RequestURI = String.Format("{0}{1}{2}{3}? ",
                    @"https://westus.api.cognitive.microsoft.com/",
                    @"qnamaker/v2.0/",
                    @"knowledgebases/",
                    kbId);
                using (HttpRequestMessage request =
                    new HttpRequestMessage(new HttpMethod("PATCH"), RequestURI))
                {
                    request.Content = new StringContent(
                        JsonConvert.SerializeObject(objQnAKnowledgeBase),
                        System.Text.Encoding.UTF8, "application/json");
                    request.Content.Headers.Add(
                        "Ocp-Apim-Subscription-Key",
                        subscriptionKey);
                    HttpResponseMessage response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        strResponse = $"Operation Add completed.";
                    }
                    else
                    {
                        string responseContent =
                            await response.Content.ReadAsStringAsync();
                        strResponse = responseContent;
                    }
                }
            }
            return strResponse;
        }

        public async Task<string> TrainAndPublish()
        {
            string strResponse = "";

            using (System.Net.Http.HttpClient client =
                new System.Net.Http.HttpClient())
            {
                string RequestURI = String.Format("{0}{1}{2}{3}",
                    @"https://westus.api.cognitive.microsoft.com/",
                    @"qnamaker/v2.0/",
                    @"knowledgebases/",
                    kbId);

                var httpContent =
                    new StringContent("",
                    Encoding.UTF8, "application/json");

                httpContent.Headers.Add(
                    "Ocp-Apim-Subscription-Key", subscriptionKey);

                System.Net.Http.HttpResponseMessage response =
                    await client.PutAsync(RequestURI, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    strResponse = $"Operation Train and Publish completed.";
                }
                else
                {
                    string responseContent =
                        await response.Content.ReadAsStringAsync();

                    strResponse = responseContent;
                }
            }

            return strResponse;
        }

        public async Task Train()
        {
            throw new NotImplementedException();


        }

        public Task Train(LuisModel model)
        {
            throw new NotImplementedException();
        }

        public async Task Train(IEnumerable<LuisModel> model)
        {
            var mongoService = new MongoService();
            var faqs = await this.GetFAQ();
            foreach (var message in model)
            {
                var answer = MapAnswer(faqs, message);
                var entry = new Entry {Question = message.ExampleText, Answer = answer?.Answer };
                if (answer != null)
                {
                    await this.UpdateQueryQnABot(message.ExampleText, answer.Answer);
                }
                await mongoService.CreateEntry(entry, answer != null);
            }
            await this.TrainAndPublish();
        }

        private QnAQuery MapAnswer(List<QnAQuery> faqs, LuisModel message)
        {
            var wordsToavoid = new List<string> { "how", "what", "when", "where", "" };
            var words = this.AnalyzeText(message.ExampleText).Where(x => !wordsToavoid.Contains(x));
            var answer = faqs.Where(x => x.Answer.ToLower().Split(' ').Intersect(words).Any()).FirstOrDefault();
            return answer;
        }

        public List<string> AnalyzeText(string message)
        {
            Analyzer analyzer = new StopAnalyzer();
            var wordList = new List<string>();

            if (analyzer != null)
            {
                StringBuilder sb = new StringBuilder();
                StringReader stringReader = new StringReader(message);
                TokenStream tokenStream = analyzer.TokenStream("defaultFieldName", stringReader);
                Token token = tokenStream.Next();

                var numberOfTokens = 0;

                while (token != null)
                {
                    numberOfTokens++;
                    wordList.Add(token.TermText());
                    token = tokenStream.Next();
                }

            }
            return wordList;
        }

        public void SendDetails(string quesion)
        {
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential("ganeshanthati@gmail.com", "oejlyekdforlpmoj");
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;
            MailMessage mail = new MailMessage();

            //Setting From , To and CC
            mail.From = new MailAddress("ganeshanthati@gmail.com");
            mail.To.Add(new MailAddress("Nagarjuna.Podapati@neudesic.com"));
            mail.To.Add(new MailAddress("ganesh.anthati@neudesic.com"));
            mail.Subject = "Bot Trainer Notification";
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.Body = $"Failed to Trian bot for question {quesion}";
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            smtpClient.Send(mail);
        }

    }
}
