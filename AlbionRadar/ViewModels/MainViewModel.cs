using AlbionDataHandlers.Handlers;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AlbionRadar.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{

    private MobsHandler _mobsHandler;

    public MainViewModel(MobsHandler mobsHandler)
    {
        _mobsHandler = mobsHandler;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
