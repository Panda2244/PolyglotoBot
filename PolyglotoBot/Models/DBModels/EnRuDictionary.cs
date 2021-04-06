using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyglotoBot.Models.DBModels
{
    public class EnRuDictionary
    {
        public Guid Id { get; set; }

        public string EnWord { get; set; }

        public string RuWord { get; set; }

        public EnRuDictionary(Guid id, string enWord, string ruWord)
        {
            Id = id;
            EnWord = enWord;
            RuWord = ruWord;
        }

    }
}
