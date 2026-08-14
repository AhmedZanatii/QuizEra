using Microsoft.AspNetCore.Http;
namespace QuizEra.BLL.Helpers
{
    public static class FileUploader
    {
        public static async Task<string> UploadFile(string folder , IFormFile file)
        {


            var directory = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/Files", folder);
            var FileName = Guid.NewGuid() + Path.GetFileName(file.FileName);
            var filePath = Path.Combine(directory, FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return FileName;
        }
        public static void DeleteFile(string folder , string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }
            var directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files", folder);
            var filePath = Path.Combine(directory, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
