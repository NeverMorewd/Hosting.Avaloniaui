using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using System.Reactive;

namespace AvaloniaSample.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject
    {
        public ViewModelBase() 
        {
            ShutDownCommand = ReactiveCommand.Create(() => 
            {
                var lifeTime = Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
                lifeTime!.Shutdown();
            });
        }
        private string _stringValue = "I am MainWindowViewModel";
        public string StringValue
        {
            get { return _stringValue; }
            set { this.RaiseAndSetIfChanged(ref _stringValue, value); }
        }

        public ReactiveCommand<Unit,Unit> ShutDownCommand 
        { 
            get; 
        }
    }
}
