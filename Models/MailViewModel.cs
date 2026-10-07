using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Anemette.Models
{
   public class MailViewModel
   {
        [Required(ErrorMessage = "Du skal skrive noget")]
        [EmailAddress(ErrorMessage = "Det skal være en den mail")]
        public string MailAdressFrom { get; set; }
        public string MailAdressTo { get; set; }
        public string MailName { get; set; }
        public string MailSubject { get; set; }
        public string MailBody { get; set; }
        public string MailTelefon { get; set; }
        public Boolean CheckNewsLetter { get; set; }
        public string Message { get; set; }

    }

    
}





