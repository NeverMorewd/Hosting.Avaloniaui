using AvaloniaSample.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using ReactiveUI;
using System;
using System.Reactive.Linq;

namespace AvaloniaSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ILogger _logger;
        private readonly ISomeService _someService;
        public MainWindowViewModel(ISomeService someService, ILogger<MainWindowViewModel> logger) 
        {
            _logger = logger;
            _someService = someService;
            _stringValue = Greeting;
            Observable.Interval(TimeSpan.FromSeconds(1))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => 
                {
                    StringValue = $"{Greeting}:{someService.GetSomeNumber()}";
                });
        }

        private string _stringValue;
        public string StringValue
        {
            get { return _stringValue; }
            set { this.RaiseAndSetIfChanged(ref _stringValue, value); }
        }

        public string Greeting => "Welcome to Avalonia!";
    }
}
