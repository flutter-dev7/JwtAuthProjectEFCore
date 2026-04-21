using JwtAuthProject.Application.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _env;

    public FileService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string?> UploadAsync(IFormFile file, string folderName)
    {
        try
        {
            if (file == null || file.Length == 0)
                throw new Exception("File is empty");

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", folderName);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine(folderName, fileName).Replace("\\", "/");
        }
        catch (System.Exception)
        {
            Console.WriteLine("File upload failed");
            return null;
        }
    }

    public Task<bool> DeleteAsync(string filePath)
    {
        var fullPath = Path.Combine(_env.WebRootPath, "uploads", filePath);

        if (!File.Exists(fullPath))
            return Task.FromResult(false);

        File.Delete(fullPath);
        return Task.FromResult(true);
    }
}