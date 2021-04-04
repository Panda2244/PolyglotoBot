using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyglotoBot.Models
{
    public class Translate
    {
        [JsonProperty("outputs")]
        public Output[] Outputs { get; set; }
    }

    public partial class Output
    {
        [JsonProperty("output")]
        public string OutputOutput { get; set; }

        [JsonProperty("stats")]
        public Stats Stats { get; set; }
    }

    public partial class Stats
    {
        [JsonProperty("elapsed_time")]
        public long ElapsedTime { get; set; }

        [JsonProperty("nb_characters")]
        public long NbCharacters { get; set; }

        [JsonProperty("nb_tokens")]
        public long NbTokens { get; set; }

        [JsonProperty("nb_tus")]
        public long NbTus { get; set; }

        [JsonProperty("nb_tus_failed")]
        public long NbTusFailed { get; set; }
    }
}
