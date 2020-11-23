using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cpsc_471_project.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cpsc_471_project.Models
{

	public class Certification
	{
		[Required]
		[ForeignKey("Resume")]
		[Key, Column(Order = 0)]
		public long ResumeId { get; set; }

		[Key, Column(Order = 1)]
		public string Name { get; set; }

		public string Source { get; set; }

		public Resume Resume { get; set; }



	}

	public class CertificationDTO
	{
		[Display(Name = "Resume Id")]
		public long ResumeId { get; set; }

		[Display(Name = "Certification Name")]
		public string Name { get; set; }

		[Display(Name = "Certification Source")]
		public string Source { get; set; }

	}

}