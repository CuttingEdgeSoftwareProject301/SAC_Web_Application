﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SAC_Web_Application.Models.GroupEmailModel
{
    public class GroupEmailViewModel
    {
        [Required, Display(Name = "Recipients")]
        public string EmailTo { get; set; }

        public SelectList EmailToList { get; set; }

        [Required, Display(Name = "Email Subject")]
        public string EmailTitle { get; set; }

        [Required, Display(Name = "Message")]
        [DataType(DataType.MultilineText)]
        public string EmailContent { get; set; }
    }
}
