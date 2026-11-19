namespace ChristmasGiftGame
{
    public partial class MainPage : ContentPage
    {
        private readonly GameViewModel ViewModel = new();
        private bool _drawing;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = ViewModel;
        }

        static async Task PressAsync(VisualElement element, double scale = 0.95)
        {
            await element.ScaleToAsync(scale, 120);
            await element.ScaleToAsync(1, 120);
        }

        async void AddPlayer_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(PlayerEntry.Text))
            {
                ViewModel.Players.Add(new Player { PlayerName = PlayerEntry.Text.Trim() });
                PlayerEntry.Text = "";

                ViewModel.GenerateCards();
            }

            await PressAsync(AddPlayer, 0.99);
        }

        async void RandomGift_Clicked(object sender, EventArgs e)
        {
            var error = ViewModel.DistributeRandomGifts(RandomGiftEntry.Text);
            if (error != null)
            {
                await DisplayAlertAsync("Slumpa ut presenter", error, "OK");
                return;
            }

            PlayerListRandom.IsVisible = true;
            RandomGiftEntry.IsVisible = false;
            RandomGiftButton.IsVisible = false;
            OptionMenu.IsVisible = true;
        }

        async void StartGame_Clicked(object sender, EventArgs e)
        {
            await PressAsync(StartGame, 0.99);

            if (ViewModel.Players.Count < 2)
            {
                var message = ViewModel.Players.Count == 1
                    ? "Du spelar själv, så du kan öppna alla presenterna nu! 😉 Lägg till minst en spelare till för att spela."
                    : "Lägg till minst 2 spelare för att kunna spela.";
                await DisplayAlertAsync("För få spelare", message, "OK");
                return;
            }

            StartGame.IsVisible = false;
            AddPlayer.IsVisible = false;
            PlayerEntry.IsVisible = false;
            PlayerList.IsVisible = false;
            DrawCard.IsVisible = false;
            MidGameGrid.IsVisible = true;
        }

        async void PlayDice_Clicked(object sender, EventArgs e)
        {
            await PressAsync(Playdice);
            MidGameGrid.IsVisible = false;
            DiceText.IsVisible = true;
            DiceRollButton.IsVisible = true;
            OptionMenu.IsVisible = true;
        }

        async void RandomGiveGift_Clicked(object sender, EventArgs e)
        {
            await PressAsync(RandomGiveGifts);
            MidGameGrid.IsVisible = false;
            RandomGiftEntry.IsVisible = true;
            RandomGiftButton.IsVisible = true;
            OptionMenu.IsVisible = true;
        }

        async void PlayCards_Clicked(object sender, EventArgs e)
        {
            await PressAsync(PlayCards);
            MidGameGrid.IsVisible = false;
            ShowCardGame();
        }

        async void BackMid_Clicked(object sender, EventArgs e)
        {
            await PressAsync(BackFromMid);
            MidGameGrid.IsVisible = false;
            PlayerEntry.IsVisible = true;
            AddPlayer.IsVisible = true;
            StartGame.IsVisible = true;
        }

        async void DrawCard_Clicked(object sender, EventArgs e)
        {
            // Snabba dubbeltryck får inte dra flera kort (det första skulle aldrig visas)
            if (_drawing)
                return;

            _drawing = true;
            try
            {
                await PressAsync(DrawCard, 0.99);

                ViewModel.DrawCard();
                ShowCardGame();

                await PressAsync(CardInPlay, 0.9);
            }
            finally
            {
                _drawing = false;
            }
        }

        async void DiceRoll_Clicked(object sender, EventArgs e)
        {
            await PressAsync(DiceRollButton, 0.98);

            ViewModel.DiceRoll();

            await PressAsync(DiceText, 0.9);
        }

        async void OptionMenuContinue_Clicked(object sender, EventArgs e)
        {
            await PressAsync(ContinueOptionMenu);

            DiceText.IsVisible = false;
            DiceRollButton.IsVisible = false;
            OptionMenu.IsVisible = false;
            PlayerListRandom.IsVisible = false;
            RandomGiftEntry.IsVisible = false;
            RandomGiftButton.IsVisible = false;
            ShowCardGame();
        }

        async void OptionMenuBack_Clicked(object sender, EventArgs e)
        {
            await PressAsync(BackOptionsMenu);

            MidGameGrid.IsVisible = true;
            DiceText.IsVisible = false;
            DiceRollButton.IsVisible = false;
            OptionMenu.IsVisible = false;
            PlayerListRandom.IsVisible = false;
            RandomGiftEntry.IsVisible = false;
            RandomGiftButton.IsVisible = false;
        }

        void ShowCardGame()
        {
            PlayerList.IsVisible = true;
            CardInPlay.IsVisible = ViewModel.CurrentCard != null;
            DrawCard.IsVisible = !ViewModel.IsGameOver;
            QuitGameButton.IsVisible = ViewModel.IsGameOver;
        }

        async void QuitGame_Clicked(object sender, EventArgs e)
        {
            await PressAsync(QuitGameButton);

            ViewModel.ResetGame();

            PlayerList.IsVisible = false;
            PlayerListRandom.IsVisible = false;
            CardInPlay.IsVisible = false;
            DrawCard.IsVisible = false;
            QuitGameButton.IsVisible = false;
            RandomGiftEntry.Text = "";

            PlayerEntry.IsVisible = true;
            AddPlayer.IsVisible = true;
            StartGame.IsVisible = true;
        }
    }
}
