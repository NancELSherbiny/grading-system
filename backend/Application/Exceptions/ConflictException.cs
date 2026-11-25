namespace Application.Exceptions;

/// <summary>The request clashes with existing data, such as a duplicate National ID. Returned as 409.</summary>
public class ConflictException : Exception
{
    public ConflictException(string field, string message) : base(message)
    {
        Field = field;
    }

    /// <summary>The field the error belongs to, so the frontend can show it under that input.</summary>
    public string Field { get; }
}
