using DotNetBB.Repository.Abstraction.Interface;

namespace SampleApp.Api.Contexts;

public class UserContext: IUserContext
{
    public UserContext()
    {
        IsAuthorized = false;
        Username = "";
        ClientId = "";
    }

    public UserContext(string userName, string? clientId)
    {
        IsAuthorized = true;
        Username = userName;
        ClientId = clientId ?? "";
    }

    public bool IsAuthorized { get; }
    public string Username { get; }
    public string ClientId { get; }
}
