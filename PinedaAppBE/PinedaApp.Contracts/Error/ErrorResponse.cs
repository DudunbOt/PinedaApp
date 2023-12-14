namespace PinedaApp.Contracts
{
    public record ErrorResponse
    {
        public ErrorResponse(object error)
        {
            Errors = [];

            if (error is string)
            {
                Errors.Add((string)error);
            }
            else if (error is List<string>)
            {
                Errors.AddRange((List<string>)error);
            }
        }
        public List<string> Errors { get; private set; }
    }
}
