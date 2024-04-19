namespace Common.Entities
{
    public class PropertyChangeInfo
    {
        public string? PropertyName { get; set; }
        public string? Path { get; set; }
        public object? OldValue { get; set; }
        public object? NewValue { get; set; }
    }
}
