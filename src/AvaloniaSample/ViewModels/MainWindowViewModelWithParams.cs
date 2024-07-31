using AvaloniaSample.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
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
                    StringValue = $"Welcome to Avalonia:{someService.GetSomeNumber()}";
                });
        }

        private string _stringValue = "Welcome to Avalonia";
        public string StringValue
        {
            get { return _stringValue; }
            set { this.RaiseAndSetIfChanged(ref _stringValue, value); }
        }
    }
}
