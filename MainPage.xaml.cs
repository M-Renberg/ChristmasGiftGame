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

        void AddPlayer_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(PlayerEntry.Text))
            {
                ViewModel.Players.Add(new Player { PlayerName = PlayerEntry.Text });
                PlayerEntry.Text = "";

                ViewModel.GenerateCards();
            }
        }

        async void DrawCard_Clicked(object sender, EventArgs e)
        {
            ViewModel.DrawCard();

            await CardInPlay.ScaleToAsync(0.9, 120);
            await CardInPlay.ScaleToAsync(1.0, 120);
        }

    }
}
