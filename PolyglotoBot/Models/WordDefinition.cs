using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyglotoBot.Models
{
    public partial class WordDefinition
    {
        [JsonProperty("entry")]
        public string Entry { get; set; }

        [JsonProperty("request")]
        public string Request { get; set; }

        [JsonProperty("response")]
        public string Response { get; set; }

        [JsonProperty("meaning")]
        public Meaning Meaning { get; set; }

        [JsonProperty("ipa")]
        public string Ipa { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("result_code")]
        public string ResultCode { get; set; }

        [JsonProperty("result_msg")]
        public string ResultMsg { get; set; }
    }

    public partial class Meaning
    {
        [JsonProperty("noun")]
        public string Noun { get; set; }

        [JsonProperty("verb")]
        public string Verb { get; set; }

        [JsonProperty("adverb")]
        public string Adverb { get; set; }

        [JsonProperty("adjective")]
        public string Adjective { get; set; }
    }
}