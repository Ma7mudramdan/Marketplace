namespace Marketplace.Domain.Interfaces
{
    /// <summary>
    /// Interface for Identity User (implemented by Infrastructure)
    /// This follows Dependency Inversion Principle
    /// </summary>
    
    public interface IIdentityUser
    {
        string Id { get; }
        string? Email { get; }
        string? UserName { get; }
    }
}