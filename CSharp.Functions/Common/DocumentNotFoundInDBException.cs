namespace Common
{
    public class DocumentNotFoundInDBException : Exception
    {
        public DocumentNotFoundInDBException(string message) : base(message) 
        {
        }
    }
}
