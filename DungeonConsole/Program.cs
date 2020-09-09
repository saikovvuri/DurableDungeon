using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Storage;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DungeonConsole
{
    /// <summary>
    ///     This app will listen to the "console" queue to narrate the
    ///     durable functions demo.
    ///     
    ///     You must set environment variable "storage_connection" to the 
    ///     storage connection string. 
    ///     
    ///     It will default to storage emulator:
    ///         UseDevelopmentStorage=true;
    /// </summary>
    class Program
    {
        const string CONNECTION = "STORAGE_CONNECTION";
        const string QUEUE_NAME = "console";
        const string DEFAULT_CONNECTION = "UseDevelopmentStorage=true";

        static void Main(string[] args)
        {
            var storageConnection = Environment.GetEnvironmentVariable(CONNECTION);

            if (string.IsNullOrWhiteSpace(storageConnection))
            {
                Console.WriteLine($"{CONNECTION} not set. Defaulting to:");
                Console.WriteLine($"{DEFAULT_CONNECTION}");
                storageConnection = DEFAULT_CONNECTION;
            }

            // Instantiate a QueueClient which will be used to create and manipulate the queue
            QueueClient queueClient = new QueueClient(storageConnection, QUEUE_NAME);

            // Create the queue
            queueClient.CreateIfNotExists();

            Task.Run(async () => await WatchQueueAsync(queueClient)).Wait();
        }

        private static async Task WatchQueueAsync(QueueClient queueClient)
        {
            var running = true;
            var found = false;
            while (running)
            {
                var response = await queueClient.ReceiveMessagesAsync(5, TimeSpan.FromMinutes(5));
                var dequeue = new List<QueueMessage>();
                foreach (var message in response.Value)
                {
                    dequeue.Add(message);
                    if (!found)
                    {
                        Console.WriteLine("*");
                        found = true;
                    }
                    Console.WriteLine(message.MessageText);                    
                }
                if (dequeue.Count < 1)
                {
                    found = false;
                    Console.Write(".");
                }
                else
                {
                    foreach(var message in dequeue)
                    {
                        await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
                    }
                }
                Thread.Sleep(500);
                found = false;
            }
        }
    }
}
