using MindKey.Shared.Models.MindKey;

namespace MindKey.Shared.Models
{
    public class ChatLine
    {
        public long Id { get; set; }
        public User User { get; set; } = default!;
        public string? Line { get; set; }
        public Idea Idea { get; set; } = default!;
        public DateTime DateTime { get; set; }
    }
}