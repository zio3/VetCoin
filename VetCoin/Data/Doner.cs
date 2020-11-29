using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VetCoin.Data
{
    public class Doner : ICreate
    {
        public int Id { get; set; }
        public int DonationId { get; set; }
        public virtual Donation Donation { get; set; }

        public int VetMemberId { get; set; }

        public virtual VetMember VetMember { get; set; }

        public int? CoinTransactionId { get; set; }

        public virtual CoinTransaction CoinTransaction { get; set; }

        public int Amount { get; set; }

        public string Comment { get; set; }

        public string CreateUser { get; set; }
        public DateTimeOffset CreateDate { get; set; }

        public DonerState DonerState { get; set; }
    }

    public enum DonerState
    {
        [Display(Name = "未受領")]
        Entry,
        [Display(Name = "受領済み")]
        Repted,
        [Display(Name ="キャンセル")]
        Cancel,
    }
}
