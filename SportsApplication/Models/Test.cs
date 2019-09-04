using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sports_Application.Models
{
    public class Test
    {
        public int Id { get; set; }
        [Display(Name ="Test Type")]
        [Required]
        public string TestType { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string CoachId { get; set; }
        public int Count { get; set; }
    }
}
