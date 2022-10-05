namespace MindKey.Shared.Models.MindKey
{
    public class Idea
    {
        public int IdeaKey { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Person Person { get; set; }
    }
}
