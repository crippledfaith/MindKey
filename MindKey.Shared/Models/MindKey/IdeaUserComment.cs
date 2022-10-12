namespace MindKey.Shared.Models.MindKey
{
    public class IdeaUserComment
    {
        public long Id { get; set; }
        public Idea Idea { get; set; } = default!;
        public User User { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime PostDateTime { get; set; } = default!;
        public ArgumentType Argument { get; set; } = default!;

    }
}
