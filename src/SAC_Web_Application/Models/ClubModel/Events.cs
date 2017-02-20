using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SAC_Web_Application.Models.ClubModel
{
    public class Events
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventID { get; set; }
        [Display(Name ="Event Name")]
        public string EventTitle { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }

        public List<MemberEvent> MemberEvents { get; set; }
    }
}
