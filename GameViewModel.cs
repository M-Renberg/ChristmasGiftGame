using ChristmasGiftGame;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;

public class GameViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    void OnPropertyChanged(string name) =>
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public ObservableCollection<Player> Players = new();
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

    public GameViewModel()
    {
        GenerateCards(); // add cards here
    }

    public void DrawCard()
    {
        if (Cards.Count == 0) return;

        int index = random.Next(Cards.Count);
        CurrentCard = Cards[index].Text;
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

        //Cards.Add(new Card { Text = "Alla flyttar ett steg till höger" });
        //Cards.Add(new Card { Text = "Alla flyttar ett steg till höger" });
        //Cards.Add(new Card { Text = "Alla flyttar ett steg till höger" });
        //Cards.Add(new Card { Text = "Alla flyttar två steg till höger" });
        //Cards.Add(new Card { Text = "Alla flyttar tre steg till höger" });
        //Cards.Add(new Card { Text = "Alla flyttar ett steg till vänster" });
        //Cards.Add(new Card { Text = "Alla flyttar ett steg till vänster" });
        //Cards.Add(new Card { Text = "Alla flyttar två steg till vänster" });
        //Cards.Add(new Card { Text = "Alla flyttar två steg till vänster" });
        //Cards.Add(new Card { Text = "Alla flyttar tre steg till vänster" });
        //Cards.Add(new Card { Text = "Byta plats med den till höger om dig" });
        //Cards.Add(new Card { Text = "Byta plats med den till vänster om dig" });
        //Cards.Add(new Card { Text = "Byta plats med den mittemot dig" });
        //Cards.Add(new Card { Text = "Ta ett valfritt paket" });
        //Cards.Add(new Card { Text = "Ge bort ett valfritt paket" });
        //Cards.Add(new Card { Text = "Ta ett paket från den till vänster om dig" });
        //Cards.Add(new Card { Text = "Ta ett paket från den till höger om dig" });
        //Cards.Add(new Card { Text = "Byta ett paket med den till höger om dig" });
        //Cards.Add(new Card { Text = "Byta ett paket med den till vänster om dig" });
        //Cards.Add(new Card { Text = "Ge ett paket till den som har färst paket" });
        //Cards.Add(new Card { Text = "Ta ett paket från den som har flest paket" });
        //Cards.Add(new Card { Text = "Byta plats med den som har färst paket" });
        //Cards.Add(new Card { Text = "Byta plats med den som har flest paket" });
        //Cards.Add(new Card { Text = "Ta det största paketet" });
        //Cards.Add(new Card { Text = "ta den minsta paketet" });

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
            Cards.Add(new Card { Text = "Du spelar själv så du kan öppna alla presenterna nu" });
        }
        else if(Players.Count == 2)
        {
            Cards.Add(new Card { Text = "ni är 2 st" });
        }
        else if (Players.Count >= 3)
        {
            Cards.Add(new Card { Text = "ni är 3 eller fler st" });
        }
    }
}