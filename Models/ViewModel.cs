using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Anemette.Models
{
    public class ViewModel
    {
       
        public tblEvent Event { get; set; }
        public List<tblEvent> Events { get; set; }


        public tblNyhedsbrev Nyhedsbrevstilmelding { get; set; }
        public List<tblNyhedsbrev> Nyhedsbrevstilmeldte { get; set; }

      
        public tblProdukt Produkt { get; set; }
        public List<tblProdukt> Produkter { get; set; }

 

        public tblProduktKategori ProduktKategori { get; set; }
        public List<tblProduktKategori> ProduktKategorier { get; set; }



        public tblPerson Person { get; set; }
        public List<tblPerson> Personer { get; set; }

        




        public tblType Type { get; set; }
        public List<tblType> Typer { get; set; }






        public tblBillede Billede { get; set; }
        public List<tblBillede> Billeder { get; set; }











        public tblTilmelding DeltagerTilmelding { get; set; }
        public List<tblTilmelding> DeltagerTilmeldings { get; set; }



        //Til administrationspanel
        public tblAdmin Adminstrator { get; set; }

        public string Besked { get; set; }
      
        
    }
}