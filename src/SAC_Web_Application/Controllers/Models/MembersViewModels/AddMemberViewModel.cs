using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using SAC_Web_Application.Models.ClubModel;

namespace SAC_Web_Application.Models.MembersViewModels
{
    // HELLO WORLD!
    public class AddMemberViewModel
    {
        [Required]
        [Display(Name = "Identifier")]
        public int Identifier { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        [DataType(DataType.Text, ErrorMessage = "Please enter a valid Name")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        [DataType(DataType.Text, ErrorMessage = "Please enter a valid Name")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Date, ErrorMessage = "Please enter a valid Date of birth")]
        [Display(Name = "Date Of Birth")]
        public DateTime DOB { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        public SelectList GenderList { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Address 1")]
        public string Address1 { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Address 2")]
        public string Address2 { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Post Code")]
        public string PostCode { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "County")]
        public string County { get; set; }
        public SelectList CountyList { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Province")]
        public string Province { get; set; }
        public SelectList ProvinceList { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression("^(08[2-9]\\d{7})", ErrorMessage = "This must be an Irish mobile number (08x) xxxxxxx")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }       

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Team Name")]
        public string TeamName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "County Of Birth")]
        public string CountyOfBirth { get; set; }
    }
}
