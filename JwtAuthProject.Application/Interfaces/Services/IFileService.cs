using System;
using JwtAuthProject.Application.Common;
using Microsoft.AspNetCore.Http;

namespace JwtAuthProject.Application.Interfaces.Services;

public interface IFileService
{
    Task<string?> UploadAsync(IFormFile form, string folderName);
    Task<bool> DeleteAsync(string filePath);
}
