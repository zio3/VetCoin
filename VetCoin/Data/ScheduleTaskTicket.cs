using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VetCoin.Data
{
    public class ScheduleExecutionTicket
    {
        public string Id { get; set; }

        public string FunctionName { get; set; }

        public string Enviroment { get; set; }

        public DateTimeOffset DateTime { get; set; }
    }
}
