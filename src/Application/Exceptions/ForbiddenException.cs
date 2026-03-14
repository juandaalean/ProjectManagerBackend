namespace Application.Exceptions;

/// <summary>
/// Exception thrown when access to a resource is forbidden.
/// </summary>
public class ForbiddenException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenException"/> class.
    /// </summary>
    public ForbiddenException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public ForbiddenException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ForbiddenException(string message, Exception innerException) : base(message, innerException) { }
}