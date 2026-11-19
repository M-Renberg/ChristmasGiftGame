using System.ComponentModel;

namespace ChristmasGiftGame;

public class Player : INotifyPropertyChanged
{
    public string? PlayerName { get; set; }

    private int _giftCount;
    public int GiftCount
    {
        get => _giftCount;
        set
        {
            if (_giftCount != value)
            {
                _giftCount = value;
                // Detta är viktigt för att listan ska uppdateras live
                OnPropertyChanged(nameof(GiftCount));
            }
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}