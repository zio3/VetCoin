using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VetCoin.Data.JsonParamEntites
{
    public class ScheduleInfo
    {
        public string MethodName { get; set; }

        [NotMapped]
        public string CronExpression { get; set; }

        public string OverideCronExpression { get; set; }

        public bool Disabled { get; set; }

        public bool SendVerboseInfo { get; set; }
    }
}
