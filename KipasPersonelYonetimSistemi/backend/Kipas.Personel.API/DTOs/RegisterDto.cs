using System.ComponentModel.DataAnnotations;

namespace Kipas.Personel.API.DTOs
{
    public class RegisterDto
    {
        [Required(
            ErrorMessage = "Kullanıcı adı zorunludur.")]
        [MinLength(
            3,
            ErrorMessage =
                "Kullanıcı adı en az 3 karakter olmalıdır.")]
        [MaxLength(
            50,
            ErrorMessage =
                "Kullanıcı adı en fazla 50 karakter olabilir.")]
        [RegularExpression(
            @"^[a-zA-Z0-9._-]+$",
            ErrorMessage =
                "Kullanıcı adı yalnızca harf, rakam, nokta, alt çizgi ve kısa çizgi içerebilir.")]
        public string Username { get; set; } =
            string.Empty;

        [Required(
            ErrorMessage = "Şifre zorunludur.")]
        [MinLength(
            15,
            ErrorMessage =
                "Şifre en az 15 karakter olmalıdır.")]
        [MaxLength(
            100,
            ErrorMessage =
                "Şifre en fazla 100 karakter olabilir.")]
        public string Password { get; set; } =
            string.Empty;
    }
}