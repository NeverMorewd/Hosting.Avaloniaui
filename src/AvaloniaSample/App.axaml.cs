using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaSample.ViewModels;
using AvaloniaSample.Views;

namespace AvaloniaSample
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                /// MainWindow and MainWindowViewModel have been added in hosting
                //desktop.MainWindow = new MainWindow
                //{
                //    DataContext = new MainWindowViewModel(),
                //};

                /// ShutdownMode has been configured in hosting
                //desktop.ShutdownMode = Avalonia.Controls.ShutdownMode.OnMainWindowClose;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}