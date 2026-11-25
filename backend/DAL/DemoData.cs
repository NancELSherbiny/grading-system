using DAL.Models;

namespace DAL;

/// <summary>
/// Sample students and marks, so the app isn't empty on first run. Added only when the Students table
/// is empty and "SeedDemoData" is true in configuration. All names and National IDs are made up.
/// </summary>
public static class DemoData
{
    // Marks are listed in subject order: Arabic, English, Mathematics, Science (subject ids 1 to 4).
    private static readonly (string Name, string NationalId, int AcademicYearId, (int Term1, int Term2)[] Marks)[] Students =
    [
        ("Mohamed Ahmed",   "31803150100015", 1,  [(28, 29), (27, 28), (29, 30), (28, 27)]), // 226/240 = A
        ("Ali Hassan",      "31706220100031", 2,  [(25, 24), (22, 23), (24, 25), (23, 22)]), // 188/240 = B
        ("Sara Mahmoud",    "31602110100042", 3,  [(20, 21), (19, 20), (22, 21), (20, 19)]), // 162/240 = C
        ("Omar Khaled",     "31509300100058", 4,  [(16, 15), (14, 15), (16, 15), (15, 14)]), // 120/240 = exactly 50% = D
        ("Nour Ibrahim",    "31404180100062", 5,  [(10, 12), (8, 9), (11, 10), (9, 8)]),     //  77/240 = F
        ("Youssef Mostafa", "31301050100077", 6,  [(26, 27), (25, 26), (28, 27), (26, 25)]), // 210/240 = B
        ("Mariam Adel",     "31211270100084", 7,  [(29, 30), (30, 29), (30, 30), (29, 29)]), // 236/240 = A
        ("Karim Tarek",     "31108140100093", 8,  [(24, 22), (21, 19)]),                     // 2 subjects only: no overall grade yet
        ("Hana Samir",      "31005090100107", 9,  [(23, 24), (20, 22), (25, 23)]),           // 3 subjects only: no overall grade yet
        ("Laila Fathy",     "30903030100112", 10, [(18, 17), (17, 16), (19, 18), (16, 17)]), // 138/240 = D
        ("Ahmed Mansour",   "30807210100125", 11, []),                                       // no marks yet
        ("Salma Yasser",    "30710160100139", 12, [(22, 23), (21, 22), (23, 24), (22, 21)]), // 178/240 = C
    ];

    public static void Seed(AppDbContext db)
    {
        if (db.Students.Any())
        {
            return;
        }

        foreach (var (name, nationalId, academicYearId, marks) in Students)
        {
            var student = new Student { Name = name, NationalId = nationalId, AcademicYearId = academicYearId };

            for (var i = 0; i < marks.Length; i++)
            {
                student.Marks.Add(new Mark { SubjectId = i + 1, Term1 = marks[i].Term1, Term2 = marks[i].Term2 });
            }

            db.Students.Add(student);
        }

        db.SaveChanges();
    }
}
