namespace Claims.DTO
{
    public class ApiErrorResponseDto
    {
        public string ReturnCode { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string? Id { get; set; }
    }
}
