using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SAC_Web_Application.Models.ClubModel
{
    public class Subscription
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubID { get; set; }
        [Display(Name ="Membership Type")]
        public string Item { get; set; }
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public decimal Cost { get; set; }
    }
}
