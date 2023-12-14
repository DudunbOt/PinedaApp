namespace PinedaApp.Models.Errors
{
    public class ValidationErrors
    {
        private List<string> err = new List<string>();
        public List<string> Errors => err;
        public bool HasErrors => err.Count > 0;

        public void AddError(string error) { err.Add(error); }
        public void ClearErrors() { err.Clear(); }
    }
}
