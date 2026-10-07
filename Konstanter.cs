using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Anemette
{
    //We are going to be referring to these folders in various files throughout the project; therefore, we need a way to store the path of each folder so that we can refer to them easily. To do this, we're going to add a new static class named Constants in the base of the project and add some constants to it for both these file paths. To do this, right-click on the BabyStore project in Solution Explorer and choose Add ➤ Class. Create a new class named Constants. Update the new class with the following code:
    //Det er belejeligt at have ting i projektet som ikke forandrer sig, til at stå her
    //The class is declared as static so it does not need to be instantiated prior to being used.
    public static class Konstanter
    {
        
        public const string BilledemappeSti = "~/Uploads/";
        public const string ThumbnailSti = "~/Uploads/Thumbnails/";
        //Now that we have constants defined globally, add a constant for PageItems (currently defined in the ProductsController class) as highlighted:
        public const int ProdukterPerSide = 10;
    }
}

