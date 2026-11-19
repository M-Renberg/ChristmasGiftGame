using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;

namespace ChristmasGiftGame;

public class GameViewModel : INotifyPropertyChanged
{
    public const int MaxRandomGifts = 200;
    const int CardImageCount = 6;

    public event PropertyChangedEventHandler? PropertyChanged;

    void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public ObservableCollection<Player> Players { get; } = new();

    readonly List<string> _cards = new();
    readonly Random _random = new();

    private string? _currentCard;
    public string? CurrentCard
    {
        get => _currentCard;
        set
        {
            if (_currentCard != value)
            {
                _currentCard = value;
                OnPropertyChanged(nameof(CurrentCard));
            }
        }
    }

    private string? _cardImage;
    public string? CardImage
    {
        get => _cardImage;
        set
        {
            if (_cardImage != value)
            {
                _cardImage = value;
                OnPropertyChanged(nameof(CardImage));
            }
        }
    }

    private bool _isGameOver;
    public bool IsGameOver
    {
        get => _isGameOver;
        private set
        {
            if (_isGameOver != value)
            {
                _isGameOver = value;
                OnPropertyChanged(nameof(IsGameOver));
            }
        }
    }

    private string? _diceSideShown;
    public string? DiceSideShown
    {
        get => _diceSideShown;
        set
        {
            _diceSideShown = value;
            OnPropertyChanged(nameof(DiceSideShown));
        }
    }

    private Color _diceColor = Colors.Black; // Standardfärg
    public Color DiceColor
    {
        get => _diceColor;
        set
        {
            if (_diceColor != value)
            {
                _diceColor = value;
                OnPropertyChanged(nameof(DiceColor));
            }
        }
    }

    public GameViewModel()
    {
        GenerateCards();
    }

    public void DrawCard()
    {
        if (IsGameOver)
            return;

        if (_cards.Count == 0)
        {
            CurrentCard = "Spelet är nu slut. God jul!";
            SetRandomCardImage();
            IsGameOver = true;
            return;
        }

        int index = _random.Next(_cards.Count);
        CurrentCard = _cards[index];
        _cards.RemoveAt(index);
        SetRandomCardImage();
    }

    // Next(1, 7) ger 1–6, övre gränsen är exklusiv
    void SetRandomCardImage() =>
        CardImage = $"card{_random.Next(1, CardImageCount + 1)}.png";

    public class CardList
    {
        public List<string>? Cards { get; set; }
    }

    public List<string> LoadCardsFromJson()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "ChristmasGiftGame.Data.cards.json";

        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Resource not found: {resourceName}");

        using var reader = new StreamReader(stream);
        string json = reader.ReadToEnd();

        var data = JsonSerializer.Deserialize<CardList>(json);
        return data?.Cards ?? new List<string>();
    }

    public void GenerateCards()
    {
        _cards.Clear();
        CurrentCard = null;
        CardImage = null;
        IsGameOver = false;

        _cards.AddRange(LoadCardsFromJson());

        if (Players.Count >= 3)
        {
            foreach (var p in Players)
            {
                _cards.Add($"{p.PlayerName} får ta ett valfritt paket");
                _cards.Add($"{p.PlayerName} måste ge bort ett valfritt paket");
                _cards.Add($"{p.PlayerName} måste byta plats med en annan spelare");
                _cards.Add($"Ge ett valfritt paket till {p.PlayerName}");
            }
        }
    }

    public void DiceRoll()
    {
        int roll = _random.Next(1, 7);
        DiceSideShown = roll.ToString();
        DiceColor = roll is 1 or 6 ? Colors.Red : Colors.Black;
    }

    /// <summary>
    /// Slumpar ut paket bland spelarna. Returnerar ett felmeddelande, eller null om det gick bra.
    /// </summary>
    public string? DistributeRandomGifts(string? input)
    {
        if (Players.Count == 0)
            return "Lägg till spelare först.";

        if (!int.TryParse(input?.Trim(), out int totalGifts) || totalGifts < 1)
            return "Skriv hur många paket som ska slumpas ut (minst 1).";

        if (totalGifts > MaxRandomGifts)
            return $"Det går att slumpa ut max {MaxRandomGifts} paket.";

        foreach (var player in Players)
        {
            player.GiftCount = 0;
        }

        for (int i = 0; i < totalGifts; i++)
        {
            Players[_random.Next(Players.Count)].GiftCount++;
        }

        return null;
    }

    public void ResetGame()
    {
        Players.Clear();
        DiceSideShown = null;
        DiceColor = Colors.Black;
        GenerateCards();
    }
}
