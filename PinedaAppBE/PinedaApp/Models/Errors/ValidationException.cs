namespace PinedaApp.Models.Errors
{
    public class ValidationException : Exception
    {
        public ValidationErrors ValidationErrors { get; }

        public ValidationException(ValidationErrors validationErrors)
        {
            ValidationErrors = validationErrors;
        }
    }
}
