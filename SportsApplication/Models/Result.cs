using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Sports_Application.Models
{
    public class Result
    {
        public int Id { get; set; }
        [Required]
        public int TestId { get; set; }
        [Display(Name ="Name")]
        [Required]
        public string UserId { get; set; }
        [Display(Name = "Distance")]
        [Required]
        public int Data { get; set; }
    }
}
