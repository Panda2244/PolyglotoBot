using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using PolyglotoBot.DB;
using PolyglotoBot.Enums;
using PolyglotoBot.Models;
using PolyglotoBot.Models.DBModels;
using PolyglotoBot.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace PolyglotoBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        protected readonly ILogger Logger;
        private readonly TranslateService TranslateService;

        public MainDialog(ConfigurationVerificationDialog configureDialog,
        ILogger<MainDialog> logger,
        TranslateService translateService)
           : base(nameof(MainDialog))
        {

            Logger = logger;
            TranslateService = translateService;

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

            //TestDB();
            // TranslateService use case
            // var test = await TranslateService.GetWordTranslate("”чить", Languages.ru);

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
            await TestAsync(stepContext, cancellationToken);
            var reply = MessageFactory.Text($"Configured! Wait for new word!");
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = reply }, cancellationToken);
        }

        private async Task TestAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var conversationId = stepContext.Context.Activity.Conversation.Id;
            var recipient = stepContext.Context.Activity.Recipient.Id;
            var locale = stepContext.Context.Activity.Locale;
            var from = stepContext.Context.Activity.From.Id;
            var id = stepContext.Context.Activity.Id;

            try
            {
                var userAccount = new ChannelAccount(stepContext.Context.Activity.Recipient.Id, stepContext.Context.Activity.Recipient.Name);
                var botAccount = new ChannelAccount(stepContext.Context.Activity.From.Id, stepContext.Context.Activity.From.Name);
                var connector = new ConnectorClient(new Uri(stepContext.Context.Activity.ServiceUrl));

                IMessageActivity message = Activity.CreateMessageActivity();
                if (!string.IsNullOrEmpty(stepContext.Context.Activity.Conversation.Id) && !string.IsNullOrEmpty(stepContext.Context.Activity.ChannelId))
                {
                    message.ChannelId = stepContext.Context.Activity.ChannelId;
                }
                else
                {
                    conversationId = (await connector.Conversations.CreateDirectConversationAsync(botAccount, userAccount)).Id;
                }
                message.From =  userAccount;
                message.Recipient = botAccount;
                message.Conversation = new ConversationAccount(id: conversationId);
                message.Text = "The text you want to send";
                message.Locale = "en-Us";
                await connector.Conversations.SendToConversationAsync((Activity)message);
            }
            catch (Exception ex) { var test = ex.Message; }
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptMessage = "What else can I do for you?";
            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        }



        private void TestDB()
        {
            //string dbName = "PolyglotoSqlLite.db";
            //if (File.Exists(dbName))
            //{
            //    File.Delete(dbName);
            //}
            try
            {
                using (var dbContext = new PolyglotoDbContext())
                {
                    //Ensure database is created
                    //  dbContext.Database.EnsureCreated();


                    if (!dbContext.EnRuDictionary.Any())
                    {
                        dbContext.EnRuDictionary.AddRange(
                         new List<EnRuDictionary>() {
                         new EnRuDictionary (Guid.NewGuid(), "an apple", "€блоко" )

                            });
                        dbContext.SaveChanges();
                    }
                    foreach (var item in dbContext.EnRuDictionary)
                    {
                        Console.WriteLine($"Id={item.EnWord}");
                    }
                }
            }
            catch (Exception ex) { var test = ex.Message; }
        }

    }
}

