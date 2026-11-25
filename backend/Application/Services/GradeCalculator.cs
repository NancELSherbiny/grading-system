using DAL.Models;

namespace Application.Services;

/// <summary>
/// Every grading rule in one place. Nothing else in the backend or frontend knows the thresholds.
/// A ≥ 90%, B ≥ 75%, C ≥ 65%, D ≥ 50%, F below 50%.
/// </summary>
public static class GradeCalculator
{
    /// <summary>Term 1 + Term 2, each out of 30.</summary>
    public const int MaxSubjectTotal = Mark.MaxTermMark * 2;

    /// <summary>
    /// The brief says "more than 50%" passes and "less than 50%" fails, but doesn't cover exactly 50%.
    /// Exactly 50% passes, because Grade D starts at 50%.
    /// </summary>
    public const decimal PassingPercentage = 50;

    // Highest grade first. Each grade starts at its percentage and ends where the grade above starts.
    private static readonly (string Grade, decimal MinPercentage)[] Scale =
    [
        ("A", 90),
        ("B", 75),
        ("C", 65),
        ("D", PassingPercentage),
        ("F", 0),
    ];

    /// <summary>Percentage rounded to two decimals, for display.</summary>
    public static decimal GetPercentage(int total, int maxTotal)
        => Math.Round(total * 100m / maxTotal, 2);

    /// <summary>Uses the unrounded percentage, so rounding can never lift a student into a higher grade.</summary>
    public static string GetGrade(int total, int maxTotal)
    {
        var percentage = total * 100m / maxTotal;
        return Scale.First(step => percentage >= step.MinPercentage).Grade;
    }

    public static bool IsPassed(int total, int maxTotal)
        => total * 100m / maxTotal >= PassingPercentage;

    /// <summary>Lowest total that passes: 30 out of 60, 120 out of 240.</summary>
    public static int MinimumPassingTotal(int maxTotal)
        => LowestTotalReaching(PassingPercentage, maxTotal);

    /// <summary>
    /// The totals that earn a grade, so the database can filter by grade by comparing numbers.
    /// Example out of 240: B is 180 (inclusive) to 216 (exclusive). A has no upper limit.
    /// </summary>
    public static (int Min, int? MaxExclusive) GetTotalRange(string grade, int maxTotal)
    {
        var index = Array.FindIndex(Scale, step => step.Grade == grade);
        if (index < 0)
        {
            throw new ArgumentException($"Unknown grade '{grade}'.", nameof(grade));
        }

        var min = LowestTotalReaching(Scale[index].MinPercentage, maxTotal);
        int? maxExclusive = index == 0 ? null : LowestTotalReaching(Scale[index - 1].MinPercentage, maxTotal);
        return (min, maxExclusive);
    }

    // Smallest whole number of marks that reaches the percentage, e.g. 75% of 60 = 45.
    private static int LowestTotalReaching(decimal percentage, int maxTotal)
        => (int)Math.Ceiling(percentage * maxTotal / 100);
}
