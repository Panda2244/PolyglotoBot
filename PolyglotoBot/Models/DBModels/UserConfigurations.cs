using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyglotoBot.Models.DBModels
{
    public class UserConfigurations
    {
        public string ConversationId { get; set; }

        public string RecipientName { get; set; }

        public string RecipientId { get; set; }

        public string FromName { get; set; }

        public string FromId { get; set; }

        public string ServiceUrl { get; set; }

        public string ChannelId { get; set; }

        public int WordCount { get; set; }

        public int RetryCount { get; set; }

        public UserConfigurations(string conversationId, string recipientName, string recipientId, string fromName,
          string fromId, string serviceUrl, string channelId, int wordCount, int retryCount)
        {
            ConversationId = conversationId;
            RecipientName = recipientName;
            RecipientId = recipientId;
            FromName = fromName;
            FromId = fromId;
            ServiceUrl = serviceUrl;
            ChannelId = channelId;
            WordCount = wordCount;
            RetryCount = retryCount;
        }
    }
}
