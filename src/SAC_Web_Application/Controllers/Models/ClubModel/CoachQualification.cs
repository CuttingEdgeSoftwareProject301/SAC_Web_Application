using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAC_Web_Application.Models.ClubModel
{
    public class CoachQualification
    {
        public int CoachID { get; set; }
        public Coaches coaches { get; set; }
        
        public int QualID { get; set; }
        public Qualifications qualifications { get; set; }
    }
}
