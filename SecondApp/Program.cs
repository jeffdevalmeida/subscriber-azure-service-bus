using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SecondApp
{
    class Program
    {
        static string ConnectionStringServiceBus = "Endpoint=sb://sb-timetracker-dev.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Re6koaCjv7gkFJyUq6mBkmlPvG34PrbN+2MqpM9DWEY=";
        static string QueueName = "follower-queue";

        static IQueueClient _queueClient;

        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            _queueClient = new QueueClient(ConnectionStringServiceBus, QueueName, ReceiveMode.PeekLock);

            Console.WriteLine("Press ctrl-c to stop receiving messages.");

            ReceiveMessages();

            Console.ReadKey();
            // Close the client after the ReceiveMessages method has exited. 
            await _queueClient.CloseAsync();
        }

        // Receives messages from the queue in a loop 
        private static void ReceiveMessages()
        {
            try
            {
                // Register a OnMessage callback 
                _queueClient.RegisterMessageHandler(
                    async (message, token) =>
                    {
                        // Process the message 
                        Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

                        // Complete the message so that it is not received again. 
                        // This can be done only if the queueClient is opened in ReceiveMode.PeekLock mode. 
                        await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
                    },
                    new MessageHandlerOptions(exceptionReceivedEventArgs =>
                    {
                        Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
                        return Task.CompletedTask;
                    })
                    { MaxConcurrentCalls = 1, AutoComplete = false });
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} > Exception: {exception.Message}");
            }
        }

    }
}
