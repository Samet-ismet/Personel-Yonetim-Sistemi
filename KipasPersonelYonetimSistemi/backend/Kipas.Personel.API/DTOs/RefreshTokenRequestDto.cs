using System.ComponentModel.DataAnnotations;

namespace Kipas.Personel.API.DTOs
{
    public class RefreshTokenRequestDto
    {
        [Required(
            ErrorMessage = "Refresh token zorunludur.")]
        [MaxLength(
            200,
            ErrorMessage =
                "Refresh token geçerli uzunlukta değildir.")]
        public string RefreshToken { get; set; } =
            string.Empty;
    }
}