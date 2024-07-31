using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaSample.ViewModels;
using AvaloniaSample.Views;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AvaloniaSample
{
    public partial class AppWithServiceProvider : Application
    {
        private readonly IServiceProvider _serviceProvider;
        public AppWithServiceProvider(IServiceProvider serviceProvider):base()
        {
            _serviceProvider = serviceProvider;
        }
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                /// MainWindow and MainWindowViewModel have been added in hosting
                desktop.MainWindow = new MainWindow
                {
                    //DataContext = new MainWindowViewModel(),
                    DataContext = _serviceProvider.GetRequiredService<MainWindowViewModelWithParams>()
                };

                /// ShutdownMode has been configured in hosting
                //desktop.ShutdownMode = Avalonia.Controls.ShutdownMode.OnMainWindowClose;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}