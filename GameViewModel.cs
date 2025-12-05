using ChristmasGiftGame;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;

public class GameViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public string? CardImage { get; set; }

    void OnPropertyChanged(string name) =>
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public ObservableCollection<Player> Players { get; set; } = new();
    public ObservableCollection<Card> Cards = new();
    

    public Random random = new();

    public string? _currentCard;
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

    public string[] diceNumbers = ["1","2","3","4","5","6"];

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
        GenerateCards(); // add cards here
    }

    public void DrawCard()
    {
        if (Cards.Count == 0) 
        {
            Cards.Add(new Card { Text = "Spelet är nu slut. God jul" });
            return;
        }

        int index = random.Next(Cards.Count);
        CurrentCard = Cards[index].Text;
        Cards.RemoveAt(index);

        int img = random.Next(1, 6);
        CardImage = $"card{img}.png";
        OnPropertyChanged(nameof(CardImage));
    }

    public class CardList
    {
        public List<string>? Cards { get; set; }
    }
    public List<string> LoadCardsFromJson()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "ChristmasGiftGame.Data.cards.json"; // ändra om din namespace/folder skiljer sig

        var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
            throw new Exception($"Resource not found: {resourceName}");

        using var reader = new StreamReader(stream);
        string json = reader.ReadToEnd();

        var data = JsonSerializer.Deserialize<CardList>(json);
        return data?.Cards ?? new List<string>();
    }
    public void GenerateCards()
    {
        Cards.Clear();


        var baseCards = LoadCardsFromJson();

        foreach (var text in baseCards)
        {
            Cards.Add(new Card { Text = text });
        }
        

        if (Players.Count == 0)
        {
            Cards.Clear();
            Cards.Add(new Card { Text = "Du måste ha minst 2 spelare för att kunna spela" });
            return;
        }

        if (Players.Count == 1)
        {
            Cards.Clear();
            Cards.Add(new Card { Text = "Du spelar själv så du kan öppna alla presenterna nu" });
            return;
        }

        if(Players.Count == 2)
        {
            return;
        }
        else if (Players.Count >= 3)
        {
            Cards.Add(new Card { Text = "ni är 3 eller fler st" });
            foreach(var p in Players)
            {
                Cards.Add(new Card { Text = $"{p.PlayerName} får ta ett valfritt paket" });
                Cards.Add(new Card { Text = $"{p.PlayerName} måste ge bort ett valfritt paket" });
                Cards.Add(new Card { Text = $"{p.PlayerName} måste byta plats med en annan spelare" });
            }
        }
    }

    public void DiceRoll()
    {
        int index = random.Next(diceNumbers.Length);
        DiceSideShown = diceNumbers[index];

        if (DiceSideShown == "1" || DiceSideShown == "6")
        {
            DiceColor = Colors.Red;
        }
        else
        {
            DiceColor = Colors.Black; 
        }
    }

}