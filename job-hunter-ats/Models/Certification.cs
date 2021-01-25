using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using job_hunter_ats.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace job_hunter_ats.Models
{

	public class Certification
	{
		[Required]
		[ForeignKey("Resume")]
		[Key, Column(Order = 0)]
		public long ResumeId { get; set; }
		public Resume Resume { get; set; } // for referential integrity

		[Required]
		[Key, Column(Order = 1)]
		public long Order { get; set; }
		
		[Required]
		public string Name { get; set; }

		public string Source { get; set; }
	}

	public class CertificationDTO
	{
		[Display(Name = "Resume Id")]
		public long ResumeId { get; set; }


		[Display(Name = "Order")]
		public long Order { get; set; }

		[Display(Name = "Certification Name")]
		public string Name { get; set; }

		[Display(Name = "Certification Source")]
		public string Source { get; set; }

	}

}