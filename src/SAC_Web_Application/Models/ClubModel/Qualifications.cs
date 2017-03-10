using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SAC_Web_Application.Models.ClubModel
{
    public class Qualifications
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QualID { get; set; }
        [Required]
        [Display(Name = "Qualification Name")]
        public string QualName { get; set; }

        public List<CoachQualification> coachQualifications { get; set; }
    }
}
