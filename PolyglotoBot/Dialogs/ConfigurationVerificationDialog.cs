using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using PolyglotoBot.Models;
using PolyglotoBot.Models.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PolyglotoBot.Dialogs
{
    public class ConfigurationVerificationDialog : CancelAndHelpDialog
    {
        public UserConfigurations userConfig;

        public ConfigurationVerificationDialog()
                   : base(nameof(ConfigurationVerificationDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
               ShowUserConfigurationStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ShowUserConfigurationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            userConfig = (UserConfigurations)stepContext.Options;

            if (userConfig.WordCount == 0 && userConfig.RetryCount == 0)
            {
                var promptMessage = MessageFactory.Text("*WordsCount* or *RetryCount* is 0 or empty, please try again.", InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }
            else {
                // IMPORTANTLY !!!!!!!!!!!!!!
                // some logic for saving userConfig -> if user choose 'change' userConfig -> re-save results here by next step
            }

            var reply = MessageFactory.Text($"You have selected: *{userConfig.WordCount}* words per day\n\nWith repetition: *{userConfig.RetryCount}* times");

            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    //USE CODES FOR EMOJI http://www.unicode.org/emoji/charts-beta/full-emoji-list.html#1f600 
                    //AND REPLACE '+' to '000'. Like U+1F44D -> \U0001F44D
                    new CardAction() { Title = "Confirm \U0001F44C", Type = ActionTypes.ImBack, Value = "Confirm" },
                    new CardAction() { Title = "Change \U0001F4DD", Type = ActionTypes.ImBack, Value = "Change" },
                }
            };

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = reply }, cancellationToken);
        }
    }
}
