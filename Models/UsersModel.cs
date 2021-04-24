using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace TryMoreAPI.Models
{
   

    public class UsersModel : BaseModel
    {
        public int UserID { get; set; }

        //[Required]
        //[Display(Name = "First Name")]
        public string FirstName { get; set; }

        //[Required]
        //[Display(Name = "Last Name")]user
        public string LastName { get; set; }

        public string UserName { get; set; }

        //[Required]
        //[Display(Name = "Password")]
        public string Password { get; set; }


        //[Required]
        //[Display(Name = "Email Id")]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        
        public string PostalCode { get; set; }

        public int Approve { get; set; }

        
        public int? Age { get; set; }

        // New Field

        public string AddrLine1 { get; set; }

        public string AddrLine2 { get; set; }
        
        public string City { get; set; }

        public string State { get; set; }
        
        public string Country { get; set; }

        public int Gender { get; set; }

        public int UserType { get; set; }
        public string ProfileImage { get; set; }
        //[Required(ErrorMessage = "Please select file.")]
        //[Display(Name = "Browse File")]

        public UserTokenViewModel UserTokenInfo { get; set; }

        public bool isFiles { get; set; }
        public string FileExt { get; set; }
    }



    public class UserTokenViewModel
    {
        public string access_token { set; get; }
        public string expires_in { set; get; }
        public string refresh_token { set; get; }
    }

    public class ChangePasswordViewModel1
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}