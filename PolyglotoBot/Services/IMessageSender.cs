using PolyglotoBot.Models.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyglotoBot.Services
{
    public interface IMessageSender
    {
        public Task SendMessageAsync(UserConfigurations userConfigurations, string messageText);
    }
}
