namespace MindKey.Shared.Models.MindKey
{
    public class Idea
    {
        public long Id { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string DescriptionShort { get; set; } = default!;
        public List<Tag> Tags { get; set; } = default!;
        public Person Person { get; set; } = default!;
        public ArgumentType Argument { get; set; } = default!;
        public DateTime PostDateTime { get; set; } = default!;
        public long ForCount { get; set; } = 0;
        public long AgainstCount { get; set; } = 0;
        public long NetrulCount { get; set; } = 0;
        public bool IsDeleted { get; set; } = false;

        public Idea()
        {
            Tags = new List<Tag>();
        }
    }
}
