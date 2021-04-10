using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using PolyglotoBot.Models.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyglotoBot.Services
{
    public class MessageSender : IMessageSender
    {
        public async Task SendMessageAsync(UserConfigurations userConfigurations, string messageText)
        {
            try
            {
                var userAccount = new ChannelAccount(userConfigurations.RecipientId, userConfigurations.RecipientName);
                var botAccount = new ChannelAccount(userConfigurations.FromId, userConfigurations.FromName);
                var connector = new ConnectorClient(new Uri(userConfigurations.ServiceUrl));

                IMessageActivity message = Activity.CreateMessageActivity();
                if (!string.IsNullOrEmpty(userConfigurations.ConversationId) && !string.IsNullOrEmpty(userConfigurations.ChannelId))
                {
                    message.ChannelId = userConfigurations.ChannelId;
                }
                else
                {
                    userConfigurations.ConversationId = (await connector.Conversations.CreateDirectConversationAsync(botAccount, userAccount)
                    .ConfigureAwait(false)).Id;
                }
                message.From = userAccount;
                message.Recipient = botAccount;
                message.Conversation = new ConversationAccount(id: userConfigurations.ConversationId);
                message.Text = messageText;
                message.Locale = "en-Us";
                await connector.Conversations.SendToConversationAsync((Activity)message).ConfigureAwait(false);
            }
            catch (Exception) { throw; }
        }
    }
}
