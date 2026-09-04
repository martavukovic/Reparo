namespace Reparo.App.Services;

public class ThemeService
{
    public bool IsDarkMode { get; private set; } = false;
    public event Action? ThemeChanged;

    public void Toggle()
    {
        IsDarkMode = !IsDarkMode;
        ThemeChanged?.Invoke();
    }
}