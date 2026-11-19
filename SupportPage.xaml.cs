namespace ChristmasGiftGame;

public partial class SupportPage : ContentPage
{
	bool _tipsLoaded;
	bool _busy;

	public SupportPage()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		if (!TipService.IsSupported)
			return;

		if (_tipsLoaded)
		{
			// Försök igen med dricks som betalats men inte förbrukats (t.ex. en väntande betalning som blivit klar)
			if (!_busy)
				await TipService.RetryLeftoverTipsAsync();
			return;
		}

		var options = await TipService.GetTipOptionsAsync();
		if (options.Count == 0)
			return; // Ingen anslutning eller produkterna finns inte: dölj sektionen

		_tipsLoaded = true;
		TipButtons.Children.Clear();
		foreach (var option in options)
		{
			var button = new Button
			{
				Text = $"{option.Title} – {option.Price}",
				FontSize = 18,
				BackgroundColor = Colors.DarkSeaGreen,
				MaximumWidthRequest = 400,
			};
			button.Clicked += async (_, _) => await GiveTipAsync(option);
			TipButtons.Children.Add(button);
		}
		TipBorder.IsVisible = true;
	}

	async Task GiveTipAsync(TipOption option)
	{
		if (_busy)
			return;

		_busy = true;
		try
		{
			var result = await TipService.GiveTipAsync(option.ProductId);
			switch (result)
			{
				case TipResult.Thanks:
					await DisplayAlertAsync("Tack! 🎄", "Stort tack för glöggen! God jul!", "OK");
					break;
				case TipResult.Pending:
					await DisplayAlertAsync("Betalningen behandlas", "Tack! Betalningen är inte klar än, men går igenom så fort Google Play har godkänt den.", "OK");
					break;
				case TipResult.Failed:
					await DisplayAlertAsync("Något gick fel", "Det gick inte att genomföra betalningen just nu. Försök gärna igen senare.", "OK");
					break;
				case TipResult.Cancelled:
					break;
			}
		}
		finally
		{
			_busy = false;
		}
	}
}
