using Microsoft.AspNetCore.Http;

namespace Kipas.Personel.API.Helpers
{
    public static class PdfFileValidator
    {
        public const long MaxFileSize =
            5 * 1024 * 1024;

        public const int MaxOriginalFileNameLength =
            255;

        private static readonly byte[] PdfSignature =
        {
            0x25, 0x50, 0x44, 0x46, 0x2D
        };

        public static async Task<string?> ValidateAsync(
            IFormFile? file,
            CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
            {
                return "CV dosyası boş olamaz.";
            }

            if (file.Length > MaxFileSize)
            {
                return "CV dosyası en fazla 5 MB olabilir.";
            }

            var originalFileName =
                Path.GetFileName(file.FileName);

            if (string.IsNullOrWhiteSpace(originalFileName))
            {
                return "Dosya adı geçerli değildir.";
            }

            if (originalFileName.Length >
                MaxOriginalFileNameLength)
            {
                return "Dosya adı en fazla 255 karakter olabilir.";
            }

            var extension =
                Path.GetExtension(originalFileName);

            if (!string.Equals(
                    extension,
                    ".pdf",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "Yalnızca PDF dosyaları yüklenebilir.";
            }

            await using var stream =
                file.OpenReadStream();

            var header =
                new byte[PdfSignature.Length];

            var totalBytesRead = 0;

            while (totalBytesRead < header.Length)
            {
                var bytesRead =
                    await stream.ReadAsync(
                        header.AsMemory(
                            totalBytesRead,
                            header.Length - totalBytesRead),
                        cancellationToken);

                if (bytesRead == 0)
                {
                    break;
                }

                totalBytesRead += bytesRead;
            }

            if (totalBytesRead != PdfSignature.Length ||
                !header.SequenceEqual(PdfSignature))
            {
                return "Gönderilen dosya geçerli bir PDF değildir.";
            }

            return null;
        }
    }
}