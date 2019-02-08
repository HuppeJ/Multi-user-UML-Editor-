/**
 * Author Meshack Musundi
 * Source: https://www.codeproject.com/Articles/1181555/SignalChat-WPF-SignalR-Chat-Application
 */

using Unity;
using PolyPaint.Services;
using PolyPaint.VueModeles;

namespace PolyPaint.Utilitaires
{
    public class ViewModelLocator
    {
        private UnityContainer container;

        public ViewModelLocator()
        {
            container = new UnityContainer();
            container.RegisterType<IDialogService, DialogService>();
            container.AddExtension(new Diagnostic());
        }

        public MainWindowViewModel MainVM
        {
            get { return container.Resolve<MainWindowViewModel>(); }
        }
    }
}
