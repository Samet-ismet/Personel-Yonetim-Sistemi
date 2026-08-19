using System.ComponentModel.DataAnnotations;

namespace Kipas.Personel.API.DTOs
{
    public class CreateEmployeeDto
        : IValidatableObject
    {
        [Required(
            ErrorMessage = "Ad zorunludur.")]
        [MaxLength(
            50,
            ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
        public string FirstName { get; set; } =
            string.Empty;

        [Required(
            ErrorMessage = "Soyad zorunludur.")]
        [MaxLength(
            50,
            ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
        public string LastName { get; set; } =
            string.Empty;

        [Required(
            ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(
            ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [MaxLength(
            254,
            ErrorMessage =
                "E-posta adresi en fazla 254 karakter olabilir.")]
        public string Email { get; set; } =
            string.Empty;

        [Required(
            ErrorMessage = "Telefon numarası zorunludur.")]
        [Phone(
            ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [MaxLength(
            30,
            ErrorMessage =
                "Telefon numarası en fazla 30 karakter olabilir.")]
        public string PhoneNumber { get; set; } =
            string.Empty;

        [Range(
     1,
     int.MaxValue,
     ErrorMessage =
         "Geçerli bir departman seçilmelidir.")]
        public int DepartmentId { get; set; }

        [Required(
            ErrorMessage = "Pozisyon zorunludur.")]
        [MaxLength(
            100,
            ErrorMessage =
                "Pozisyon en fazla 100 karakter olabilir.")]
        public string Position { get; set; } =
            string.Empty;

        public DateTime HireDate { get; set; }

        public bool IsActive { get; set; } = true;

        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (HireDate == default)
            {
                yield return new ValidationResult(
                    "İşe giriş tarihi zorunludur.",
                    new[] { nameof(HireDate) });
            }
            else if (HireDate.Date > DateTime.UtcNow.Date)
            {
                yield return new ValidationResult(
                    "İşe giriş tarihi gelecekte olamaz.",
                    new[] { nameof(HireDate) });
            }
        }
    }
}