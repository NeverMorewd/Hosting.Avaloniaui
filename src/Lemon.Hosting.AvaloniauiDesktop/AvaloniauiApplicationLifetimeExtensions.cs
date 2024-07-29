using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

namespace Lemon.Hosting.AvaloniauiDesktop
{
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    public static class AvaloniauiApplicationLifetimeExtensions
    {
        /// <summary>
        /// Configures the host to use avaloniaui application lifetime.
        /// Also configures the <typeparamref name="TApplication"/> as a singleton.
        /// </summary>
        /// <typeparam name="TApplication">The type of avaloniaui application <see cref="Application"/> to manage.</typeparam>
        /// <param name="appBuilderConfiger"><see cref="AppBuilder.Configure{TApplication}()"/></param>

        public static IServiceCollection AddAvaloniauiDesktopApplication<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TApplication>(this IServiceCollection services,
            Func<AppBuilder, AppBuilder> appBuilderConfiger)
            where TApplication : Application, new()
        {
            return services
                    .AddSingleton<TApplication>()
                    .AddSingleton(provider =>
                    {
                        var appBuilder = AppBuilder.Configure(() =>
                        {
                            return provider.GetRequiredService<TApplication>();
                        });

                        appBuilder = appBuilderConfiger(appBuilder);

                        return appBuilder;
                    })
                    .AddSingleton<IHostLifetime, AvaloniauiApplicationLifetime<TApplication>>();
        }

        /// <summary>
        /// Add MainWindow&MainWindowViewModel to ServiceCollection
        /// </summary>
        /// <typeparam name="TWindow"><see cref="Window"/></typeparam>
        /// <typeparam name="TViewModel">MainWindowViewModel</typeparam>
        /// <param name="services"><see cref="IServiceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection AddMainWindow<TWindow, TViewModel>(this IServiceCollection services)
            where TWindow : Window where TViewModel : class
        {
            return services
                .AddSingleton<TViewModel>()
                .AddSingleton(provider =>
                { 
                    var viewModel = provider.GetRequiredService<TViewModel>();
                    var window= ActivatorUtilities.CreateInstance<TWindow>(provider);
                    window.DataContext = viewModel;
                    return window;
                });
        }


        /// <summary>
        /// Runs the avaloniaui application along with the .NET generic host.
        /// Note:Host will set the ShutdownMode with ShutdownMode.OnMainWindowClose
        /// </summary>
        /// <typeparam name="TApplication">The type of the avaloniaui application <see cref="Application"/> to run.</typeparam>
        /// <param name="commandArgs">commmandline args</param>
        /// <param name="cancellationToken">cancellationToken</param>
        public static Task RunAvaliauiApplication<TMainWindow>(this IHost host,
            string[] commandArgs,
            CancellationToken cancellationToken = default)
            where TMainWindow : Window
        {
            _ = host ?? throw new ArgumentNullException(nameof(host));
            var builder = host.Services.GetRequiredService<AppBuilder>();
            builder = builder.SetupWithLifetime(new ClassicDesktopStyleApplicationLifetime
            {
                Args = commandArgs,
                ShutdownMode = ShutdownMode.OnMainWindowClose,
            });

            if (builder.Instance == null)
            {
                throw new InvalidOperationException("AppBuilder has not been initialized yet!");
            }

            var hostTask = host.RunAsync(token: cancellationToken);

            var mainWindow = host.Services.GetRequiredService<TMainWindow>() ?? throw new InvalidOperationException("The MainWindow must been registered in Services before running");
            if (builder.Instance.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime classicDesktop)
            {
                classicDesktop.MainWindow = mainWindow;
                ///https://github.com/AvaloniaUI/Avalonia/pull/16167
                Environment.ExitCode = classicDesktop.Start(classicDesktop.Args ?? []);
#if DEBUG
                Console.WriteLine($"Process has been exited:{Environment.ExitCode}");
#endif
            }
            else
            {
                throw new InvalidOperationException("Generic host support classic desktop only!");
            }
            return hostTask;
        }
    }
}
