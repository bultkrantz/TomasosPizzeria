using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TomasosPizzeria.Models
{
    public partial class Kund
    {
        public Kund()
        {
            Bestallning = new HashSet<Bestallning>();
        }

        public int KundId { get; set; }

        [Display(Name = "Fullständigt namn")]
        [Required(ErrorMessage = "Du måste ange ett namn")]
        public string Namn { get; set; }

        [Required(ErrorMessage = "Du måste ange en gatuadress")]
        public string Gatuadress { get; set; }

        [Display(Name = "Postnummer")]
        [Required(ErrorMessage = "Du måste ange ett postnummer")]
        public string Postnr { get; set; }

        [Required(ErrorMessage = "Du måste ange en postort")]
        public string Postort { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string Telefon { get; set; }

        [Display(Name = "Användarnamn")]
        [Required(ErrorMessage = "Du måste ange ett användarnamn")]
        public string AnvandarNamn { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Lösenord")]
        [Required(ErrorMessage = "Du måste ange ett lösenord")]
        public string Losenord { get; set; }

        public virtual ICollection<Bestallning> Bestallning { get; set; }
    }
}
