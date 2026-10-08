using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
//Husk IO til uploads
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
//Vi bruger den mappe der hedder Models. Heri ligger også ViewModels. Hvis de havde ligget i en mappe for sig, skulle vi have skrevet "using Anemette.ViewModels";
using Anemette.Filters;
using Anemette.Models;
using PagedList;
using static Anemette.Models.SoegeViewModel;

namespace Anemette.Controllers
{
    public class ProduktController : Controller
    {
        private DatabaseEntities db = new DatabaseEntities();



        // GET: Produkt
        //To demonstrate the new method in action, start the web site without debugging and navigate to the Product Index page. Now append ?category=clothes onto the end of the URL. The list of products should now be filtered

        public ActionResult Index(string category, string search, string SorterEfter, int? side)
        {

            //instantiate a new view model
            SoegeViewModel viewModel = new SoegeViewModel();

            

            //select the products
            //There are performance implications to choosing which method of loading to use. Eager loading results in one round trip to the database, but on occasion may result in complex join statements that are slow to process. However, lazy loading results in several round trips to the database. Here eager loading is used since the join statement will be relatively simple and we want to load the related categories in order to search over them.

            //The products variable is filtered using the Where operator to match products when the Name property of the product's Category property matches the category parameter passed into the method. This may seem a little like overkill and you may be wondering why I didn't just use the CategoryID property and pass in a number rather than a string.The answer to this lies in the fact that using a category name is much more meaningful in a URL when using routing.We will cover this later in the book.

            //This is an excellent example of why navigational properties are so useful and powerful. By using a navigational property in my Product class, I am able to search two related entities using minimal code.If I wanted to match products by category name, but did not use navigational properties, I would have to enter the realm of loading category entities via the ProductsController which by convention is only meant to manage products.

            var products = db.tblProdukts.Include(p => p.tblProduktKategori);

            #region Søgeformular
            //First, a search parameter is added to the method and then if search is not null or empty, the products query is modified to filter on the value of search using this code:
            //perform the search and save the search string to the viewModel
            if (!String.IsNullOrEmpty(search))
            {
                //Translated into plain English, this code says "find the products where either the product name field contains search, the product description contains search, or the product's category name contains search". The code again makes use of a lambda expression but this expression is more complex and uses the logical OR operator ||. Note that there is still only one operator required on the left of the => lambda operator despite there being multiple alternatives in the code statement to the right of =>. When the query is run against the database, the Contains method is translated to SQL LIKE and is case-insensitive.
                products = products.Where(p => p.Navn.Contains(search) ||
                p.Beskrivelse.Contains(search) ||
                p.tblProduktKategori.Kategori.Contains(search));
                //Gem søgningen i Viewbag https://2masteritezproxy.skillport.com/skillportfe/assetNonSSOLaunch.action?courseName=_ss_chapter:117555-168822317&courseType=7 
                //The search is stored in the ViewBag to allow it to be reused when a user clicks on the category filter. If it weren't stored, the search term would be discarded and the products would not be filtered correctly. 
                //The code viewModel.Search = search; assigns the search variable to the viewModel instead of to ViewBag.
                viewModel.Soegning = search;
            }
            #endregion


            ////The code var categories = products.OrderBy(p => p.Category.Name).Select(p => p.Category. Name).Distinct(); then generates a distinct list of categories ordered alphabetically. The list of categories is not exhaustive; it only contains categories from the products that have been filtered by the search.
            //var categories = products.OrderBy(p => p.tblProduktKategori.Kategori).Select(p =>
            // p.tblProduktKategori.Kategori).Distinct();


            #region Sortering på Kategori
            //The third code change is a LINQ statement that populates the CatsWithCount property of viewModel with a list of CategoryWithCount objects. In this example, I used a different form of LINQ than what I used previously due to the complexity of the query.I used a form of LINQ known as query syntax to make the query easier to read.
            //The statement works by grouping products by category name, where the category ID is not null, using this code:
            //group search results into categories and count how many items in each category
            viewModel.KategoriTaellers = from matchingProducts in products
                                      where
                                      matchingProducts.FK_ProductCategoryID != null
                                      group matchingProducts by
                                      matchingProducts.tblProduktKategori.Kategori into
                                      catGroup
                                      //For each group, the category name and the number of products are then assigned to a CategoryWithCount object:
                                      select new KategoriTaeller()
                                      {
                                          KategoriNavn = catGroup.Key,
                                          ProduktAntal = catGroup.Count()
                                      };

            if (!String.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.tblProduktKategori.Kategori == category);
                viewModel.Kategori = category;
            }
            #endregion 

            #region Sortering På Pris
            //sort the results
            //This new code uses the Entity Framework OrderBy and OrderByDescending methods to sort products by ascending and descending price. Run the application without debugging and manually change the URL to test that sorting works as expected, by using the Products?sortBy=price_lowest and Products?sortBy=price_highest URLs. The products should reorder with the lowest priced item at the top and the highest priced item at the top, respectively. Figure 5-1 shows the products being sorted with the highest price first.
            switch (SorterEfter)
            {
                case "laveste_pris":
                    products = products.OrderBy(p => p.Pris);
                    break;
                case "hojeste_pris":
                    products = products.OrderByDescending(p => p.Pris);
                    break;
                default:
                    //We now need to modify the Index method of the ProductsController class so that it returns Products as a PagedList (achieved by using the ToPagedList() method). A default sort order also needs to be set in order to use PagedList. First of all, add the code using PagedList; to the using statements at the top of the file. Then modify the Controllers\ProductsController.cs file, as highlighted, in order to use the new PagedList package.
                    //The code products = products.OrderBy(p => p.Name); is then used to set a default order of products because PagedList requires the list it receives to be sorted.
                    products = products.OrderBy(p => p.Navn);
                    break;
            }


            //The SortBy property will be used as the name of the select element in the view and the Sorts property will be used to hold the data to populate the select element.
            //Now we need to populate the Sorts property from the ProductController class. Modify the \Controllers\ProductsController.cs file to add the following line of code to the end of the Index method prior to returning the View:
            viewModel.SorteringsMuligheder = new Dictionary<string, string>
            {
                {"Pris fra lav til høj", "laveste_pris" },
                {"Pris fra høj til lav", "hojeste_pris" }
            };

            #endregion



            //The final code change assigns the products variable to the Products property of the viewModel instead of passing it to the view and then instead passes the viewModel to the view as follows:
            //viewModel.Produkts = products;

            //Next, we specify the number of items to appear on each page by adding a constant using the line of code const int PageItems = 3;. We then declare an integer variable int currentPage = (page ?? 1); to hold the current page number and take the value of the page parameter, or 1, if the page variable is null.
            //Vi har defineret ProdukterPerSide i Konstanter.cs som vi refererer til istedet
            //const int PageItems = 3;

            int currentPage = (side ?? 1);
            //The products property of the view model is then assigned a PagedList of products specifying the current page and the number of items per page using the code viewModel.Products = products. ToPagedList(currentPage, PageItems);.
            viewModel.Produkts = products.ToPagedList(currentPage, Konstanter.ProdukterPerSide);
            //Finally, the sortBy value is now saved to the view model so that the sort order of the products list is preserved when moving from one page to another by the code: viewModel.SortBy = sortBy;.
            viewModel.SorterEfter = SorterEfter;


            //// a new SelectList is created from the categories variable and stored in the ViewBag ready for use in the view.
            //ViewBag.Kategori = new SelectList(categories);



            //A common error often made by programmers new to using Entity Framework is using ToList() in the wrong place. During a method, LINQ is often used for building queries and that is precisely what is does; it simply builds up a query, it does not execute the query! The query is only executed when ToList() is called. Novice programmers often use ToList() at the beginning of their method. The consequences of this are that more records (usually all) will be retrieved from the database than are required, often with an adverse effect on performance. All these records are then held in memory and processed as an in-memory list, which is usually undesirable and can slow the web site down dramatically. Alternatively, do not even call ToList() and the query will only be executed when the view loads. This topic is known as deferred execution due to the fact that the execution of the query is deferred until after ToList() is called.
            return View(viewModel);

            //    //This code uses LINQ method syntax to specify which column to order by. A lambda expression is used to specify the Name column. This code then returns an ordered list of categories to the view for display. LINQ stands for Language-Integrated Query and it is a query language built into the .NET framework. Using LINQ method syntax means that queries are built using a dot notation to quickly chain methods together. An alternative to method syntax is query syntax and I give an example of this in Chapter 3 when writing a more complex query. Method syntax is more SQL-like in its appearance and can be easier to understand for more complex queries; however, for shorter queries it can appear more long-winded.
            //    //Lambda expressions are anonymous functions that can be used to create delegates. In simple terms, they enable you to create an expression where the value on the left side of the lambda operator (=>) is the input parameter and the value on the right is the expression to be evaluated and returned.Considering the lambda expression we have entered above, it takes a category as an input and returns the Name property. Therefore, in plain English, it says to order by the category's Name property.
            //    //I don't cover LINQ or lambda expressions in detail in this book. I suggest if you want to learn more about them that you read the excellent Pro C# books by Andrew Troelsen.
            //    return View(tblProdukts.OrderBy(c => c.Navn).ToList());
        }










        // GET: Produkt/Details/5
        public ActionResult Details(int? id)
        {

            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Specifikt produkt på ID (Valgt fra View)
            tblProdukt tblProdukt = db.tblProdukts.Find(id);
            
            if (tblProdukt == null)
            {
                return HttpNotFound();
            }


            return View(tblProdukt);
        }

        // GET: ProduktKategori/Create
        [AdminAdgang]
        public ActionResult OpretProdukt()
        {
            //Tilknyt produkt til kategori med dropdownmenu i View
            ViewBag.FK_ProductCategoryID = new SelectList(db.tblProduktKategoris, "ID", "Kategori");
            return View();
        }




        // POST: Produkt/Create
        [HttpPost]
        //[ValidateAntiForgeryToken] ensures that the token passed by the HTML form, thus validating the request. The purpose of this is to ensure that the request actually came from the form it is expected to come from in order to prevent cross-site request forgeries. In simple terms, a cross-site request forgery is a request from a form on another web site to your web site with malicious intentions.
        [ValidateAntiForgeryToken]
        //The parameters ([Bind(Include = “ID,Name”)] Category category) tell the method to include only the ID and the Name properties when adding a new category. The Bind attribute is used to protect against overposting attacks by creating a list of safe properties to update; however, as we will discuss later, it does not work as expected and so it is safer to use a different method for editing or creating where some values may be blank. As an example of overposting, consider a scenario where the price is submitted as part of the request when a user submits an order for a product. An overposting attack would attempt to alter this price data by changing the submitted request data in an attempt to buy the product cheaper.
        [AdminAdgang]
        public ActionResult OpretProdukt([Bind(Include = "ID,Navn,Beskrivelse,Pris,Antal,FK_ProductCategoryID")] IEnumerable<HttpPostedFileBase> SamlingAfBilleder, tblProdukt NytProdukt)
        {
            if (ModelState.IsValid)
            {

                //Denne linje udputter SQL til Output-vinduet her i VS
                db.Database.Log = sql => Trace.WriteLine(sql); 
                #region FotoUpload



                foreach (var BilledeAfProdukt in SamlingAfBilleder)
                {


                    //HUSK HUSK!!! Tilføj atributten i formular-helper-start-opmærkningen using (Html.BeginForm("Unik", "Home" , FormMethod.Post, new { enctype = "multipart/form-data" }))
                    //Foto-upload. BilledeAfProdukt skal være det samme som <input type="file" name="BilledeAfProdukt" />
                    if (BilledeAfProdukt != null && BilledeAfProdukt.ContentLength > 0)
                    {
                       
                        List<string> tilladteFilformater = new List<string>() { "image/png", "image/jpeg", "image/jpg", "image/gif" };
                        string Filformat = BilledeAfProdukt.ContentType.ToLower();
                        bool erTilladtFilformat = tilladteFilformater.Contains(Filformat);

                        if (erTilladtFilformat)
                        {
                        
                            //Filnavn på den valgte fil sammen med en random string og filtypen
                            string ImageFileName = Path.GetFileName(BilledeAfProdukt.FileName) + "-" + Guid.NewGuid() + Path.GetExtension(BilledeAfProdukt.FileName);

                            //Image orjres = Image.FromStream(BilledeAfProdukt.InputStream);

                            //Læg filnavn i denne mappe
                            //Hvis mappen ikke findes får du en fejl, så opret den i mappelisten. Men der er et sted hvor man kan oprette en mappe med IO-tools??? Find det
                    
                            string BilledemappeSti = Path.Combine(Server.MapPath("~/Uploads"), ImageFileName);
                            string ThumbnailSti = Path.Combine(Server.MapPath("~/Uploads/Thumbnails"), ImageFileName);
                            if (!Directory.Exists(Server.MapPath("~/Uploads")))
                            {
                                Directory.CreateDirectory(Server.MapPath("~/Uploads"));
                            }

                            #region Ændr billededimensioner
                            WebImage img = new WebImage(BilledeAfProdukt.InputStream);
                            
                            //Indkommentér denne betingelse hvis der skal være begrænsning på filstørrelsen
                            //if (img.Width > 190)
                            //{
                            //    img.Resize(190, img.Height);
                            //}
                            img.Save(BilledemappeSti);
                            if (img.Width > 100)
                            {
                                img.Resize(100, img.Height);
                            }
                            img.Save(ThumbnailSti);
                            #endregion



                            //Gem filen i den mappe
                            //BilledeAfProdukt.SaveAs(Mappestien);

                            //Tildel modellen samme filnavn
                            tblBillede NytBillede = new tblBillede();
                            NytBillede.BilledeFil = ImageFileName;
                            NytBillede.FK_Produkt = NytProdukt.ID;
                            db.tblBilledes.Add(NytBillede);                    
                        }
                        else //hvis filformaterne ikke er overholdt
                        {
                                ViewBag.Besked = "Du kan kun uploade billedefiler";
                        }
                  
                    }
                    else
                    {
                        ViewBag.Besked = "Du har ikke valgt en fil endnu";
                    }
                  
                }
                #endregion
                //Tilføj produktet til databasen
                db.tblProdukts.Add(NytProdukt);

                //If you are building a system responsible for bulk upload and using Entity Framework, then I do not recommend calling SaveChanges() for each individual record. In the multiple file upload code, I have included the call to SaveChanges() for each item in a loop in order to demonstrate some of the features of Entity Framework and also because the code can deal with only 10 records at a time. In a system that's making bulk uploads, it is much faster to call SaveChanges() once, after all the items have been added to the DbContext.
                db.SaveChanges();

                //Hvis man vil have egne fejlmeddelelser
                //This code uses a try catch statement to attempt to save the database changes or catch an exception of the type DBUpdateException. It then checks the InnerException property of the InnerException property of the exception to check if it is exception number 2601 (this is the SQL Exception number for trying to insert a duplicate key when a unique index is in place on a table). If the exception is number 2601, then an error is added to the ModelState to notify the user that the file already exists. If the exception is a different number, a more generic error is thrown. Finally, if there was an exception, the Update view is returned to the user to display the error message.
                //try
                //{
                //    db.SaveChanges();
                //}
                //catch (DbUpdateException ex)
                //{
                //    SqlException innerException = ex.InnerException.InnerException as SqlException;
                //    if (innerException != null && innerException.Number == 2601)
                //    {
                //        ModelState.AddModelError("FileName", "The file " + Billedet.FileName + " already exists in the system. Please delete it and try again if you wish to re-add it");
                //    }
                //    else
                //    {
                //        ModelState.AddModelError("FileName", "Sorry an error has occurred saving to the database, please try again");
                //    }
                //return View();
                //}

                return RedirectToAction("Index");
            }

            //Det valgte punkt på dropdown-menuen bibeholdes, hvis noget går galt
            //This code assigns an item to a ViewBag property named CategoryID.The item is a SelectList object consisting of all the categories in the database, with each entry in the list using the Name property as the text and the ID field as the value.The optional fourth parameter determines the preselected item in the select list.As an example, if the fourth argument product.CategoryID is set to 2, then the Toys category will be preselected in the drop-down list when it appears in the view.
            ViewBag.FK_ProductCategoryID = new SelectList(db.tblProduktKategoris, "ID", "Kategori", NytProdukt.FK_ProductCategoryID);
            return View(NytProdukt);
        }





        [AdminAdgang]
        public ActionResult OpretKategori()
        {
            
            return View();
        }



        // POST: tblProduktKategoris/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAdgang]
        public ActionResult OpretKategori([Bind(Include = "ID,Kategori")] tblProduktKategori tblProduktKategori)
        {
            if (ModelState.IsValid)
            {
                db.tblProduktKategoris.Add(tblProduktKategori);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblProduktKategori);
        }














        // GET: Produkt/Edit/5
        [AdminAdgang]
        public ActionResult EditProdukt(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Find på ID ligesom ved Detail
            tblProdukt tblProdukt = db.tblProdukts.Find(id);
            if (tblProdukt == null)
            {
                return HttpNotFound();
            }
            //Det valgte punkt på dropdown-menuen
            ViewBag.FK_ProductCategoryID = new SelectList(db.tblProduktKategoris, "ID", "Kategori", tblProdukt.FK_ProductCategoryID);
            return View(tblProdukt);
        }

        // POST: Produkt/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAdgang]
        public ActionResult EditProdukt([Bind(Include = "ID,Navn,Beskrivelse,Pris,Antal,FK_ProductCategoryID")] IEnumerable<HttpPostedFileBase> SamlingAfBilleder, tblProdukt EditeretProdukt)
        {
            if (ModelState.IsValid)
            {
                #region FotoUpload




                foreach (var BilledeAfProdukt in SamlingAfBilleder)
                {



                    //HUSK HUSK!!! Tilføj atributten i formular-helper-start-opmærkningen using (Html.BeginForm("Unik", "Home" , FormMethod.Post, new { enctype = "multipart/form-data" }))
                    //Foto-upload. BilledeAfProdukt skal være det samme som <input type="file" name="BilledeAfProdukt" />
                    if (BilledeAfProdukt != null && BilledeAfProdukt.ContentLength > 0)
                    {

                        List<string> tilladteFilformater = new List<string>() { "image/png", "image/jpeg", "image/jpg", "image/gif" };
                        string Filformat = BilledeAfProdukt.ContentType.ToLower();
                        bool erTilladtFilformat = tilladteFilformater.Contains(Filformat);

                        if (erTilladtFilformat)
                        {
                            //Filnavn på den valgte fil sammen med en random string og filtypen
                            string ImageFileName = Path.GetFileName(BilledeAfProdukt.FileName) + "-" + Guid.NewGuid() + Path.GetExtension(BilledeAfProdukt.FileName);

                            //Image orjres = Image.FromStream(BilledeAfProdukt.InputStream);

                            //Læg filnavn i denne mappe
                            //Hvis mappen ikke findes får du en fejl, så opret den i mappelisten. Men der er et sted hvor man kan oprette en mappe med IO-tools??? Find det

                            string FolderPath = Path.Combine(Server.MapPath("~/Uploads"), ImageFileName);
                            if (!Directory.Exists(Server.MapPath("~/Uploads")))
                            {
                                Directory.CreateDirectory(Server.MapPath("~/Uploads"));
                            }
                            //Gem filen i den mappe
                            BilledeAfProdukt.SaveAs(FolderPath);
                            //Tildel modellen samme filnavn
                            tblBillede NytBillede = new tblBillede();
                            NytBillede.BilledeFil = ImageFileName;
                            NytBillede.FK_Produkt = EditeretProdukt.ID;
                            db.tblBilledes.Add(NytBillede);
                        }
                        else //hvis filformaterne ikke er overholdt
                        {
                            ViewBag.Besked = "Du kan kun uploade billedefiler";
                        }

                    }
                    else
                    {
                        ViewBag.Besked = "Du har ikke valgt en fil endnu";
                    }

                }
                #endregion


                #region Valg af eksisterende billeder som ikke er knyttet til et produkt
                var AlleAndreBilleder = db.tblBilledes.Where(billeder => billeder.FK_Produkt == null).ToList();

                //Tilføj billederne til en selectlist hvor man kan sætte flueben ved de billeder man gerne vil have, og så gemmes FK_Produkt == Model.ID på hvert af disse
                #endregion

                //Denne linje er den eneste der er forandret fra "create-actionresultet" som hedder: db.tblProdukts.Add(EditeretProdukt);
                //The POST version of the Edit method is very similar to the POST version of the Create method. It contains an extra line of code to check that the entity has been modified before attempting to save it to the database and, if it's successful, the Index view is returned or else the Edit view is redisplayed:
                db.Entry(EditeretProdukt).State = EntityState.Modified;    
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //Det valgte punkt på dropdown-menuen bibeholdes, hvis noget går galt
            ViewBag.FK_ProductCategoryID = new SelectList(db.tblProduktKategoris, "ID", "Kategori", EditeretProdukt.FK_ProductCategoryID);
            return View(EditeretProdukt);
        }











        // GET: Produkt/Delete/5
        
        [AdminAdgang]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblProdukt tblProdukt = db.tblProdukts.Find(id);
            if (tblProdukt == null)
            {
                return HttpNotFound();
            }
            return View(tblProdukt);
        }

        // POST: Produkt/Delete/5
        //This auto-generated Delete method does not work correctly due to the fact that the product entity contains a foreign key referencing the category entity. See Chapter 4 for how to correct this issue.
        //There are several reasons why ASP.NET takes this approach to disallow a GET request to update the database and several comments and debates about the different reasons about the security of doing so; however, one of the key reasons for not doing it is that a search engine spider will crawl public hyperlinks in your web site and potentially be able to delete all the records if there is an unauthenticated link to delete records. Later we will add security to editing categories so that this becomes a moot point.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AdminAdgang]
        public ActionResult DeleteConfirmed(int id)
        {

           tblProdukt tblProdukt = db.tblProdukts.Find(id);
           

            //A referential integrity error occurs when trying to delete a category
            //This error occurs because the database column CategoryID is used as a foreign key in the Products table and currently there is no modification of this table when a category is deleted.This means that a product will be left with a foreign key field that contains an ID that no longer refers to a record in the Category table; this causes the error.
            //To fix this issue, the code created by the scaffolding process needs to be updated so that it sets the foreign key of all the affected products to null.Update the HttpPost version of the Delete method in the file \Controllers\CategoriesController.cs with the following changes highlighted in bold:
            //OBS! Enten kan man tillade ON DELETE CASCADE eller fjerne FK_id fra alle produkter. Nedenstående fjerner produktID fra alle produkter som har det FK-id
            //This code adds a simple foreach loop using the products navigational property of the category entity to set the CategoryID of each product to null.When you now try to delete Test Category, it will be deleted without an error and the CategoryID column of Test Product will be set to null in the database.
            //foreach (var p in tblProdukt.tblProduktKategori.tblProdukts)
            //{
            //    p.FK_ProductCategoryID = null;
            //}

            if (tblProdukt == null)
            {
                return HttpNotFound();
            }

            //Fjern alle billed-rækker til produktet (et produkt kan have ingen eller flere billeder)
            //Slet billederne fysisk
            //System.IO.File.Delete(Request.MapPath(Konstanter.BilledemappeSti + billedet.BilledeFil));
            //System.IO.File.Delete(Request.MapPath(Konstanter.ThumbnailSti + billedet.BilledeFil));
            db.tblBilledes.RemoveRange(db.tblBilledes.Where(b => b.FK_Produkt == tblProdukt.ID));
            db.tblProdukts.Remove(tblProdukt);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
