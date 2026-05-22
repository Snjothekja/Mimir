using Microsoft.AspNetCore.Routing.Constraints;
using System.Runtime.CompilerServices;

namespace Mimir.Server.FileStorage
{
    public class ImageUpload
    {

        public static string UploadImage(IFormFile image, string whereToUpload)
        {
            string fileName = Path.ChangeExtension(Path.GetRandomFileName(), ".jpg");
            var filePath = Path.Combine("C:/Users/Lykaios/Desktop/Coding_Stuff/Mimir/frontend/public/", fileName);
            string fileNamePath = "";
            switch (whereToUpload)
            {
                case "pfp":
                    filePath = Path.Combine("C:/Users/Lykaios/Desktop/Coding_Stuff/Mimir/frontend/public/pfps", fileName);
                    fileNamePath = "/pfps/" + fileName;
                    break;

                case "banner":
                    filePath = Path.Combine("C:/Users/Lykaios/Desktop/Coding_Stuff/Mimir/frontend/public/banners", fileName);
                    fileNamePath = "/banners/" + fileName;
                    break;

                case "postimage":
                    filePath = Path.Combine("C:/Users/Lykaios/Desktop/Coding_Stuff/Mimir/frontend/public/postimages", fileName);
                    fileNamePath = "/postimages/" + fileName;
                    break;
            }

            using (var stream = System.IO.File.Create(filePath))
            {
                image.CopyTo(stream);
            }
            
            

            return fileNamePath;
        }

    }
}
