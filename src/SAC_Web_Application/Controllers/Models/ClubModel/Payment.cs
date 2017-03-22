using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SAC_Web_Application.Models.ClubModel
{
    [Table("Payments")]
    public class Payment
    {
        [Key]
        public string PaymentID { get; set; }
        public DateTime CreateTime { get; set; }
        public string UpdateTime { get; set; }
        public string Amount { get; set; }

        public List<MemberPayment> MemberPayments { get; set; }
    }
}
