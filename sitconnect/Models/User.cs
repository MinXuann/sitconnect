using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace sitconnect.Models
{
    public class User: IdentityUser
    {   
        [PersonalData]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string ProfilePic { get; set; }
        
        [ProtectedPersonalData]
        public string CreditCardNo { get; set; }
        public DateTime CreditCardExpiry { get; set; }
        public string CVV { get; set; }
    }
}