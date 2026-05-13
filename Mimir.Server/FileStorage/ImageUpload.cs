using Microsoft.AspNetCore.Routing.Constraints;
using System.Runtime.CompilerServices;

namespace Mimir.Server.FileStorage
{
    public class ImageUpload
    {

        public static string UploadImage(IFormFile image)
        {
            string fileName = Path.ChangeExtension(Path.GetRandomFileName(), ".jpg");
            var filePath = Path.Combine("C:/Users/Lykaios/Desktop/Coding_Stuff/Mimir/frontend/public/",
            fileName);

            using (var stream = System.IO.File.Create(filePath))
            {
                image.CopyTo(stream);
            }
            
            

            return "/" + fileName;
        }

    }
}
