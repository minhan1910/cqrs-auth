namespace Common.Responses
{
    public class Error
    {
        public List<string> ErrorMessages { get; set; } = new();
        public string FriendlyErrorMessage { get; set; } = string.Empty;
    }
}