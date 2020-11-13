using Discord;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VetCoin.Data
{
    public class Trade : ICreate,IUpdate, IMember
    {
        public int Id { get; set; }
        public int VetMemberId { get; set; }

        [DisplayName("種別")]
        public Direction Direction { get; set; }

        [Required]
        [DisplayName("タイトル")]
        public string Title { get; set; }

        [Required]
        [DisplayName("内容")]
        public string Content { get; set; }

        [DisplayName("希望価格")]
        public int? Reward { get; set; }

        [DisplayName("希望価格(補足)")]
        public string RewardComment { get; set; }

        [DisplayName("納期")]
        public string DeliveryDate { get; set; }

        [DisplayName("ステータス")]
        public TradeStatus TradeStatus { get; set; }

        [DisplayName("事前相談")]
        public ConsultationRequest ConsultationRequest { get; set; }

        public virtual ICollection<TradeMessage> TradeMessages { get; set; }
        public virtual ICollection<Contract> Contracts { get; set; }

        public string CreateUser { get; set; }
        public DateTimeOffset CreateDate { get; set; }

        public string UpdateUser { get; set; }
        public DateTimeOffset UpdateDate { get; set; }

        public virtual VetMember VetMember { get; set; }

        public virtual ICollection<TradeImage> TradeImages { get; set; }

        [DisplayName("募集継続")]
        public bool IsContinued { get; set; }

        public DateTimeOffset OrderRefDate { get; set; }

        public Trade Clone()
        {
            return this.MemberwiseClone() as Trade;
        }

    }

    public enum Direction
    {
        [Display(Name = "商品・サービスの販売")]
        Sell,
        [Display(Name = "働き手の募集")]
        Buy,
    }

    public enum TradeStatus
    {
        [Display(Name = "受付")]
        Open,
        [Display(Name = "終了")]
        Close,
        [Display(Name = "キャンセル")]
        Cancel,
    }


    public enum ConsultationRequest
    {
        [Display(Name = "事前相談不要")]
        None,
        [Display(Name = "必要に応じて")]
        CaseByCase,
        [Display(Name = "事前相談必須")]
        Required,
    }

}


