using Microsoft.AspNetCore.Http;
using Kipas.Personel.API.Interfaces;

namespace Kipas.Personel.API.Services
{
    public sealed class LocalFileStorageService
        : IFileStorageService
    {
        private readonly string _storageDirectory;
        private readonly ILogger<LocalFileStorageService> _logger;

        public LocalFileStorageService(
            IWebHostEnvironment environment,
            ILogger<LocalFileStorageService> logger)
        {
            _logger = logger;

            _storageDirectory = Path.Combine(
                environment.ContentRootPath,
                "App_Data",
                "employee-cvs");

            Directory.CreateDirectory(
                _storageDirectory);
        }

        public async Task<string> SavePdfAsync(
            IFormFile file,
            CancellationToken cancellationToken)
        {
            var storedFileName =
                $"{Guid.NewGuid():N}.pdf";

            var fullPath =
                GetSafeFullPath(storedFileName);

            try
            {
                await using var fileStream =
                    new FileStream(
                        fullPath,
                        FileMode.CreateNew,
                        FileAccess.Write,
                        FileShare.None,
                        bufferSize: 81920,
                        useAsync: true);

                await file.CopyToAsync(
                    fileStream,
                    cancellationToken);

                return storedFileName;
            }
            catch
            {
                try
                {
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                }
                catch (Exception cleanupException)
                {
                    _logger.LogWarning(
                        cleanupException,
                        "Başarısız CV yüklemesinden kalan dosya temizlenemedi. Dosya: {StoredFileName}",
                        storedFileName);
                }

                throw;
            }
        }

        public Stream? OpenRead(
            string storedFileName)
        {
            var fullPath =
                GetSafeFullPath(storedFileName);

            if (!File.Exists(fullPath))
            {
                return null;
            }

            return new FileStream(
                fullPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);
        }

        public Task DeleteAsync(
            string storedFileName)
        {
            var fullPath =
                GetSafeFullPath(storedFileName);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Task.CompletedTask;
        }

        private string GetSafeFullPath(
            string storedFileName)
        {
            if (string.IsNullOrWhiteSpace(storedFileName))
            {
                throw new InvalidOperationException(
                    "Dosya adı boş olamaz.");
            }

            var safeFileName =
                Path.GetFileName(storedFileName);

            if (!string.Equals(
                safeFileName,
                storedFileName,
                StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "Geçersiz dosya adı.");
            }

            var fileNameWithoutExtension =
                Path.GetFileNameWithoutExtension(
                    safeFileName);

            var extension =
                Path.GetExtension(safeFileName);

            if (!Guid.TryParseExact(
                    fileNameWithoutExtension,
                    "N",
                    out _) ||
                !string.Equals(
                    extension,
                    ".pdf",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Geçersiz saklanan dosya adı.");
            }

            return Path.Combine(
                _storageDirectory,
                safeFileName);
        }
    }
}