using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace FunctionApp1
{
    public static class Function1
    {
        private static HttpClient client = new HttpClient();

        [FunctionName("Function1")]
        public static void Run([IoTHubTrigger("messages/events", Connection = "connString")]EventData message, ILogger log)
        {
            log.LogInformation($"C# IoT Hub trigger function processed a message: {Encoding.UTF8.GetString(message.Body.Array)}");

            var messageData = (message.Body != null && message.Body.Array.Length > 0) ? Encoding.UTF8.GetString(message.Body.Array) : string.Empty;

            EventMessage eventMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<EventMessage>(messageData);

            string accountSid = "ACb5c7af1242aae4f4b46928c358d8899d";
            string authToken = "eb54e3ba9fbda80864ddb81489b0fe07";

            log.LogInformation($"Sugar Level is - {eventMessage.SugarLevel}");

            if (eventMessage.SugarLevel > 100)
            {
                TwilioClient.Init(accountSid, authToken);

                MessageResource.Create(
                    body: "Your sugar level has been increased! Please have either medicine or control the sugar level. Reach out to doctor in case of any emergency.",
                    from: new Twilio.Types.PhoneNumber("+13512137825"),
                    to: new Twilio.Types.PhoneNumber("+13207614965")
                );
            }
        }
    }

    public class EventMessage
    {
        public int SugarLevel { get; set; }
    }
}