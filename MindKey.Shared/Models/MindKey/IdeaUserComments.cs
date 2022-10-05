namespace MindKey.Shared.Models.MindKey
{
    public class IdeaUserComment
    {
        public long IdeaUserCommentKey { get; set; }
        public string Comment { get; set; } = default!;
        public Person Person { get; set; } = default!;
        public ArgumentType Argument { get; set; } = default!;

    }
}
