using ReactiveUI;

namespace AvaloniaSample.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject
    {
        private string _stringValue = "I am MainWindowViewModel";
        public string StringValue
        {
            get { return _stringValue; }
            set { this.RaiseAndSetIfChanged(ref _stringValue, value); }
        }
    }
}
