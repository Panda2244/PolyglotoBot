using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyglotoBot.Models
{
    public class Period
    {
        public string UserDialogId { get; set; }
        //12:30, 09:55
        public List<string> TimePeriods { get; set; }
    }
}