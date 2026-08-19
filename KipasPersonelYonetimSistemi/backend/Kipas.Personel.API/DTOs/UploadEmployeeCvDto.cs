using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Kipas.Personel.API.DTOs
{
    public class UploadEmployeeCvDto
    {
        [Required(ErrorMessage = "CV dosyası zorunludur.")]
        public IFormFile File { get; set; } = null!;
    }
}