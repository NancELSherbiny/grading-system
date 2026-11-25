namespace DAL.Models;

/// <summary>A student's Term 1 and Term 2 marks in one subject.</summary>
public class Mark
{
    public const int MaxTermMark = 30;

    public int Id { get; set; }

    public int StudentId { get; set; }

    public Student? Student { get; set; }

    public int SubjectId { get; set; }

    public Subject? Subject { get; set; }

    public int Term1 { get; set; }

    public int Term2 { get; set; }
}
