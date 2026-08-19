using System.ComponentModel.DataAnnotations;

namespace Kipas.Personel.API.DTOs
{
    public class EmployeeQueryParameters
    {
        [MaxLength(
            100,
            ErrorMessage =
                "Arama metni en fazla 100 karakter olabilir.")]
        public string? Search { get; set; }

        [Range(
    1,
    int.MaxValue,
    ErrorMessage =
        "Departman ID değeri en az 1 olmalıdır.")]
        public int? DepartmentId { get; set; }

        [Required(
            ErrorMessage = "Sıralama alanı zorunludur.")]
        [RegularExpression(
            @"(?i)^(id|firstname|lastname|department)$",
            ErrorMessage =
                "Sıralama alanı id, firstname, lastname veya department olmalıdır.")]
        public string SortBy { get; set; } = "id";

        [Required(
            ErrorMessage = "Sıralama yönü zorunludur.")]
        [RegularExpression(
            @"(?i)^(asc|desc)$",
            ErrorMessage =
                "Sıralama yönü asc veya desc olmalıdır.")]
        public string SortDirection { get; set; } = "asc";

        [Range(
            1,
            int.MaxValue,
            ErrorMessage =
                "Sayfa numarası en az 1 olmalıdır.")]
        public int PageNumber { get; set; } = 1;

        [Range(
            1,
            100,
            ErrorMessage =
                "Sayfa boyutu 1 ile 100 arasında olmalıdır.")]
        public int PageSize { get; set; } = 10;
    }
}