using Discord.Commands.Builders;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VetCoin.Data
{
    public class Contract : ICreate, IUpdate
    {
        public int Id { get; set; }

        public int TradeId { get; set; }

        [DisplayName("価格")]
        public int Reword { get; set; }
        
        [DisplayName("納期")]
        public string DeliveryDate { get; set; }

        [DisplayName("提案内容")]
        public string AgreementContent { get; set; }

        public bool TraderSigne { get; set; }
        public bool ContractorSigne { get; set; }

        [DisplayName("事前相談")]
        public ConsultationRequest ConsultationRequest { get; set; }

        public int VetMemberId { get; set; }

        public int? EscrowTransactionId { get; set; }
        public virtual CoinTransaction EscrowTransaction { get; set; }

        public ContractStatus ContractStatus { get; set; }

        public string CreateUser { get; set; }
        public DateTimeOffset CreateDate { get; set; }

        public string UpdateUser { get; set; }
        public DateTimeOffset UpdateDate { get; set; }

        public virtual VetMember VetMember { get; set; }

        public virtual Trade Trade { get; set; }

        public virtual ICollection<ContractMessage> ContractMessages { get; set; }

        public virtual ICollection<ContractImage> ContractImages { get; set; }
    }

    public enum ContractStatus
    {
        [Display(Name = "提案中")]
        Suggestion,
        [Display(Name = "作業中")]
        Working,
        [Display(Name = "完了")]
        Complete,
        [Display(Name = "キャンセル")]
        Canceled
    }

}
