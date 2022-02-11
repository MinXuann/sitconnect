using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace sitconnect.Models
{
    public class CreditCard
    {
        [ForeignKey("FK_User")]
        [Key]
        public string UserId { get; set; }
        
        [ProtectedPersonalData]
        public string Number { get; set; }
        
        [ProtectedPersonalData]
        public string ExpiryMonth { get; set; }
        
        [ProtectedPersonalData]
        public string ExpiryYear { get; set; }
        
        [ProtectedPersonalData]
        public string CVV { get; set; }
    }
}