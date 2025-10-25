<p align="center">
    <img src="https://github.com/NeverMorewd/Lemon.ModuleNavigation/blob/master/src/Lemon.ModuleNavigation.Sample/Assets/lemon-100.png" />
</p>

<p align="center">
  <a href="https://www.nuget.org/packages/Lemon.Hosting.AvaloniauiDesktop/">
    <img src="https://img.shields.io/nuget/v/Lemon.Hosting.AvaloniauiDesktop.svg?label=NuGet" alt="NuGet Package Version"/>
  </a>
    
  <img src="https://img.shields.io/badge/AOT-Supported-brightgreen.svg" alt="AOT Supported"/>
  <img src="https://img.shields.io/badge/Linux-Supported-yellow.svg" alt="Linux Supported"/>
  <img src="https://img.shields.io/badge/macOS-Supported-ff69b4.svg" alt="macOS Supported"/>
  <img src="https://img.shields.io/badge/Windows-Supported-0078d7.svg" alt="Windows Supported"/>
</p>

# Introduction
.NET Generic Host support for Avaloniaui desktop app.
Support native aot!
- Examples:
```C#
internal sealed class Program
{
    [STAThread]
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

        RunApp(hostBuilder);
    }

    private static void RunApp(HostApplicationBuilder hostBuilder)
    {
        hostBuilder.Services.AddAppBuilder(BuildAvaloniaApp);
        var appHost = hostBuilder.Build();
        appHost.RunAvaloniaApp();
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
                    .UsePlatformDetect()
                    .WithInterFont()
                    .LogToTrace()
                    .UseReactiveUI();
    }
}
```

References:
[Nito.Host.Wpf](https://github.com/StephenCleary/Hosting)
