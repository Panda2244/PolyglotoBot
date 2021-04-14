using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PolyglotoBot.Bots;
using PolyglotoBot.DB;
using PolyglotoBot.Dialogs;
using PolyglotoBot.Services;
using System;
using System.Threading.Tasks;

namespace PolyglotoBot
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();
            services.AddSingleton<IStorage, MemoryStorage>();
            services.AddSingleton<UserState>();
            services.AddSingleton<ConversationState>();
            services.AddSingleton<ConfigurationVerificationDialog>();
            services.AddSingleton<MainDialog>();
            services.AddTransient<IBot, DialogAndWelcomeBot<MainDialog>>();
            services.AddHttpClient<DefinitionService>(c =>
            {
                c.BaseAddress = new Uri("https://twinword-word-graph-dictionary.p.rapidapi.com/");
                c.DefaultRequestHeaders.Add("x-rapidapi-key", "c37a88260cmsh41c47505a2d4263p19c42cjsn9311e035e535");
                c.DefaultRequestHeaders.Add("x-rapidapi-host", "twinword-word-graph-dictionary.p.rapidapi.com");
            });
            services.AddHttpClient<TranslateService>(c =>
            {
                c.BaseAddress = new Uri("https://systran-systran-platform-for-language-processing-v1.p.rapidapi.com/");
                c.DefaultRequestHeaders.Add("x-rapidapi-key", "c37a88260cmsh41c47505a2d4263p19c42cjsn9311e035e535");
                c.DefaultRequestHeaders.Add("x-rapidapi-host", "systran-systran-platform-for-language-processing-v1.p.rapidapi.com");
                //c.DefaultRequestHeaders.Add("useQueryString", "true");
            });
            services.AddSingleton<PolyglotoDbContext>();
            services.AddSingleton<IServicebusTrigger, ServicebusTrigger>();
            services.AddTransient<IMessageSender, MessageSender>();
            services.AddTransient<IDatePeriodService, DatePeriodService>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            // app.UseHttpsRedirection();
        }
    }
}
