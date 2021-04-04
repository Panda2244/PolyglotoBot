using Newtonsoft.Json;
using PolyglotoBot.Enums;
using PolyglotoBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

using System.Threading.Tasks;

namespace PolyglotoBot.Services
{
    public class TranslateService
    {
        private readonly HttpClient _httpClient;

        public TranslateService(HttpClient client)
        {
            _httpClient = client;
        }

        public async Task<Output> GetWordTranslate(string currentWord, Languages currentLanguage,
        Languages targetLanguage = Languages.en)
        {
            var targetLanguageName = Enum.GetName(typeof(Languages), targetLanguage);
            var currentLanguageNane = Enum.GetName(typeof(Languages), currentLanguage);

            var response = await _httpClient.GetAsync($"/translation/text/translate?" +
                                                        $"source={currentLanguageNane}" +
                                                        $"&target={targetLanguageName}" +
                                                        $"&input={currentWord}");

            response.EnsureSuccessStatusCode();

            var responsejson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Translate>(responsejson).Outputs.FirstOrDefault();
        }

    }
}
