using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace Anemette.Models
{
    public class SoegeViewModel
    {

        //ProductIndexViewModel needs to hold a combination of information that was previously passed to the view using ViewBag and also the model IEnumerable<BabyStore.Models.Product> (since this is the model currently specified at the top of the /Views/Products/Index.cshtml file).
        //Vi kommer til at bruge den her ViewModel de steder hvor vi ellers har skrevet  @model.Anemette.tblProdukt

        //The first property in the class is public IQueryable<Product> Products { get; set; }. This will be used instead of the model currently used in the view.
        //public IQueryable<tblProdukt> Produkts { get; set; }

        //the Products property is changed to the type IPagedList. Modify the ViewModels\ProductIndexViewModel.cs file to update the code highlighted here:
        public IPagedList<tblProdukt> Produkts { get; set; }

        //Denne erstatter ViewBag.Search
        //The second property, called public string Search { get; set; }, will replace ViewBag.Search currently set in the ProductsController class.
        public string Soegning { get; set; }

        //The third property, called public IEnumerable<CategoryWithCount> CatsWithCount { get; set; }, will hold all of the CategoryWithCount items to be used inside the select control in the view.
        public IEnumerable<KategoriTaeller> KategoriTaellers { get; set; }

        //The fourth property, Category, will be used as the name of the select control in the view.
        public string Kategori { get; set; }

        //We now need to add some user interface controls for sorting into the web site to allow users to choose how they want to sort. To demonstrate this, add a select list and populate it with values and text from a dictionary type.

        //First of all, add the following highlighted SortBy and Sorts properties to the ProductIndexViewModel class in the \ViewModels\ProductIndexViewModel.cs file:
        public string SorterEfter { get; set; }
        public Dictionary<string, string> SorteringsMuligheder { get; set; }


        //Finally, the property public IEnumerable<SelectListItem> CatFilterItems is used to return a list of the type SelectListItem, which will generate a value of the categoryName to be used as the value when the HTML form is submitted and the text displayed in the format of CatNameWithCount
        public IEnumerable<SelectListItem> KategoriFiltrering
        {
            get
            {
                var allCats = KategoriTaellers.Select(kategoritaeller => new SelectListItem
                {
                    Value = kategoritaeller.KategoriNavn,
                    Text = kategoritaeller.KategoriMedTaeller
                });

                return allCats;
            }
        }
        


        //CategoryWithCount is a simple class used to hold a category name and the number of products within that category.
        public class KategoriTaeller
        {

            //The ProductCount property holds the number of matching products in a category and CategoryName simply holds the name of the category. The CatNameWithCount property then returns both of these properties combined into a string. An example of this property is Clothes(2).
            public int ProduktAntal { get; set; }
            public string KategoriNavn { get; set; }
            public string KategoriMedTaeller
            {
                get
                {
                    return KategoriNavn + " (" + ProduktAntal.ToString() + ")";
                }
            }
        }
    }
}


