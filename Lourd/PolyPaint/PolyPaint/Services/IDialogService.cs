/**
 * Author Meshack Musundi
 * Source: https://www.codeproject.com/Articles/1181555/SignalChat-WPF-SignalR-Chat-Application
 */

namespace PolyPaint.Services
{
    public interface IDialogService
    {
        void ShowNotification(string message, string caption = "");
    }
}