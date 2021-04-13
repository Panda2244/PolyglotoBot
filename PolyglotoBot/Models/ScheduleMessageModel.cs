using System;
using System.Collections.Generic;
using System.Text;

namespace PolyglotoBot.Models
{
    public class ScheduleMessageModel
    {
        public ScheduleMessageModel(string conversationId, string messageText)
        {
            ConversationId = conversationId;
            MessageText = messageText;
        }

        public string ConversationId { get; set; }
        public string MessageText { get; set; }
    }
}
