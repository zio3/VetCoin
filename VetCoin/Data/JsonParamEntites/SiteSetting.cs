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
        [DisplayName("TOPの注釈テキスト")]
        public string TitleDescription { get; set; }

        [DisplayName("寄付・クラファンの非表示")]
        public bool HideDonations { get; set; }
    }
}
