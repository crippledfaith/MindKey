namespace MindKey.Shared.Models.MindKey
{
    public class Idea
    {
        public int IdeaKey { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public Person Person { get; set; } = default!;
        public ArgumentType Argument { get; set; }

    }
}
