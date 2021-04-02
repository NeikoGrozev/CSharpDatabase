﻿namespace VaporStore.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using VaporStore.Data.Models.Enums;

    public class Card
    {
        public Card()
        {
            this.Purchases = new HashSet<Purchase>();
        }

        public int Id { get; set; }

        [Required]
        [RegularExpression(@"[\d]{4}\s[\d]{4}\s[\d]{4}\s[\d]{4}")]
        public string Number { get; set; }

        [Required]
        [RegularExpression(@"[\d]{3}")]
        public string Cvc { get; set; }

        [Required]
        public CardType Type { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }

        public virtual IEnumerable<Purchase> Purchases { get; set; }
    }
}
