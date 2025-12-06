namespace ChristmasGiftGame
{
    public partial class MainPage : ContentPage
    {
        private GameViewModel ViewModel = new();
        public MainPage()
        {
            InitializeComponent();
            BindingContext = ViewModel;
        }

        async void AddPlayer_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(PlayerEntry.Text))
            {
                ViewModel.Players.Add(new Player { PlayerName = PlayerEntry.Text });
                PlayerEntry.Text = "";

                ViewModel.GenerateCards();
            }

            await AddPlayer.ScaleToAsync(0.99, 120);
            await AddPlayer.ScaleToAsync(1, 120);
        }

        async void RandomGift_Clicked(object sender, EventArgs e) 
        {
            if (!string.IsNullOrWhiteSpace(RandomGiftEntry.Text)) 
            {
                ViewModel.RandomGift = RandomGiftEntry.Text;
                
            }
        }

        async void StartGame_Clicked(object sender, EventArgs e)
        {
            await StartGame.ScaleToAsync(0.99, 120);
            StartGame.IsVisible = false;
            AddPlayer.IsVisible = false;
            PlayerEntry.IsVisible = false;
            PlayerList.IsVisible = false;
            DrawCard.IsVisible = false;
            MidGameGrid.IsVisible = true;
        }
        async void PlayDice_Clicked(object sender, EventArgs e)
        {
            await Playdice.ScaleToAsync(0.95, 120);
            await Playdice.ScaleToAsync(1, 120);
            MidGameGrid.IsVisible = false;
            DiceText.IsVisible = true;
            DiceRollButton.IsVisible = true;
            OptionMenu.IsVisible = true;
        }
        async void RandomGiveGift_Clicked(object sender, EventArgs e) 
        {
            await RandomGiveGifts.ScaleToAsync(0.95, 120);
            await RandomGiveGifts.ScaleToAsync(1, 120);
            MidGameGrid.IsVisible = false;
        }
        async void PlayCards_Clicked(object sender, EventArgs e) 
        {
            await PlayCards.ScaleToAsync(0.95, 120);
            await PlayCards.ScaleToAsync(1, 120);
            MidGameGrid.IsVisible = false;
            PlayerList.IsVisible = true;
            DrawCard.IsVisible = true;
        }
        async void BackMid_Clicked(object sender, EventArgs e) 
        {
            await BackFromMid.ScaleToAsync(0.95, 120);
            await BackFromMid.ScaleToAsync(1, 120);
            MidGameGrid.IsVisible = false;
            PlayerEntry.IsVisible = true;
            AddPlayer.IsVisible = true;
            StartGame.IsVisible = true;
        }

        async void DrawCard_Clicked(object sender, EventArgs e)
        {
            await DrawCard.ScaleToAsync(0.99, 120);
            await DrawCard.ScaleToAsync(1, 120);

            if (CardInPlay.IsVisible == false)
            {
                CardInPlay.IsVisible = true;
            }
            ViewModel.DrawCard();

            await CardInPlay.ScaleToAsync(0.9, 120);
            await CardInPlay.ScaleToAsync(1.0, 120);
        }

        async void DiceRoll_Clicked(object sender, EventArgs e)
        {
            await DiceRollButton.ScaleToAsync(0.98, 120);
            await DiceRollButton.ScaleToAsync(1, 120);

            ViewModel.DiceRoll();

            await DiceText.ScaleToAsync(0.9, 120);
            await DiceText.ScaleToAsync(1, 120);
        }

        async void OptionMenuContinue_Clicked(object sender, EventArgs e) 
        {
            PlayerList.IsVisible = true;
            DrawCard.IsVisible = true;
            DiceText.IsVisible = false;
            DiceRollButton.IsVisible = false;
            OptionMenu.IsVisible = false;
        }

        async void OptionMenuBack_Clicked(object sender, EventArgs e) 
        {
            MidGameGrid.IsVisible = true;
            DiceText.IsVisible = false;
            DiceRollButton.IsVisible = false;
            OptionMenu.IsVisible = false;
        }

    }
}
