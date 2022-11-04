namespace MindKey.Shared.Models.MindKey
{
    public class Tag
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public long? IdeaId { get; set; }

    }
}
