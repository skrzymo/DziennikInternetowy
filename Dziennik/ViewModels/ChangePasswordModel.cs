using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Dziennik.ViewModels
{
    public class ChangePasswordModel
    {
        [Display(Name = "Stare hasło:")]
        [Required(ErrorMessage = "Podanie starego hasła jest wymagane")]
        [DataType(DataType.Password)]
        public string oldPassword { get; set; }

        [Display(Name = "Nowe hasło:")]
        [Required(ErrorMessage = "Podanie nowego hasła jest wymagane")]
        [DataType(DataType.Password)]
        public string newPassword { get; set; }

        [Display(Name = "Potwierdź nowe hasło")]
        [Required(ErrorMessage = "Potwierdzenie nowego hasła jest wymagane")]
        [Compare(otherProperty: "newPassword", ErrorMessage = "Hasło i jego potwierdzenie nie są zgodne")]
        [DataType(DataType.Password)]
        public string confirmNewPassword { get; set; }
    }
}