using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyglotoBot.Models.DBModels
{
    public class Results
    {

        public string Id { get; set; }

        public string ChatId { get; set; }

        public string UserName { get; set; }

        public Results(string id, string chatId, string userName)
        {
            Id = id;
            ChatId = chatId;
            UserName = userName;
        }
    }
}
