namespace RestMockCore;

/// <summary>
/// Thrown by <see cref="Models.RouteTableItem.Verify()"/> overloads when a verifiable
/// route is not satisfied (e.g. it was never called, or it was called the wrong number of times).
/// </summary>
public class VerificationException : Exception
{
    public VerificationException(string message) : base(message) { }
}
