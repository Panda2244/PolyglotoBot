using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using PolyglotoBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace PolyglotoBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        protected readonly ILogger Logger;

        public MainDialog(ConfigurationVerificationDialog configureDialog, ILogger<MainDialog> logger)
           : base(nameof(MainDialog))
        {

            Logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(configureDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                FirstStepAsync,
                IntroStepAsync,
                ActStepAsync,
                VerifyUserAnswerStepAsync,
                ConfirmConfigureStepAsync,
                FinalStepAsync,
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }
        private async Task<DialogTurnResult> FirstStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text("Do you want configure me?");

            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    //USE CODES FOR EMOJI http://www.unicode.org/emoji/charts-beta/full-emoji-list.html#1f600 
                    //AND REPLACE '+' to '000'. Like U+1F44D -> \U0001F44D
                    new CardAction() { Title = "Yes \U0001F44D", Type = ActionTypes.ImBack, Value = "Yes" },
                    new CardAction() { Title = "No \U0001F44E", Type = ActionTypes.ImBack, Value = "No" },
                }
            };

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = reply }, cancellationToken);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userAnswer = stepContext.Result;
            var messageText = string.Empty;
            Activity promptMessage;

            switch (userAnswer)
            {
                case "No":
                    var res = userAnswer.ToString();
                    promptMessage = MessageFactory.Text("Ok, bye looser!");
                    stepContext.ActiveDialog.State["stepIndex"] = (int)stepContext.ActiveDialog.State["stepIndex"] - 3;
                    break;

                case "Yes":

                default:
                    messageText = stepContext.Options?.ToString() ?? "Say something like: \n\n\"*10 words per day and 20 times to repeat.*\"";
                    //+"\nSpecify the number of words first, then the number of repetitions for one word";
                    promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);
                    break;
            }

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            if (stepContext.Result == null)
            {
                throw new Exception($"{nameof(stepContext.Result)} is null");
            }

            try
            {
                var matches = Regex.Matches(stepContext.Result.ToString(), "([0-9]+)");
                var wordCount = matches.FirstOrDefault()?.Value;
                var retryCount = matches.LastOrDefault()?.Value;

                var model = new Configure()
                {
                    WordsCount = wordCount,
                    RetryCount = retryCount
                };

                return await stepContext.BeginDialogAsync(nameof(ConfigurationVerificationDialog), model, cancellationToken);

            }
            catch
            {
                return await stepContext.NextAsync(null, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> VerifyUserAnswerStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userAnswer = stepContext.Result;

            switch (userAnswer)
            {
                case "Confirm": break;

                case "Change":

                default:
                    stepContext.ActiveDialog.State["stepIndex"] = (int)stepContext.ActiveDialog.State["stepIndex"] - 3;
                    break;
            }
            return await stepContext.NextAsync(stepContext, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmConfigureStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text($"Configured! Waiting for new word!");
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = reply }, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptMessage = "What else can I do for you?";
            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        }
    }
}
