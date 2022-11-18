namespace JwtApplication.Models
{
    public class ApiResponseModel
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "";
        public object Data { get; set; } = null!;
    }
}
