using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using AvaloniaSample.Services;
using AvaloniaSample.ViewModels;
using AvaloniaSample.Views;
using Lemon.Hosting.AvaloniauiDesktop;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace AvaloniaSample;

internal sealed class Program
{
    [STAThread]
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    public static async Task Main(string[] args)
    {
        var hostBuilder = Host.CreateApplicationBuilder();

        // config IConfiguration
        hostBuilder.Configuration
            .AddCommandLine(args)
            .AddEnvironmentVariables()
            .AddInMemoryCollection();

        // config ILogger
        hostBuilder.Services.AddLogging(builder => builder.AddConsole());
        // add some services
        hostBuilder.Services.AddSingleton<ISomeService, SomeService>();

        #region run app default
        //RunApp(hostBuilder);
        #endregion
        #region run app async
        var exitCode = await RunAppAsync(hostBuilder);
        Debug.WriteLine($"exitCode:{exitCode}");
        #endregion
        #region run empty app with mainwindow
        //RunAppWithMainWindow(hostBuilder);
        #endregion

        #region run app with lifetime
        //RunAppWithLifetimePreSetup(hostBuilder, args);
        #endregion
    }


    /// <summary>
    /// Default ConfigAvaloniaAppBuilder.
    /// </summary>
    /// <returns></returns>
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<AppDefault>()
                    .UsePlatformDetect()
                    .WithInterFont()
                    .LogToTrace()
                    .UseReactiveUI();
    }
    public static AppBuilder BuildAvaloniaEmptyApp()
    {
        return AppBuilder.Configure<AppEmpty>()
                    .UsePlatformDetect()
                    .WithInterFont()
                    .LogToTrace()
                    .UseReactiveUI();
    }
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private static void RunApp(HostApplicationBuilder hostBuilder)
    {
        hostBuilder.Services.AddAppBuilder(BuildAvaloniaApp);
        var appHost = hostBuilder.Build();
        appHost.RunAvaloniaAppAsync();
    }
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private static Task<int> RunAppAsync(HostApplicationBuilder hostBuilder)
    {
        hostBuilder.Services.AddAppBuilder(BuildAvaloniaApp);
        var appHost = hostBuilder.Build();
        return appHost.RunAvaloniaAppAsync();
    }
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private static void RunAppWithMainWindow(HostApplicationBuilder hostBuilder)
    {
        hostBuilder.Services.AddAppBuilder(BuildAvaloniaEmptyApp);
        hostBuilder.Services.AddMainWindow<MainWindow, MainWindowViewModelWithParams>();
        var appHost = hostBuilder.Build();
        appHost.RunAvaloniaAppAsync();
    }

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private static void RunAppWithLifetimePreSetup(HostApplicationBuilder hostBuilder, string[] args)
    {
        hostBuilder.Services.AddAppBuilder(BuildAvaloniaEmptyApp, () => new ClassicDesktopStyleApplicationLifetime
        {
            Args = args,
            ShutdownMode = ShutdownMode.OnMainWindowClose,
        });
        hostBuilder.Services.AddMainWindow<MainWindow, MainWindowViewModelWithParams>();
        var appHost = hostBuilder.Build();
        appHost.RunAvaloniaAppAsync();
    }
}
