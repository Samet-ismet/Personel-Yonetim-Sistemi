namespace Kipas.Personel.API.DTOs
{
    public sealed class EmployeeCvDownloadResult
    {
        public Stream FileStream { get; init; } = Stream.Null;

        public string ContentType { get; init; } =
            "application/pdf";

        public string FileName { get; init; } =
            "cv.pdf";
    }
}