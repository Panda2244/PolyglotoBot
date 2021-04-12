using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyglotoBot.Models.DBModels
{
    public class ResultsDictionary
    {
        public string Id { get; set; }

        public string ResultsId { get; set; }

        public string Dictionary { get; set; }

        public ResultsDictionary(string id, string resultsId, string dictionary)
        {
            Id = id;
            ResultsId = resultsId;
            Dictionary = dictionary;
        }
    }
}
