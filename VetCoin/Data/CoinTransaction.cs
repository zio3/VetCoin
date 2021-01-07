using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VetCoin.Data
{
    public class CoinTransaction : ICreate, IUpdate
    {
        public int Id { get; set; }

        public int SendeVetMemberId { get; set; }
        public int RecivedVetMemberId { get; set; }

        public long Amount { get; set; }
        public string Text { get; set; }

        public string CreateUser { get; set; }
        public string UpdateUser { get; set; }

        public CoinTransactionType TransactionType {get;set;}

        [DisplayName("日時")]
        public DateTimeOffset CreateDate { get; set; }
        public DateTimeOffset UpdateDate { get; set; }

        [DisplayName("SendMember")]
        public virtual VetMember SendVetMember { get; set; }

        [DisplayName("RecivedMember")]
        public virtual VetMember RecivedVetMember { get; set; }

        public virtual ICollection<Contract> EscrowContracts { get; set; }

    }

    public enum CoinTransactionType
    {
        Issue,
        InitialDistribution,
        RegularDistribution,
        SuperChat,
        ReactionSend,
        Contract,
        Transfer,
        DistributionFraction,
        Subscription,
        Donate,
        Vender,

        Other = 100
    }
}
