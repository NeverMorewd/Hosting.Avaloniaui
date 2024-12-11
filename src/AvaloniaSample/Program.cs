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
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

namespace AvaloniaSample
{
    internal sealed class Program
    {
        [STAThread]
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("linux")]
        [SupportedOSPlatform("macos")]
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

            #region run app default
            //RunAppDefault(hostBuilder, args);
            #endregion

            #region run empty app with mainwindow
            //RunAppWithMainWindow(hostBuilder, args);
            #endregion

            #region run app with serviceprovider
            //RunAppWithServiceProvider(hostBuilder, args);
            #endregion

            #region run app with serviceprovider
            //RunAppWithLifetime(hostBuilder, args);
            #endregion

            #region run app with serviceprovider
            RunAppWithLifetimePreSetupMainWindow(hostBuilder, args);
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
        private static void RunAppWithMainWindow(HostApplicationBuilder hostBuilder, string[] args)
        {
            // add avaloniaui application and config AppBuilder
            hostBuilder.Services.AddAvaloniauiDesktopApplication<AppEmpty>(ConfigAvaloniaAppBuilder);
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
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("linux")]
        [SupportedOSPlatform("macos")]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void RunAppWithLifetime(HostApplicationBuilder hostBuilder, string[] args)
        {

            hostBuilder.Services.AddAvaloniauiDesktopApplication<AppDefault>(ConfigAvaloniaAppBuilder);
            hostBuilder.Services.AddApplicationLifetime((sp) => 
            {
                var lifetime = new ClassicDesktopStyleApplicationLifetime
                {
                    Args = args,
                    ShutdownMode = ShutdownMode.OnMainWindowClose
                };
                return lifetime;
            });
            // build host
            var appHost = hostBuilder.Build();
            // run app
            appHost.RunAvaloniauiApplication(args);
        }
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("linux")]
        [SupportedOSPlatform("macos")]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void RunAppWithLifetimePreSetupMainWindow(HostApplicationBuilder hostBuilder, string[] args)
        {
            hostBuilder.Services.AddAvaloniauiDesktopApplication<AppEmpty>(ConfigAvaloniaAppBuilder);
            hostBuilder.Services.AddMainWindow<MainWindow, MainWindowViewModelWithParams>();
            hostBuilder.Services.AddApplicationLifetime<MainWindow>((sp) =>
            {
                var lifetime = new ClassicDesktopStyleApplicationLifetime
                {
                    Args = args,
                    ShutdownMode = ShutdownMode.OnMainWindowClose
                };
                return lifetime;
            });
            var appHost = hostBuilder.Build();
            appHost.RunAvaloniauiApplication(args);
        }
    }
}
