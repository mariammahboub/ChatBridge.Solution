using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Username is required")]
        public string userName { get; set; }
        [Required(ErrorMessage = "Password is required")]

        public string password { get; set; }
    }
}
