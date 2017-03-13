using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SAC_Web_Application.Models.ClubModel
{
    public class Coaches
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CoachID { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [RegularExpression("^(08[2-9]\\d{7})", ErrorMessage = "Invalid phone number")]
        [Display(Name = "Phone Number")]
        public string ContactNumber { get; set; }
        [Required]
        [Display(Name = "Garda Vetting Expiry")]
        [DataType(DataType.Date, ErrorMessage = "Please enter a valid Date")]
        public DateTime GardaVetExpDate { get; set; }
        public string Availability { get; set; }


        public List<CoachQualification> coachQualifications { get; set; }

    }
}
