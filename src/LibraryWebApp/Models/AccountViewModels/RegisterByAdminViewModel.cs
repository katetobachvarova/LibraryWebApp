using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryWebApp.Models.AccountViewModels
{
    public class RegisterByAdminViewModel : RegisterViewModel
    {
        [Required]
        [Display(Name = "Confirmed")]
        public bool Confirmed { get; set; }

        [Required]
        [Display(Name = "UserRole")]
        public String Role { get; set; }

        public string Id { get; set; }
    }
}
