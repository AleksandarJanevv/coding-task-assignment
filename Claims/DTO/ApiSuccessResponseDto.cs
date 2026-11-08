namespace Claims.DTO
{
    public class ApiSuccessResponseDto<T>
    {
        public string ReturnCode { get; set; } = "SUCCESS";
        public string Details { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}
