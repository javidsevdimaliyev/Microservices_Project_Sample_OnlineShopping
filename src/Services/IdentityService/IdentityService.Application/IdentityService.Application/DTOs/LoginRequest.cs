using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.DTOs
{
    public class LoginRequest
    {
        /// <summary>
        /// Email or username
        /// </summary>
        [Required]
        public string UsernameOrEmail { get; set; }

        /// <summary>
        ///     password
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
