using Plugin.InAppBilling;

namespace ChristmasGiftGame;

public record TipOption(string ProductId, string Title, string Price);

public enum TipResult { Thanks, Cancelled, Pending, Failed }

/// <summary>
/// "Bjud på en glögg": frivillig dricks via Google Play Billing.
/// Produkterna är förbrukningsbara (consumable), så samma dricks kan ges flera gånger.
/// Produkt-ID:na måste skapas som engångsprodukter i Play Console med exakt dessa ID:n.
/// </summary>
public static class TipService
{
    static readonly (string Id, string Title)[] Tips =
    [
        ("tip_glogg", "En glögg"),
        ("tip_glogg_pepparkakor", "Glögg och pepparkakor"),
        ("tip_julbord", "Ett helt julbord"),
    ];

    // Bara Android har produkter uppsatta (Play Console)
    public static bool IsSupported => DeviceInfo.Platform == DevicePlatform.Android;

    static readonly SemaphoreSlim _lock = new(1, 1);

    /// <summary>
    /// Hämtar dricksalternativen med priser från Google Play. Tom lista om något inte fungerar.
    /// Förbrukar också eventuell dricks som betalats men inte förbrukats (t.ex. om appen stängdes mitt i köpet),
    /// annars återbetalar Google den automatiskt efter tre dagar.
    /// </summary>
    public static async Task<IReadOnlyList<TipOption>> GetTipOptionsAsync()
    {
        if (!IsSupported)
            return [];

        await _lock.WaitAsync();
        var billing = CrossInAppBilling.Current;
        try
        {
            if (!await billing.ConnectAsync())
                return [];

            await ConsumeLeftoverTipsAsync(billing);

            var products = await billing.GetProductInfoAsync(ItemType.InAppPurchase, Tips.Select(t => t.Id).ToArray());
            var byId = products?.ToDictionary(p => p.ProductId) ?? new();

            return Tips
                .Where(t => byId.ContainsKey(t.Id))
                .Select(t => new TipOption(t.Id, t.Title, byId[t.Id].LocalizedPrice))
                .ToList();
        }
        catch (Exception)
        {
            return [];
        }
        finally
        {
            await DisconnectQuietlyAsync(billing);
            _lock.Release();
        }
    }

    public static async Task<TipResult> GiveTipAsync(string productId)
    {
        await _lock.WaitAsync();
        var billing = CrossInAppBilling.Current;
        try
        {
            if (!await billing.ConnectAsync())
                return TipResult.Failed;

            var purchase = await billing.PurchaseAsync(productId, ItemType.InAppPurchaseConsumable);
            if (purchase == null)
                return TipResult.Failed;

            switch (purchase.State)
            {
                case PurchaseState.Purchased:
                    await ConsumeQuietlyAsync(billing, purchase);
                    return TipResult.Thanks;
                case PurchaseState.PaymentPending:
                case PurchaseState.Deferred:
                    return TipResult.Pending;
                default:
                    return TipResult.Failed;
            }
        }
        catch (InAppBillingPurchaseException ex) when (ex.PurchaseError == PurchaseError.UserCancelled)
        {
            return TipResult.Cancelled;
        }
        catch (Exception)
        {
            return TipResult.Failed;
        }
        finally
        {
            await DisconnectQuietlyAsync(billing);
            _lock.Release();
        }
    }

    static async Task ConsumeLeftoverTipsAsync(IInAppBilling billing)
    {
        var purchases = await billing.GetPurchasesAsync(ItemType.InAppPurchase);
        if (purchases == null)
            return;

        foreach (var p in purchases)
        {
            if (p.State == PurchaseState.Purchased && Tips.Any(t => t.Id == p.ProductId))
                await ConsumeQuietlyAsync(billing, p);
        }
    }

    static async Task ConsumeQuietlyAsync(IInAppBilling billing, InAppBillingPurchase purchase)
    {
        try
        {
            // Om detta misslyckas tas det om nästa gång sidan öppnas (ConsumeLeftoverTipsAsync)
            await billing.ConsumePurchaseAsync(purchase.ProductId, purchase.PurchaseToken);
        }
        catch (Exception)
        {
        }
    }

    static async Task DisconnectQuietlyAsync(IInAppBilling billing)
    {
        try { await billing.DisconnectAsync(); } catch (Exception) { }
    }
}
