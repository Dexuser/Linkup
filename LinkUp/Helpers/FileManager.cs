namespace LinkUp.Helpers
{
    public static class FileManager
{
    public static string? Upload(IFormFile? file, string id, string folderName, bool isEditMode = false, string? imagePath = "")
    {
        if (isEditMode && file == null)
        {
            return imagePath;
        }

        if (file == null)
        {
            return string.Empty;
        }

        string basePath = Path.Combine("Images", folderName, id);
        string physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", basePath);

        if (!Directory.Exists(physicalPath))
        {
            Directory.CreateDirectory(physicalPath);
        }

        Guid guid = Guid.NewGuid();
        FileInfo fileInfo = new(file.FileName);
        string fileName = guid + fileInfo.Extension;

        string fullFilePath = Path.Combine(physicalPath, fileName);

        using (var stream = new FileStream(fullFilePath, FileMode.Create))
        {
            file.CopyTo(stream);
        }

        if (isEditMode && !string.IsNullOrWhiteSpace(imagePath))
        {
            string normalizedOldPath = imagePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
            string completeOldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", normalizedOldPath);

            if (File.Exists(completeOldPath))
            {
                File.Delete(completeOldPath);
            }
        }

        return $"/Images/{folderName}/{id}/{fileName}";
    }

    public static bool Delete(string id, string folderName)
    {
        string physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", folderName, id);

        if (Directory.Exists(physicalPath))
        {
            Directory.Delete(physicalPath, true);
            return true;
        }

        return false;
    }

    public static bool DeleteFile(string imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
            return false;

        string normalizedPath = imagePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
        string physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", normalizedPath);

        if (File.Exists(physicalPath))
        {
            File.Delete(physicalPath);
            return true;
        }

        return false;
    }
}

}