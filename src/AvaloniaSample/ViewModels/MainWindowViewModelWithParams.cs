using AvaloniaSample.Services;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using System;
using System.Reactive.Linq;

namespace AvaloniaSample.ViewModels
{
    public class MainWindowViewModelWithParams : ViewModelBase
    {
        private readonly ILogger _logger;
        private readonly ISomeService _someService;
        public MainWindowViewModelWithParams(ISomeService someService, ILogger<MainWindowViewModelWithParams> logger) 
        {
            _logger = logger;
            _someService = someService;
            Observable.Interval(TimeSpan.FromSeconds(1))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ =>
                {
                    StringValue = $"Welcome to Lemon.Hosting.AvaloniauiDesktop:{_someService.GetSomeNumber()}";
                });
        }

    }
}
