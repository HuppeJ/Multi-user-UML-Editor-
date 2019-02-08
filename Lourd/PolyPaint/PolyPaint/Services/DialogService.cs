/**
 * Author Meshack Musundi
 * Source: https://www.codeproject.com/Articles/1181555/SignalChat-WPF-SignalR-Chat-Application
 */

using System.Windows;

namespace PolyPaint.Services
{
    public class DialogService : IDialogService
    {

        public void ShowNotification(string message, string caption = "")
        {
            MessageBox.Show(message, caption); ;
        }
    }
}
