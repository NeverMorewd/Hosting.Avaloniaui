using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
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
        [Obsolete("This method will break the design view.")]
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
        /// Configures the host to use avaloniaui application lifetime.This method does not violate the definition of AppBuilder in Avaloniaui.
        /// </summary>
        /// <typeparam name="TApplication"></typeparam>
        /// <param name="services"></param>
        /// <param name="appBuilderConfiger"></param>
        /// <returns></returns>
        public static IServiceCollection AddAvaloniauiDesktopApplication<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TApplication>(this IServiceCollection services,
           Func<AppBuilder> appBuilderConfiger)
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

                        appBuilder = appBuilderConfiger();

                        return appBuilder;
                    })
                    .AddSingleton<IHostLifetime, AvaloniauiApplicationLifetime<TApplication>>();
        }
        /// <summary>
        /// Add MainWindow&MainWindowViewModel to ServiceCollection.
        /// </summary>
        /// <typeparam name="TWindow"><see cref="Window"/></typeparam>
        /// <typeparam name="TViewModel">MainWindowViewModel</typeparam>
        /// <param name="services"><see cref="IServiceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection AddMainWindow<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMainWindow, TViewModel>(this IServiceCollection services)
            where TMainWindow : Window 
            where TViewModel : class
        {
            return services
                .AddSingleton<TViewModel>()
                .AddKeyedSingleton<Window,TMainWindow>(typeof(TMainWindow))
                .AddSingleton(sp =>
                {
                    var viewmodel = sp.GetRequiredService<TViewModel>();
                    var window = sp.GetRequiredKeyedService<Window>(typeof(TMainWindow));
                    window.DataContext = viewmodel;
                    return (window as TMainWindow)!;
                });
        }

        /// <summary>
        /// Add ApplicationLifetime
        /// </summary>
        /// <param name="services"></param>
        /// <param name="lifetimeBuilder"></param>
        /// <returns></returns>
        public static IServiceCollection AddApplicationLifetime(this IServiceCollection services, Func<IServiceProvider, IClassicDesktopStyleApplicationLifetime> lifetimeBuilder)
        {
            return services
                    .AddSingleton(sp =>
                    {
                        var appBuilder = sp.GetRequiredService<AppBuilder>();
                        var lifetime = lifetimeBuilder(sp);
                        appBuilder = appBuilder.SetupWithLifetime(lifetime);
                        return lifetime;
                    });
        }

        /// <summary>
        /// Add ApplicationLifetime along with MainWindow
        /// </summary>
        /// <typeparam name="TMainWindow"></typeparam>
        /// <param name="services"></param>
        /// <param name="lifetimeBuilder"></param>
        /// <returns></returns>
        public static IServiceCollection AddApplicationLifetime<TMainWindow>(this IServiceCollection services, 
            Func<IServiceProvider, IClassicDesktopStyleApplicationLifetime> lifetimeBuilder) 
            where TMainWindow : Window
        {
            return services
                    .AddSingleton(sp =>
                    {
                        var appBuilder = sp.GetRequiredService<AppBuilder>();
                        var lifetime = lifetimeBuilder(sp);
                        appBuilder = appBuilder.SetupWithLifetime(lifetime);
                        lifetime.MainWindow = sp.GetRequiredService<TMainWindow>();
                        return lifetime;
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
            return RunAvaloniauiApplicationCore<TMainWindow>(host, 
                commandArgs, 
                shutdownMode, 
                cancellationToken);
        }
        /// <summary>
        /// Runs the avaloniaui application along with the .NET generic host.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="commandArgs"></param>
        /// <param name="shutdownMode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task RunAvaloniauiApplication(this IHost host,
            string[]? commandArgs = null,
            ShutdownMode shutdownMode = ShutdownMode.OnMainWindowClose,
            CancellationToken cancellationToken = default)
        {
            var lifetime = host.Services.GetService<IClassicDesktopStyleApplicationLifetime>();
            if (lifetime == null)
            {
                return RunAvaloniauiApplicationCore(host, commandArgs, shutdownMode, cancellationToken);
            }
            else
            {
                return RunAvaloniauiApplicationCore(host, lifetime, commandArgs, cancellationToken);
            }
        }
        private static Task RunAvaloniauiApplicationCore<TMainWindow>(IHost host,
            string[]? commandArgs,
            ShutdownMode shutdownMode = ShutdownMode.OnMainWindowClose,
            CancellationToken cancellationToken = default) where TMainWindow : Window
        {
            _ = host ?? throw new ArgumentNullException(nameof(host));
            var builder = host.Services.GetRequiredService<AppBuilder>();
            builder = builder.SetupWithLifetime(new ClassicDesktopStyleApplicationLifetime
            {
                Args = commandArgs,
                ShutdownMode = shutdownMode,
            });

            if (builder.Instance!.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                var window = host.Services.GetRequiredService<TMainWindow>();
                desktopLifetime.MainWindow = window;
                return RunHost(host, desktopLifetime, cancellationToken);
            }
            throw new InvalidOperationException("Generic host support classic desktop only!");
        }
        private static Task RunAvaloniauiApplicationCore(IHost host,
            string[]? commandArgs,
            ShutdownMode shutdownMode = ShutdownMode.OnMainWindowClose,
            CancellationToken cancellationToken = default)
        {
            _ = host ?? throw new ArgumentNullException(nameof(host));
            var builder = host.Services.GetRequiredService<AppBuilder>();
            
            builder = builder.SetupWithLifetime(new ClassicDesktopStyleApplicationLifetime
            {
                Args = commandArgs,
                ShutdownMode = shutdownMode,
            });

            if (builder.Instance!.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                return RunHost(host, desktopLifetime, cancellationToken);
            }
            throw new InvalidOperationException("Generic host support classic desktop only!");
        }

        private static Task RunAvaloniauiApplicationCore(IHost host,
            IClassicDesktopStyleApplicationLifetime lifetime,
            string[]? commandArgs = null,
            CancellationToken cancellationToken = default)
        {
            _ = host ?? throw new ArgumentNullException(nameof(host));
            ///https://github.com/AvaloniaUI/Avalonia/issues/17747
            if (lifetime is ClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                if (commandArgs != null && commandArgs.Length > 0)
                {
                    desktopLifetime.Args = commandArgs;
                }
                return RunHost(host, desktopLifetime, cancellationToken);
            }
            throw new InvalidOperationException($"Support '{nameof(ClassicDesktopStyleApplicationLifetime)}' only!");
        }

        private static Task RunHost(IHost host,
            ClassicDesktopStyleApplicationLifetime lifetime,
            CancellationToken cancellationToken = default)
        {
            var hostTask = host.RunAsync(token: cancellationToken);
            Environment.ExitCode = lifetime.Start(lifetime.Args ?? []);
            Debug.WriteLine($"ApplicationLifetime is terminated:{Environment.ExitCode}");
            return hostTask;
        }
    }
}
