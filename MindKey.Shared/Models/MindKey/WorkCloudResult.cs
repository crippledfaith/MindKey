namespace MindKey.Shared.Models.MindKey
{
    public class WorkCloudResult
    {

        public List<WorkCloudData> Data { get; set; }
        public string Image { get; set; }

        public WorkCloudResult()
        {
            Data = new List<WorkCloudData>();
        }
    }
    public class WorkCloudData
    {
        public string Word { get; set; }
        public string FillStyle { get; set; }
        public string Font { get; set; }
        public float Rotate { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }
}
