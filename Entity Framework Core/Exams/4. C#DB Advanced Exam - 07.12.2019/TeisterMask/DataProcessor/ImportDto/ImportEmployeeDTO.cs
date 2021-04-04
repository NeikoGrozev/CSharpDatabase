using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeisterMask.DataProcessor.ImportDto
{
    public class ImportEmployeeDTO
    {
        [Required]
        [StringLength(40, MinimumLength = 3)]
        //[RegularExpression(@"^[A-Za-z\d]$")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^[\d]{3}-[\d]{3}-[\d]{4}$")]
        public string Phone { get; set; }

        public int[] Tasks { get; set; }
    }
}
