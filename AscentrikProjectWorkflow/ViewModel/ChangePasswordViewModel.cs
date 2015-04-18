using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AscentrikProjectWorkflow.ViewModel
{
    public class ChangePasswordViewModel
    {
        public int UserId { get; set; }
        public string EmailAddress { get; set; }
        [Display(Name="Old Password")]
        [Required]
        [MinLength(6,ErrorMessage="Please enter atleast 6 characters.")]
        public string OldPassword { get; set; }
        [Display(Name = "New Password")]
        [Required]
        [MinLength(6, ErrorMessage = "Please enter atleast 6 characters.")]
        public string NewPassword { get; set; }
        [Display(Name = "Confirm Password")]
        [Required]
        [MinLength(6, ErrorMessage = "Please enter atleast 6 characters.")]
        [Compare("NewPassword", ErrorMessage="Confirm password should match with New password")]
        public string ConfirmPassword { get; set; }
    }
}