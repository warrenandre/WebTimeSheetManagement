using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace WebTimeSheetManagement.Models
{
    [NotMapped]
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Enter Old Password")]
        [MinLength(7, ErrorMessage = "Minimum Password must be 7 in charaters")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Enter New Password")]
        [MembershipPassword(
    MinRequiredNonAlphanumericCharacters = 1,
    MinNonAlphanumericCharactersError = "Your password needs to contain at least one symbol (!, @, #, etc) .",
    ErrorMessage = "Your password must be 8 characters long and contain at least one symbol (!, @, #, etc) and one number..",
    MinRequiredPasswordLength = 8,
    MinPasswordLengthError = "Password must be 8 characters long.",
    PasswordStrengthRegularExpression = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$"
)]
        public string NewPassword { get; set; }
    }
}
