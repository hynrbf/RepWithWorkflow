namespace Common.Entities
{
    public class CustomerPermission : FcaPermission
    {
        public string State { get; set; }
        public bool HasPendingApplication { get; set; }
        public bool IsModified { get; set; }
    }
}
