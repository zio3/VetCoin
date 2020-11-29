using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VetCoin.Data
{
    public class Donation : IUpdate,ICreate
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string Content { get; set; }

        public int VetMemberId { get; set; }

        public virtual VetMember VetMember { get; set; }
        public virtual ICollection<Doner> Doners { get; set; }
        public virtual ICollection<DonationLog> DonationLogs { get; set; }

        public string CreateUser { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTimeOffset UpdateDate { get; set; }

        public DonationState DonationState { get; set; }

        public virtual ICollection<DonationMessage> DonationMessages { get; set; }

    }

    public enum DonationState
    {
        [Display(Name="募集中")]
        Open,
        [Display(Name = "終了")]
        Close,
        [Display(Name = "キャンセル")]
        Cancel
    }
}


