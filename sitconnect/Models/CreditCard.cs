using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sitconnect.Models
{
    public class CreditCard
    {
        public string CreditCardId { get; set; }
        [ForeignKey("FK_User")]
        public string UserId { get; set; }
        public string Number { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string CVV { get; set; }
    }
}