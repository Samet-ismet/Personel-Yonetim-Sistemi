using Microsoft.AspNetCore.Http;

namespace Kipas.Personel.API.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SavePdfAsync(
            IFormFile file,
            CancellationToken cancellationToken);

        Stream? OpenRead(string storedFileName);

        Task DeleteAsync(string storedFileName);
    }
}