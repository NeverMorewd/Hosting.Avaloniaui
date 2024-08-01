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
            where TApplication : Application
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
        /// Add MainWindow&MainWindowViewModel to ServiceCollection.Note:Support native AOT with rd.xml
        /// </summary>
        /// <typeparam name="TWindow"><see cref="Window"/></typeparam>
        /// <typeparam name="TViewModel">MainWindowViewModel</typeparam>
        /// <param name="services"><see cref="IServiceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection AddMainWindow<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMainWindow, TViewModel>(this IServiceCollection services)
            where TMainWindow : Window where TViewModel : class
        {
            return services
                .AddSingleton<TViewModel>()
                .AddSingleton(sp =>
                {
                    var viewmodel = sp.GetRequiredService(typeof(TViewModel));
                    var window = ActivatorUtilities.CreateInstance<TMainWindow>(sp);
                    window.DataContext = viewmodel;
                    return window;
                });
        }


        /// <summary>
        /// Runs the avaloniaui application along with the .NET generic host.
        /// Note:
        /// 1.Host will set the ShutdownMode with ShutdownMode.OnMainWindowClose
        /// 2.Support native AOT with rd.xml
        /// </summary>
        /// <typeparam name="TApplication">The type of the avaloniaui application <see cref="Application"/> to run.</typeparam>
        /// <param name="commandArgs">commmandline args</param>
        /// <param name="cancellationToken">cancellationToken</param>
        public static Task RunAvaloniauiApplication<TMainWindow>(this IHost host,
            string[] commandArgs,
            ShutdownMode shutdownMode = ShutdownMode.OnLastWindowClose,
            CancellationToken cancellationToken = default)
            where TMainWindow : Window
        {
            var mainWindowFactory = host.Services.GetRequiredService<TMainWindow>;
            return RunAvaloniauiApplicationCore(host, commandArgs, shutdownMode, mainWindowFactory, cancellationToken);
        }

        /// <summary>
        ///  Runs the avaloniaui application along with the .NET generic host.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="commandArgs"></param>
        /// <param name="shutdownMode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task RunAvaloniauiApplication(this IHost host,
            string[] commandArgs,
            ShutdownMode shutdownMode = ShutdownMode.OnLastWindowClose,
            CancellationToken cancellationToken = default)
        {
            return RunAvaloniauiApplicationCore(host, commandArgs, shutdownMode, null, cancellationToken);
        }

        private static Task RunAvaloniauiApplicationCore(IHost host,
            string[] commandArgs,
            ShutdownMode shutdownMode = ShutdownMode.OnLastWindowClose,
            Func<Window>? mainWindowFactory = null,
            CancellationToken cancellationToken = default)
        {
            _ = host ?? throw new ArgumentNullException(nameof(host));
            var builder = host.Services.GetRequiredService<AppBuilder>();
            builder = builder.SetupWithLifetime(new ClassicDesktopStyleApplicationLifetime
            {
                Args = commandArgs,
                ShutdownMode = shutdownMode,
            });

            if (builder.Instance == null)
            {
                throw new InvalidOperationException("AppBuilder has not been initialized yet!");
            }

            var hostTask = host.RunAsync(token: cancellationToken);

            if (builder.Instance.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime classicDesktop)
            {
                if (mainWindowFactory != null)
                {
                    classicDesktop.MainWindow = mainWindowFactory() ?? throw new InvalidOperationException("The MainWindow must been registered in Services before running");
                }
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
