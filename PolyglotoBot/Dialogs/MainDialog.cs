using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.EntityFrameworkCore;
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
        private readonly IMessageSender MessageSender;
        private readonly PolyglotoDbContext DbContext;
        private readonly DefinitionService DefinitionService;

        public MainDialog(ConfigurationVerificationDialog configureDialog,
        ILogger<MainDialog> logger,
        PolyglotoDbContext dbContext,
        TranslateService translateService,
        IMessageSender messageSender,
        DefinitionService definitionService)
           : base(nameof(MainDialog))
        {

            Logger = logger;
            TranslateService = translateService;
            MessageSender = messageSender;
            DbContext = dbContext;
            DefinitionService = definitionService;

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
            var listDefinition = await DefinitionService.DefinitionAsync("mask");

            /* var reply = MessageFactory.Text("Do you want configure me?");*/
            string result = string.Empty;
            foreach (var item in listDefinition)
            {
                result += $"\U0001F47D	{item}\n";
            }
            var reply = MessageFactory.Text(result);
            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    //USE CODES FOR EMOJI http://www.unicode.org/emoji/charts-beta/full-emoji-list.html#1f600 
                    //AND REPLACE '+' to '000'. Like U+1F44D -> \U0001F44D
                    new CardAction() { Title = "No \U0001F44E", Type = ActionTypes.ImBack, Value = "No" },
                    new CardAction() { Title = "Yes \U0001F44D", Type = ActionTypes.ImBack, Value = "Yes" },
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

                var wordCount = 0;
                var retryCount = 0;

                int.TryParse(matches.FirstOrDefault()?.Value, out wordCount);
                int.TryParse(matches.LastOrDefault()?.Value, out retryCount);

                var model = new UserConfigurations(
                    stepContext.Context.Activity.Conversation.Id,
                    stepContext.Context.Activity.Recipient.Name,
                    stepContext.Context.Activity.Recipient.Id,
                    stepContext.Context.Activity.From.Name,
                    stepContext.Context.Activity.From.Id,
                    stepContext.Context.Activity.ServiceUrl,
                    stepContext.Context.Activity.ChannelId,
                    wordCount,
                    retryCount);

                AddOrUpdate(model);
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
           // using var dbContext = new PolyglotoDbContext();
            var userConfigs = DbContext.UserConfigurations.FirstOrDefault(u => u.ConversationId.Equals(stepContext.Context.Activity.Conversation.Id));
            await MessageSender.SendMessageAsync(userConfigs, "testmessage");
            var reply = MessageFactory.Text($"Configured! Wait for new word!");
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = reply }, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptMessage = "What else can I do for you?";
            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        }


        private async Task AddOrUpdate(UserConfigurations model)
        {
            try
            {
             //   using var dbContext = new PolyglotoDbContext();
                DbContext.Database.EnsureCreated();
                if (await DbContext.UserConfigurations
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ConversationId.Equals(model.ConversationId)) != null)
                {
                    DbContext.UserConfigurations.Update(model);
                }
                else
                {
                   await DbContext.UserConfigurations.AddAsync(model).ConfigureAwait(false);
                }
               await DbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex) { var test = ex; }
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
                    dbContext.Database.EnsureCreated();


                    if (!dbContext.Results.Any())
                    {
                        dbContext.Results.AddRange(
                         new List<Results>() {
                         new Results ("1", "2", "3" )

                            });
                        dbContext.SaveChanges();
                    }
                    foreach (var item in dbContext.Results)
                    {
                        Console.WriteLine($"Id={item.Id}");
                    }
                }
            }
            catch (Exception ex) { var test = ex.Message; }
        }

    }
}

