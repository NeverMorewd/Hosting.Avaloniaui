using ReactiveUI;

namespace AvaloniaSample.ViewModels
{
    public class MainWindowViewModel:ViewModelBase
    {
        private string _stringValue = "I am MainWindowViewModel";
        public string StringValue
        {
            get { return _stringValue; }
            set { this.RaiseAndSetIfChanged(ref _stringValue, value); }
        }
    }
}
