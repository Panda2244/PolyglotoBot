using Microsoft.Azure.ServiceBus;
using PolyglotoBot.DB;
using PolyglotoBot.Models;
using ServiceBusManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolyglotoBot.Services
{
    public class ServicebusTrigger : IServicebusTrigger
    {
        private readonly IMessageSender MessageSender;
        private readonly PolyglotoDbContext DbContext;

        public ServicebusTrigger(IMessageSender messageSender, PolyglotoDbContext dbContext)
        {
            MessageSender = messageSender;
            DbContext = dbContext;
        }

        public async Task Start()
        {
            while (true)
            {
                try
                {
                    var message = await ServiceBusService.ReceiveMessages();
                    if (message == null)
                    {
                        await Task.Delay(TimeSpan.FromMinutes(1)).ConfigureAwait(false);
                    }
                    else
                    {
                        await ProcessedMessage(message).ConfigureAwait(false);
                    }
                }
                catch (Exception ex) { var test = ex.Message; }
            }
        }


        private async Task ProcessedMessage(Message message)
        {
            try
            {
                var model = JsonSerializer.Deserialize<ScheduleMessageModel>(message.Body);
                var userConfigs = DbContext.UserConfigurations.FirstOrDefault(u => u.ConversationId.Equals(model.ConversationId));
                await MessageSender.SendMessageAsync(userConfigs, model.MessageText).ConfigureAwait(false);
            }
            catch (Exception ex) { var test = ex.Message; }
        }
    }
}
