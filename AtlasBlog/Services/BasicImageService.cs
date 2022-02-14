using AtlasBlog.Services.Interfaces;
using System.IO;

namespace AtlasBlog.Services
{
    public class BasicImageService : IImageService
    {
        public string ConvertByteArrayToFile(byte[] fileData, string extension)
        {
            try
            {
                var imageBase64Data = Convert.ToBase64String(fileData);
                return $"data:{extension};base64,{imageBase64Data}";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            try
            {                
                using MemoryStream memorystream = new();
                await file.CopyToAsync(memorystream);
                byte[] byteFile = memorystream.ToArray();
                return byteFile;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

    }

}
