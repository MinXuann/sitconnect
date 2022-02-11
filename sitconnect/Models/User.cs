using System;
using Microsoft.AspNetCore.Identity;

namespace sitconnect.Models
{
    public class User: IdentityUser
    {   
        [PersonalData]
        public string FirstName { get; set; }
        
        [PersonalData]
        public string LastName { get; set; }
        
        [PersonalData]
        public DateTime DateOfBirth { get; set; }
        
        [PersonalData]
        public byte[] ProfilePic { get; set; }
    }
}