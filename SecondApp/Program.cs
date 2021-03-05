using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SecondApp
{
    class Program
    {
        static string ConnectionStringServiceBus = "";
        static string QueueName = "example-queue";

        static readonly QueueClient _queueClient = new QueueClient(ConnectionStringServiceBus, QueueName, ReceiveMode.PeekLock);

        static void Main(string[] args)
        {
            RegisterHandler();
        }

        private static void RegisterHandler()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionHandler)
            {
                AutoComplete = false
            };

            _queueClient.RegisterMessageHandler(ProccessMessageHandler, messageHandlerOptions);
        }

        private static async Task ProccessMessageHandler(Message message, CancellationToken cancellationToken)
        {
            string messageString = Encoding.UTF8.GetString(message.Body);
            var userFollowing = JsonConvert.DeserializeObject<UserFollowingInputModel>(messageString);

            Console.WriteLine("The user {0} started follow you. Followed at: {1}", userFollowing.IdUserFollower, userFollowing.FollowedAt);

            await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private static Task ExceptionHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine(exceptionReceivedEventArgs.Exception.Message);
            return Task.CompletedTask;
        }
    }
}
