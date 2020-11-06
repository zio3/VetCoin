using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VetCoin.Data
{
    public class ScheduledExecutionLog
    {
        public int Id { get; set; }

        public string FunctionName { get; set; }

        public DateTimeOffset Start { get; set; }

        public DateTimeOffset? Finished { get; set; }

        public bool HasException { get; set; }

        public string ExceptionMessage { get; set; }
    }
}
