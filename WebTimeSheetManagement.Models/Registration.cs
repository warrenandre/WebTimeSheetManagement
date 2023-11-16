using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Security;

namespace WebTimeSheetManagement.Models
{
    [Table("Registration")]
    public class Registration
    {
        [Key]
        public int RegistrationID { get; set; }

        [Required(ErrorMessage = "Enter Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Mobileno Required")]
        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Wrong Mobileno")]
        public string Mobileno { get; set; }

        [Required(ErrorMessage = "EmailID Required")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        public string EmailID { get; set; }

        [MinLength(6, ErrorMessage = "Minimum Username must be 6 in charaters")]
        [Required(ErrorMessage = "Username Required")]
        public string Username { get; set; }

        [MembershipPassword(
    MinRequiredNonAlphanumericCharacters = 1,
    MinNonAlphanumericCharactersError = "Your password needs to contain at least one symbol (!, @, #, etc).",
    ErrorMessage = "Your password must be 8 characters long and contain at least one symbol (!, @, #, etc) and one number.",
    MinRequiredPasswordLength = 8,
    MinPasswordLengthError = "Password must be 8 characters long.",
    PasswordStrengthRegularExpression = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$"
)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Enter Valid Password")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Gender Required")]
        public string Gender { get; set; }
        public DateTime? Birthdate { get; set; }
        public DateTime? DateofJoining { get; set; }
        
        public int? RoleID { get; set; }

        [MaxLength(5, ErrorMessage = "Minimum Employee must be 5 in charaters")]
        public string  EmployeeID { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ForceChangePassword { get; set; }
    }
}
