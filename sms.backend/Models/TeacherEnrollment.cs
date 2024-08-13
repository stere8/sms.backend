using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class TeacherEnrollment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TeacherEnrollmentId { get; set; }
    public int StaffId { get; set; }
    public int ClassId { get; set; }
    public int LessonId { get; set; }
    
}