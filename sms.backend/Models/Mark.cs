namespace sms.backend.Models
{
    public class Mark
    {
        public int MarkId { get; set; }
        public int StudentId { get; set; }
        public int LessonId { get; set; }
        public decimal Value { get; set; }
        public DateTime Date { get; set; }
    }
}