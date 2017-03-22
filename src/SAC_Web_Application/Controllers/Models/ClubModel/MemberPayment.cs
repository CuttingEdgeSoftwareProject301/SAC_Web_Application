using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SAC_Web_Application.Models.ClubModel
{
    [Table("MemberPayments")]
    public class MemberPayment
    {
        public string PaymentID { get; set; }
        public Payment Payment { get; set; }

        public int MemberID { get; set; }
        public Members Member { get; set; }
    }
}
