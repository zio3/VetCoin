using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VetCoin.Data
{
    public class RuleTextLog : ICreate
    {
        public int Id { get; set; }

        [DisplayName("更新内容テキスト")]
        public string RuleMarkdown { get; set; }

        [DisplayName("更新者")]
        public string CreateUser { get; set; }
        [DisplayName("更新日")]
        public DateTimeOffset CreateDate { get; set; }
    }
}
