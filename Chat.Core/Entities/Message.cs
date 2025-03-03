namespace Chat.Core.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public required string User { get; set; }
        public required string Content { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}
