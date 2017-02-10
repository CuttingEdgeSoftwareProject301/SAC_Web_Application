using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SAC_Web_Application.Models.ClubModel
{
    [Table("Member")]
    public class Members
    {
        [Key]
        public int MemberID { get; set; }

        public int Identifier { get; set; }

        public string Email { get; set; }
        [Column(TypeName = "date")]
        public DateTime DateRegistered { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateTime DOB { get; set; }
        [Required]
        public string Gender { get; set; }        
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string PostCode { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string TeamName { get; set; }
        [Required]
        public string CountyOfBirth { get; set; }
        public string Province { get; set; }
        public bool MembershipPaid { get; set; }
        //[Required]
        //[ForeignKey("Payments")]
        //public string PaymentID { get; set; }

        public List<MemberPayment> MemberPayments { get; set; }
    }
}
