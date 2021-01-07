using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VetCoin.Data.JsonParamEntites
{
    public class SiteSetting
    {
        [DisplayName("TOPの注釈(Markdown記法)")]
        public string TitleDescription { get; set; }

        [DisplayName("寄付・クラファンの非表示")]
        public bool HideDonations { get; set; }

        [DisplayName("販売所の非表示")]
        public bool HideVenders { get; set; } = true;

    }
}
