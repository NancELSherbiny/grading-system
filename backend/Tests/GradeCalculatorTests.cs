using Application.Services;

namespace Tests;

public class GradeCalculatorTests
{
    // Subject totals out of 60, at every grade boundary.
    [Theory]
    [InlineData(60, "A")]
    [InlineData(54, "A")] // 90%
    [InlineData(53, "B")] // 88.33%
    [InlineData(45, "B")] // 75%
    [InlineData(44, "C")] // 73.33%
    [InlineData(39, "C")] // 65%
    [InlineData(38, "D")] // 63.33%
    [InlineData(30, "D")] // 50%
    [InlineData(29, "F")] // 48.33%
    [InlineData(0, "F")]
    public void GetGrade_returns_the_grade_for_each_boundary(int total, string expected)
    {
        Assert.Equal(expected, GradeCalculator.GetGrade(total, GradeCalculator.MaxSubjectTotal));
    }

    [Fact]
    public void Exactly_50_percent_passes()
    {
        Assert.True(GradeCalculator.IsPassed(30, 60));
        Assert.False(GradeCalculator.IsPassed(29, 60));
    }

    [Theory]
    [InlineData(60, 30)]
    [InlineData(240, 120)]
    public void MinimumPassingTotal_is_half_the_maximum(int maxTotal, int expected)
    {
        Assert.Equal(expected, GradeCalculator.MinimumPassingTotal(maxTotal));
    }

    [Fact]
    public void GetPercentage_rounds_to_two_decimals()
    {
        Assert.Equal(48.33m, GradeCalculator.GetPercentage(29, 60));
        Assert.Equal(50m, GradeCalculator.GetPercentage(120, 240));
    }

    [Fact]
    public void GetTotalRange_for_B_out_of_240()
    {
        Assert.Equal((180, 216), GradeCalculator.GetTotalRange("B", 240));
    }

    [Fact]
    public void GetTotalRange_for_A_has_no_upper_limit()
    {
        Assert.Equal((216, (int?)null), GradeCalculator.GetTotalRange("A", 240));
    }

    /// <summary>
    /// The students list filters grades in SQL using these ranges. For every possible total, the range
    /// must agree with the grade GetGrade gives, or filtering and displaying would disagree.
    /// </summary>
    [Theory]
    [InlineData(60)]
    [InlineData(240)]
    public void Every_total_falls_in_exactly_the_range_of_its_grade(int maxTotal)
    {
        foreach (var grade in new[] { "A", "B", "C", "D", "F" })
        {
            var (min, maxExclusive) = GradeCalculator.GetTotalRange(grade, maxTotal);

            for (var total = 0; total <= maxTotal; total++)
            {
                var inRange = total >= min && (maxExclusive is null || total < maxExclusive);
                var hasGrade = GradeCalculator.GetGrade(total, maxTotal) == grade;
                Assert.True(inRange == hasGrade, $"Total {total}/{maxTotal}, grade {grade}: in range {inRange}, has grade {hasGrade}");
            }
        }
    }
}
