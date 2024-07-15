public class Class
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int GradeLevel { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; }
}