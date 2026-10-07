//For at tilgå andre klasser i dette projekt skal namespacet for projektet også inkluderes
using Anemette;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
//To ensure that this code compiles, make sure that you add this using statement to the top of the file: using System.Web.Helpers;.
using System.Web.Helpers;

public class IOTools
{
    /// <summary>
    /// This new method returns a Boolean and takes an input parameter named file of the type HttpPostedFileBase. The method obtains the extension of the file and checks to see if the file is of the allowed extension types (GIF, PNG, JPEG, and JPG). It also checks if it is between 0 bytes and 2MB in size. If it is, then the method returns true (otherwise, it returns false). One point to note is that rather than loop through the allowedFileTypes array, we used the LINQ contains operator to shorten the amount of code needed.
    /// </summary>
    /// <param name="file"></param>
    /// <returns>en sand falsk om filen er af filtypen og indenfor bytelængden</returns>
    public static bool ValidateFile(HttpPostedFileBase file)
    {
        //Filformater tilladte
        string fileExtension = System.IO.Path.GetExtension(file.FileName).ToLower();
        
        //Eller:
        //string Filformat = file.ContentType.ToLower();
        
        
        List<string> tilladteFilformater = new List<string>() { ".png", ".jpeg", ".jpg", ".gif" };
        //Eller:
        //string[] allowedFileTypes = { ".gif", ".png", ".jpeg", ".jpg" };

        //Filstørrelse tilladte
        if ((file.ContentLength > 0 && file.ContentLength < 2097152) && tilladteFilformater.Contains(fileExtension))
        {
            return true;
        }
        return false;
    }

    public static string GivUniktNavn(HttpPostedFileBase file)
    {
        //Læg Filnavn på den valgte fil sammen med en random string og filtypen
        string BilledeFilNavnPlusRandom = Path.GetFileName(file.FileName) + "-" + System.Guid.NewGuid() + Path.GetExtension(file.FileName);


        return BilledeFilNavnPlusRandom;
    }

    


    /// <summary>
    /// The SaveFileToDisk() method takes an input parameter file, again of the type HttpPostedFileBase. Then it uses the WebImage class to resize the image if the width is greater than 190 pixels and save it to the ProductImages directory. It then resizes the image down to 100 pixels in width if needed and saves it to the thumbnails directory.
    /// </summary>
    /// <param name="file"></param>
    public static void SaveFileToDisk(HttpPostedFileBase file, string uniktnavn)
    {
        //Fat at det er et billede
        WebImage img = new WebImage(file.InputStream);
       
        

        //Hvis det er for stort så ændr det
        if (img.Width > 190)
        {
            img.Resize(190, img.Height);
        }
        //Opret mappesti hvis den ikke er der
        DirEx(Konstanter.BilledemappeSti);


        //Gem det store billede i mappen med billeder (Defineret specifikt for dette projekt i Konstanter.cs)
        //Uniktnavn kommer fra GivUniktNavn og Kombinerer filens navn fra filsystemet, et unikt GUID og filformatet
        img.Save(Konstanter.BilledemappeSti + uniktnavn);
        //Hvis billedet er stort nok til at trænge til en thumbnail så ændr det
        if (img.Width > 100)
        {
            img.Resize(100, img.Height);
        }
        //Opret mappesti hvis den ikke er der
        DirEx(Konstanter.ThumbnailSti);
        //Gem thumbnail i mappen med thumbnails
        img.Save(Konstanter.ThumbnailSti + uniktnavn);
    }


  


    public static List<FileInfo> DirInfo(string MyFiles)
    {
        DirEx(MyFiles);

        //Hvilken mappe ligger billederne i
        DirectoryInfo MyDir = new DirectoryInfo(HttpContext.Current.Server.MapPath(MyFiles));

        //Lav en liste med filer
        List<FileInfo> MyFileList = new List<FileInfo>();

        foreach (var Files in MyDir.GetFiles())
        {
            //Læg disse filer til listen
            MyFileList.Add(Files);
        }

        return MyFileList;
    }


    public static string FileUplader(string MyPath, string FileÍd, HttpPostedFileBase MyFile, string[] FileEx)
    {
        string Msg = "";

        //Hent filnavnet fra filbrowseren
        string MyFileName = Path.GetFileName(MyFile.FileName);
        //Tving filformaterne til at være skrevet med små bogstaver
        string MyFileEx = Path.GetExtension(MyFile.FileName.ToLower());

        //Tjek om mappen eksisterer
        DirEx(MyPath);

        //Hvis et af de tilladte fil-formater er at finde i filen, så 
        if (FileEx.Contains(MyFileEx))
        {
           
            //Guid FileId = Guid.NewGuid(); //Navngiver filerne med et unikt navn for at undgå at billederne hedder det samme.
            MyFile.SaveAs(HttpContext.Current.Server.MapPath(MyPath) + "/" + FileÍd.ToString() + "_" + MyFileName);

            Msg = "Ok";
        }
        //Hvis filformatet ikke er i FileEx (det array som står i Fil-controlleren fx string[] Ex = { ".jpg", ".png", ".gif" }
        else
        {
            Msg = "Forkert filformat";
        }

        return Msg;
    }

    public static string DelFile(string MyFilenameAndPath)
    {
        string Msg = "";

        File.Delete(HttpContext.Current.Server.MapPath(MyFilenameAndPath));
        Msg = "Filen blev slettet";

        return Msg;
    }






    //Slet hele mappen
    //public void DeleteDirectory(string targetDir)
    //{
    //    File.SetAttributes(targetDir, FileAttributes.Normal);

    //    string[] files = Directory.GetFiles(targetDir);
    //    string[] dirs = Directory.GetDirectories(targetDir);

    //    foreach (string file in files)
    //    {
    //        File.SetAttributes(file, FileAttributes.Normal);
    //        File.Delete(file);
    //    }

    //    foreach (string dir in dirs)
    //    {
    //        DeleteDirectory(dir);
    //    }

    //    Directory.Delete(targetDir, false);
    //}




    // laver en mappe hvis ikke den allerede findes..
    public static void DirEx(string MyPath)
    {
        if (!Directory.Exists(HttpContext.Current.Server.MapPath(MyPath)))
        {
            Directory.CreateDirectory(HttpContext.Current.Server.MapPath(MyPath));
        }
    }
}
