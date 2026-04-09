namespace LibraryManagement.WebApp.Services;

public class SessionStateService
{
    public string CurrentReaderId { get; set; } = string.Empty;
    public bool IsLoggedIn => !string.IsNullOrEmpty(CurrentReaderId);
    
    public event Action? OnChange;
    
    public void SetReaderId(string id)
    {
        CurrentReaderId = id;
        NotifyStateChanged();
    }
    
    public void Logout()
    {
        CurrentReaderId = string.Empty;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
