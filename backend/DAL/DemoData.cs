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
        ("Ziad Hamdy",      "31904120100147", 1,  [(27, 28), (28, 27), (29, 28), (27, 28)]), // 222/240 = A
        ("Farida Nabil",    "31808230100152", 2,  [(24, 23), (23, 22), (25, 24), (22, 23)]), // 186/240 = B
        ("Yassin Ashraf",   "31701090100168", 3,  [(21, 20), (19, 21), (20, 19), (21, 20)]), // 161/240 = C
        ("Malak Hesham",    "31611300100173", 4,  [(29, 29), (28, 29), (30, 29), (28, 28)]), // 230/240 = A
        ("Adam Sherif",     "31503050100181", 5,  [(17, 16), (15, 16), (18, 17), (16, 15)]), // 130/240 = D
        ("Jana Walid",      "31407190100196", 6,  [(23, 24), (24, 23), (22, 23), (24, 24)]), // 187/240 = B
        ("Mostafa Gamal",   "31302140100204", 7,  [(12, 11), (10, 13), (14, 12), (11, 10)]), //  93/240 = F
        ("Habiba Osama",    "31209260100219", 8,  [(25, 26), (24, 25), (26, 25), (23, 24)]), // 198/240 = B
        ("Seif Magdy",      "31106110100225", 9,  [(20, 20), (18, 20), (21, 20), (19, 18)]), // 156/240 = exactly 65% = C
        ("Rawan Tamer",     "31012030100236", 10, [(28, 30), (29, 28), (29, 29), (30, 28)]), // 231/240 = A
        ("Hamza Reda",      "30905170100241", 11, [(15, 16), (16, 15), (17, 16), (15, 14)]), // 124/240 = D
        ("Nada Hany",       "30808080100257", 12, [(26, 25), (25, 24), (27, 26), (24, 25)]), // 202/240 = B
        ("Marwan Sami",     "31910010100263", 1,  [(22, 21), (20, 22)]),                     // 2 subjects only: no overall grade yet
        ("Lina Kamal",      "31802280100278", 2,  [(22, 21), (21, 20), (20, 21), (22, 20)]), // 167/240 = C
        ("Taha Fouad",      "31706150100284", 3,  [(14, 13), (12, 14), (13, 12), (15, 13)]), // 106/240 = F
        ("Rana Emad",       "31604040100299", 4,  [(23, 22), (22, 23), (23, 22), (22, 23)]), // 180/240 = exactly 75% = B
        ("Khaled Wael",     "31512200100305", 5,  []),                                       // no marks yet
        ("Aya Mohsen",      "31410310100312", 6,  [(28, 26), (27, 27), (26, 28), (27, 27)]), // 216/240 = exactly 90% = A
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
