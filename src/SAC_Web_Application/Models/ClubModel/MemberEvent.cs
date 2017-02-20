using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SAC_Web_Application.Models.ClubModel
{
    [Table("MemberEvent")]
    public class MemberEvent
    {
        public int MemberID { get; set; }
        public Members member { get; set; }

        public int EventID { get; set; }
        public Events eventt { get; set; }
    }
}
