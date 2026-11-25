namespace Application.Exceptions;

/// <summary>
/// The request is well-formed but breaks a rule that needs the database to check,
/// such as an academic year that doesn't exist. Returned as 400.
/// </summary>
public class BadRequestException : Exception
{
    public BadRequestException(string field, string message) : base(message)
    {
        Field = field;
    }

    public string Field { get; }
}
