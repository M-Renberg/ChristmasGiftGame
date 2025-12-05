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

        async void StartGame_Clicked(object sender, EventArgs e)
        {
            await StartGame.ScaleToAsync(0.99, 120);
            StartGame.IsVisible = false;
            AddPlayer.IsVisible = false;
            PlayerEntry.IsVisible = false;
            PlayerList.IsVisible = true;
            DrawCard.IsVisible = true;
            

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
            ViewModel.DiceRoll();
        }

    }
}
