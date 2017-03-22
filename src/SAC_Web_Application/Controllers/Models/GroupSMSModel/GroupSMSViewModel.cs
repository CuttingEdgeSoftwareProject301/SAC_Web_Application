using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SAC_Web_Application.Models.GroupSMSModel
{
    public class GroupSMSViewModel
    {
        [Required, Display(Name = "Recipients")]
        public string SMSTo { get; set; }

        public SelectList EmailToList { get; set; }

        [Required, Display(Name = "Message")]
        [DataType(DataType.MultilineText)]
        public string SMSContent { get; set; }
    }
}
