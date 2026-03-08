namespace EmbeddedUI.UI.Models
{
    public class ChartEntry
    {
        public int Id { get; set; }
        public List<string> Categories { get; set; }
        public ChartIntervalEnum Interval { get; set; }
        public bool IsContinuous { get; set; }
        public int Index { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
