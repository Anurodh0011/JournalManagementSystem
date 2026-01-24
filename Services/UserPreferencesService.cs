using Microsoft.Maui.Storage;

namespace JournalManagementSystem.Services;

public interface IUserPreferencesService
{
    string GetUsername();
    void SetUsername(string username);
    event Action? OnUsernameChanged;
}

public class UserPreferencesService : IUserPreferencesService
{
    public event Action? OnUsernameChanged;

    public string GetUsername()
    {
        return Preferences.Get("username", "User");
    }

    public void SetUsername(string username)
    {
        Preferences.Set("username", username);
        OnUsernameChanged?.Invoke();
    }
}