using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyglotoBot.Models.DBModels
{
    public class UserConfigurations
    {
        public string ChatId { get; set; }

        public int WordCount { get; set; }

        public int RetryCount { get; set; }

        public UserConfigurations(string chatId, int wordCount, int retryCount)
        {
            ChatId = chatId;
            WordCount = wordCount;
            RetryCount = retryCount;
        }
    }
}
