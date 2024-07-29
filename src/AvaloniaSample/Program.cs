using Avalonia;
using Avalonia.Controls;
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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

namespace AvaloniaSample
{
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

            // add avaloniaui application and config AppBuilder
            hostBuilder.Services.AddAvaloniauiDesktopApplication<App>(ConfigAvaloniaAppBuilder);

            hostBuilder.Services.AddSingleton<ISomeService, SomeService>();

            // add MainWindow & MainWindowViewModel
            hostBuilder.Services.AddMainWindow<MainWindow, MainWindowViewModel>();

            // build host
            var appHost = hostBuilder.Build();

            // run app
            appHost.RunAvaliauiApplication<MainWindow>(args);
        }

        public static AppBuilder ConfigAvaloniaAppBuilder(AppBuilder appBuilder)
        {
            return appBuilder
                        .UsePlatformDetect()
                        .WithInterFont()
                        .LogToTrace()
                        .UseReactiveUI();
        }
    }
}
