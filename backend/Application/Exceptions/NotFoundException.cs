namespace Application.Exceptions;

/// <summary>The requested record doesn't exist. Returned as 404.</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}
