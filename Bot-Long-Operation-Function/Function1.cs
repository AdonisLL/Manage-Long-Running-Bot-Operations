using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Bot.Connector.DirectLine;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Bot_Long_Operation
{
    public class Function1
    {
        [FunctionName("Function1")]
        public static async Task RunAsync([QueueTrigger("myqueue-items", Connection = "QueueStorageConnection")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processing");

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
            Activity originalActivity = JsonConvert.DeserializeObject<Activity>(myQueueItem, jsonSettings);
            string activityString = originalActivity.Value.ToString();

            bool option1 = activityString.Equals("option 1", StringComparison.OrdinalIgnoreCase);
            bool option2 = activityString.Equals("option 2", StringComparison.OrdinalIgnoreCase);

            // Perform long operation here....
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(180));

            if (option1)
            {
                originalActivity.Value = " (Result for long operation one!)";
            }
            else if (option2)
            {
                originalActivity.Value = " (A different result for operation two!)";
            }

            originalActivity.Value = "LongOperationComplete:" + originalActivity.Value;
            var responseActivity = new Activity("event");
            responseActivity.Value = originalActivity;
            responseActivity.Name = "LongOperationResponse";
            responseActivity.From = new ChannelAccount("GenerateReport", "AzureFunction");

            var directLineSecret = Environment.GetEnvironmentVariable("DirectLineSecret");
            using (DirectLineClient client = new DirectLineClient(directLineSecret))
            {
                var conversation = await client.Conversations.StartConversationAsync();
                await client.Conversations.PostActivityAsync(conversation.ConversationId, responseActivity);
            }

            log.LogInformation($"Done...");
        }
    }
}
