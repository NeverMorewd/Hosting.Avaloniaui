using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

namespace Lemon.Hosting.AvaloniauiDesktop;

[SupportedOSPlatform("windows")]
[SupportedOSPlatform("linux")]
[SupportedOSPlatform("macos")]
public static class AvaloniauiApplicationLifetimeExtensions
{
    private const string MAINWINDOW_KEY = "D29D9D6F-A09E-4989-AC33-0EEBF537F30C-Lemon.Hosting";
    /// <summary>
    /// [Obsolete]Configures the host to use avaloniaui application lifetime.
    /// Also configures the <typeparamref name="TApplication"/> as a singleton.
    /// </summary>
    /// <typeparam name="TApplication">The type of avaloniaui application <see cref="Application"/> to manage.</typeparam>
    /// <param name="appBuilderConfiger"><see cref="AppBuilder.Configure{TApplication}()"/></param>
    [Obsolete($"Use {nameof(AddAppBuilder)} instead")]
    public static IServiceCollection AddAvaloniauiDesktopApplication<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TApplication>(this IServiceCollection services,
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
    /// Configures the host to use avaloniaui application lifetime.
    /// Also configures the <typeparamref name="TApplication"/> as a singleton.
    /// </summary>
    /// <param name="appBuilderFactory"><see cref="AppBuilder.Configure{TApplication}()"/></param>
    public static IServiceCollection AddAppBuilder(this IServiceCollection services,
        Func<AppBuilder> appBuilderFactory,
        Func<ClassicDesktopStyleApplicationLifetime>? lifetimeFactory = null)
    {
        var appBuilder = appBuilderFactory();
        var lifetime = lifetimeFactory?.Invoke() ?? new ClassicDesktopStyleApplicationLifetime();
        appBuilder = appBuilder.SetupWithLifetime(lifetime);
     
        if(appBuilder.Instance is null)
            throw new InvalidOperationException("AppBuilder.Instance is null after SetupWithLifetime!");
        return services
                .AddSingleton(appBuilder.Instance)
                .AddSingleton(appBuilder)
                .AddSingleton<IHostLifetime>(sp => ActivatorUtilities.CreateInstance<AvaloniauiApplicationLifetime<Application>>(sp, sp.GetRequiredService<Application>()));
    }


    /// <summary>
    /// Add MainWindow&MainWindowViewModel to ServiceCollection.
    /// </summary>
    /// <typeparam name="TWindow"><see cref="Window"/></typeparam>
    /// <typeparam name="TViewModel">MainWindowViewModel</typeparam>
    /// <param name="services"><see cref="IServiceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection AddMainWindow<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TMainWindow, 
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TViewModel>(this IServiceCollection services)
        where TMainWindow : Window
        where TViewModel : class
    {
        return services
            .AddSingleton<TViewModel>()
            .AddSingleton<TMainWindow>()
            .AddKeyedSingleton<Window, TMainWindow>(MAINWINDOW_KEY, (sp, key) =>
            {
                var viewmodel = sp.GetRequiredService<TViewModel>();
                var window = sp.GetRequiredService<TMainWindow>();
                window.DataContext = viewmodel;
                return window;
            });
    }

    /// <summary>
    /// [Obsolete]Runs the avaloniaui application along with the .NET generic host.
    /// Note:
    /// 1.Host will set the ShutdownMode with ShutdownMode.OnMainWindowClose
    /// 2.Support native AOT with rd.xml
    /// </summary>
    /// <typeparam name="TApplication">The type of the avaloniaui application <see cref="Application"/> to run.</typeparam>
    /// <param name="commandArgs">commmandline args</param>
    /// <param name="cancellationToken">cancellationToken</param>
    [Obsolete($"Use {nameof(RunAvaloniaAppAsync)} instead")]
    public static Task RunAvaloniauiApplication<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TMainWindow>(this IHost host,
        string[] commandArgs,
        ShutdownMode shutdownMode = ShutdownMode.OnMainWindowClose,
        CancellationToken cancellationToken = default)
        where TMainWindow : Window
    {
        return RunAvaloniaAppAsync(host, cancellationToken);
    }
    /// <summary>
    /// [Obsolete]Runs the avaloniaui application along with the .NET generic host.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="commandArgs"></param>
    /// <param name="shutdownMode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Obsolete($"Use {nameof(RunAvaloniaAppAsync)} instead")]
    public static Task RunAvaloniauiApplication(this IHost host,
        string[]? commandArgs = null,
        ShutdownMode shutdownMode = ShutdownMode.OnMainWindowClose,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return RunAvaloniaAppAsync(host, cancellationToken);
    }

    /// <summary>
    /// Runs the avaloniaui application along with the .NET generic host.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static Task RunAvaloniaAppAsync(this IHost host, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _ = host ?? throw new ArgumentNullException(nameof(host));
        var builder = host.Services.GetRequiredService<AppBuilder>();


        if (builder.Instance is null || builder.Instance.ApplicationLifetime is null)
        {
            builder = builder.SetupWithLifetime(new ClassicDesktopStyleApplicationLifetime());
        }

        if (builder.Instance!.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            var mainWindow = host.Services.GetKeyedService<Window>(MAINWINDOW_KEY);
            if (mainWindow is not null)
            {
                desktopLifetime.MainWindow = mainWindow;
            }
            return RunHostAsync(host, desktopLifetime, cancellationToken);
        }
        throw new InvalidOperationException("Generic host support classic desktop only!");
    }

    

    private static Task RunHostAsync(IHost host,
        ClassicDesktopStyleApplicationLifetime lifetime,
        CancellationToken cancellationToken = default)
    {
        var hostTask = host.RunAsync(token: cancellationToken);
        Environment.ExitCode = lifetime.Start(lifetime.Args ?? []);
        Debug.WriteLine($"ApplicationLifetime is terminated:{Environment.ExitCode}");
        return hostTask;
    }
}
