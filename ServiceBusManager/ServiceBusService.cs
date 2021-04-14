using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusManager
{
    public static class ServiceBusService
    {
        private const string servicebusConnectionString = "Endpoint=sb://polygloto-servicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=+XVpj0vEY8cwJaO1JGlIt506dPua38RhpOcYkHsVE4o=";
        private const string queueName = "schedule-messages-queue";

        public static async Task SendMessage(string text)
        {
            var queueClient = new QueueClient(servicebusConnectionString, queueName);
            var message = new Message(Encoding.UTF8.GetBytes(text));
            await queueClient.SendAsync(message).ConfigureAwait(false);
            await queueClient.CloseAsync().ConfigureAwait(false);
        }

        public static async Task SendScheduleMessage(byte[] model, int minutes)
        {
            var queueClient = new QueueClient(servicebusConnectionString, queueName);
            await queueClient.ScheduleMessageAsync(new Message(model), new DateTimeOffset(DateTime.UtcNow.AddSeconds(minutes))).ConfigureAwait(false);
            await queueClient.CloseAsync().ConfigureAwait(false);
        }

        public static async Task<Message> ReceiveMessages()
        {
            var messageReceiver = new MessageReceiver(servicebusConnectionString, queueName, ReceiveMode.ReceiveAndDelete);
            var message = await messageReceiver.ReceiveAsync().ConfigureAwait(false);
            await messageReceiver.CloseAsync().ConfigureAwait(false);
            return message;
        }
    }
}
