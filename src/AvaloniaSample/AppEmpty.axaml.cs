using Avalonia;
using Avalonia.Markup.Xaml;

namespace AvaloniaSample;

public partial class AppEmpty : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
}