namespace DotNetBB.Repository.Abstraction.Interface;

public interface IUserContext
{
    bool IsAuthorized { get; }
    string Username { get; }
    string ClientId { get; }
}