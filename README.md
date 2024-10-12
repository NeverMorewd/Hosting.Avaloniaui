<p align="center">
    <img src="https://github.com/NeverMorewd/Lemon.ModuleNavigation/blob/master/src/Lemon.ModuleNavigation.Sample/Assets/lemon-100.png" />
</p>

[![NuGet Badge](https://img.shields.io/badge/NuGet-v1.0.0-blue.svg)](https://www.nuget.org/packages/Lemon.Hosting.AvaloniauiDesktop)
# Introduction
.NET Generic Host support for Avaloniaui desktop app.
Support Aot!
- Examples:
```C#
internal sealed class Program
{
    [STAThread]
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    [RequiresDynamicCode("Calls Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder()")]
    public static void Main(string[] args)
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

        #region app default
        RunAppDefault(hostBuilder, args);
        #endregion

        #region app without mainwindow
        //RunAppWithoutMainWindow(hostBuilder, args);
        #endregion

        #region app with serviceprovider
        //RunAppWithServiceProvider(hostBuilder, args);
        #endregion
    }

    public static AppBuilder ConfigAvaloniaAppBuilder(AppBuilder appBuilder)
    {
        return appBuilder
                    .UsePlatformDetect()
                    .WithInterFont()
                    .LogToTrace()
                    .UseReactiveUI();
    }

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private static void RunAppDefault(HostApplicationBuilder hostBuilder, string[] args)
    {
        hostBuilder.Services.AddAvaloniauiDesktopApplication<AppDefault>(ConfigAvaloniaAppBuilder);
        // build host
        var appHost = hostBuilder.Build();
        // run app
        appHost.RunAvaloniauiApplication(args);
    }

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private static void RunAppWithoutMainWindow(HostApplicationBuilder hostBuilder, string[] args)
    {
        // add avaloniaui application and config AppBuilder
        hostBuilder.Services.AddAvaloniauiDesktopApplication<AppWithoutMainWindow>(ConfigAvaloniaAppBuilder);
        // add MainWindow & MainWindowViewModelWithParams
        hostBuilder.Services.AddMainWindow<MainWindow, MainWindowViewModelWithParams>();
        // build host
        var appHost = hostBuilder.Build();
        // run app
        appHost.RunAvaloniauiApplication<MainWindow>(args);
    }

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private static void RunAppWithServiceProvider(HostApplicationBuilder hostBuilder, string[] args) 
    {
        // add avaloniaui application and config AppBuilder
        hostBuilder.Services.AddAvaloniauiDesktopApplication<AppWithServiceProvider>(ConfigAvaloniaAppBuilder);
        // add MainWindowViewModelWithParams
        hostBuilder.Services.AddSingleton<MainWindowViewModelWithParams>();
        // build host
        var appHost = hostBuilder.Build();
        // run app
        appHost.RunAvaloniauiApplication(args);
    }
}
```

References:
[Nito.Host.Wpf](https://github.com/StephenCleary/Hosting)
