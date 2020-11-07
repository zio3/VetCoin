using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VetCoin.Data
{
    public class Subscription :ICreate,IUpdate, IMember
    {
        public int Id { get; set; }

        [DisplayName("タイトル")]
        public string Title { get; set; }

        public string Content { get; set; }

        [DisplayName("月額料金")]
        public int Fee { get; set; }

        public virtual ICollection<SubscriptionMember> SubscriptionMembers { get; set; }
        public string CreateUser { get; set; }

        public int VetMemberId { get; set; }

        public virtual VetMember VetMember { get; set; }

        [DisplayName("作成日")]
        public DateTimeOffset CreateDate  { get; set; }
        public string UpdateUser  { get; set; }
        public DateTimeOffset UpdateDate  { get; set; }

        public SubscriptionStatus SubscriptionStatus { get; set; }
    }
    public enum SubscriptionStatus
    {
        Open,
        Cancel,
    }

}
