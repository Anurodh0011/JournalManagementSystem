using JournalManagementSystem.Model;

namespace JournalManagementSystem.Services;

public class UserSessionService
{
    public UserDisplayModel? CurrentUser { get; private set; }

    public void SetCurrentUser(UserDisplayModel user)
    {
        CurrentUser = user;
    }

    public UserDisplayModel? GetCurrentUser()
    {
        return CurrentUser;
    }

    public void Logout()
    {
        CurrentUser = null;
    }

    public bool IsLoggedIn => CurrentUser != null;
}
