using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using OpenAI;
using TMPro;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Economy;
using Unity.Services.Economy.Model;

public class EconomyCode : MonoBehaviour
{
    public static EconomyCode instance;
    [SerializeField] private GameObject PurchasingPanel;
    public TextMeshProUGUI tokenText;
    private long tokens;

    private string TokensPurchaseID = "BUY_TOKENS_1";
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(this);
        }
    }

    public async void UpdateHUD()
    {
        GetBalancesResult result = await EconomyService.Instance.PlayerBalances.GetBalancesAsync();
        PlayerBalance tokenBalance = result.Balances.Single(balance => balance.CurrencyId == "STORY_TOKENS");
        print("token balance: "+tokenBalance.Balance);

        tokenText.text = "Story Tokens: " + tokenBalance.Balance;
        tokens = tokenBalance.Balance;
    }


    public async void Initialize()
    {
        
        // Economy needs to be initialized and then the user must sign in.

        // Cache the Economy configuration
        await EconomyService.Instance.Configuration.SyncConfigurationAsync();

        // NOTE: You need to set up your Economy configuration with the items used in here before running this sample,
        // or alternatively alter the sample to to work with your current configuration.
        ListAllCurrencyIds();

        

        GetBalancesResult result = await EconomyService.Instance.PlayerBalances.GetBalancesAsync();
        PlayerBalance tokenBalance = result.Balances.Single(balance => balance.CurrencyId == "STORY_TOKENS");
        PlayerBalance pointBalance = result.Balances.Single(balance => balance.CurrencyId == "STORY_POINTS");
        LeaderboardManager.instance.AddScore((int)pointBalance.Balance);
        
        print("token balance: "+tokenBalance.Balance);

        tokenText.text = "Story Tokens: " + tokenBalance.Balance;
        tokens = tokenBalance.Balance;
        /*
        await UpdatePlayersBalance("GOLD", 10);

        await AddItemToPlayersInventory("SWORD", "MY_ID");

        Dictionary<string, object> instanceData = new Dictionary<string, object>
        {
            { "rarity", "purple" }
        };
        await UpdatePlayersItemInstanceData("MY_ID", instanceData);

        await MakeVirtualPurchase("VIRTUAL_PURCHASE");

        await RedeemApplePurchase("APPLE_PURCHASE", "PURCHASE_RECEIPT", 10, "USD");

        await WriteLockExample();
        */
    }

    private void DeleteCurrentItems()
    {
        try
        {
            List<InventoryItemDefinition> items = EconomyService.Instance.Configuration.GetInventoryItems();
            print(items.Count);
            foreach (var VARIABLE in items)
            {
                EconomyService.Instance.PlayerInventory.DeletePlayersInventoryItemAsync(VARIABLE.Id, null);
                print("Deleting "+VARIABLE.Id);
            }
            
        }
        catch (EconomyException e)
        {
            Debug.LogError(e);
        }
    }

    private void ListAllCurrencyIds()
    {
        try
        {
            List<CurrencyDefinition> currencies = EconomyService.Instance.Configuration.GetCurrencies();

            List<string> currenciesIds = new List<string>();
            foreach (var currency in currencies)
            {
                currenciesIds.Add(currency.Id);
            }

            Debug.Log($"Currencies in config: {string.Join(", ", currenciesIds)}");
        }
        catch (EconomyException e)
        {
            Debug.LogError(e);
        }
    }

    public void SetTokensToZero()
    {
        UpdatePlayersBalance("STORY_TOKENS", 0);
    }

    private async Task UpdatePlayersBalance(string currencyId, long newBalance)
    {
        try
        {
            PlayerBalance updatedBalance = await EconomyService.Instance.PlayerBalances.SetBalanceAsync(currencyId, newBalance);
            
            Debug.Log($"{updatedBalance.CurrencyId} set to {updatedBalance.Balance}");

            if (updatedBalance.CurrencyId == "STORY_TOKENS")
            {
                tokenText.text = "Story Tokens: " + updatedBalance.Balance;
            }
        }
        catch (EconomyRateLimitedException e)
        {
            Debug.LogError($"{e} - Retry after {e.RetryAfter}");
        }
        catch (EconomyException e)
        {
            Debug.LogError(e);
        }
    }

    public async void StartNewStory()
    {
        if (getTokens() <= 0)
        {
            showTokenOptions();
            return;
        }
        await UseToken();
    }
    public async void ContinueStory(string c)
    {
        if (getTokens() <= 0)
        {
            showTokenOptions();
            return;
        }
        await UseToken(c);
    }

    
    public async Task UseToken()
    {
        
        await MakeVirtualPurchase("NEXT_SEGMENT");
        GetBalancesResult result = await EconomyService.Instance.PlayerBalances.GetBalancesAsync();
        PlayerBalance tokenBalance = result.Balances.Single(balance => balance.CurrencyId == "STORY_TOKENS");
        PlayerBalance pointBalance = result.Balances.Single(balance => balance.CurrencyId == "STORY_POINTS");
        LeaderboardManager.instance.AddScore((int)pointBalance.Balance);
        
        tokenText.text = "Story Tokens: " + tokenBalance.Balance;
        tokens = tokenBalance.Balance;
        
        print("spent Token");
        StreamResponse.instance.StartStory();

    
        
    }
    
    public async Task UseToken(string c)
    {
        if (getTokens() <= 0)
        {
            showTokenOptions();
            return;
        }
        await MakeVirtualPurchase("NEXT_SEGMENT");
        GetBalancesResult result = await EconomyService.Instance.PlayerBalances.GetBalancesAsync();
        PlayerBalance tokenBalance = result.Balances.Single(balance => balance.CurrencyId == "STORY_TOKENS");
        PlayerBalance pointBalance = result.Balances.Single(balance => balance.CurrencyId == "STORY_POINTS");
        LeaderboardManager.instance.AddScore((int)pointBalance.Balance);

    

        tokenText.text = "Story Tokens: " + tokenBalance.Balance;
        tokens = tokenBalance.Balance;
        
        print("spent Token");

        ContentHandler.instance.selectOption(c);
        
        
    }

    private InventoryExchangeItem currentItem;
    private async Task MakeVirtualPurchase(string purchaseId)
    {
        try
        {
            MakeVirtualPurchaseResult result = await EconomyService.Instance.Purchases.MakeVirtualPurchaseAsync(purchaseId);
            Debug.Log($"Successfully processed virtual purchase");
        }
        catch (EconomyValidationException e)
        {
            foreach (var errorDetail in e.Details)
            {
                foreach (var message in errorDetail.Messages)
                {
                    Debug.LogError(message);
                }
            }
        }
        catch (EconomyRateLimitedException e)
        {
            Debug.LogError($"{e} - Retry after {e.RetryAfter}");
        }
        catch (EconomyException e)
        {
            Debug.LogError(e);
        }
    }

    public void WatchedRewardedAd()
    {
        print("rewarding ad watch");
        UpdatePlayersBalance("STORY_TOKENS", tokens += 10);
        PurchasingPanel.SetActive(false);
    }

    public void BoughtTokens(int i)
    {
        print($"Purchased {i} Tokens");
        UpdatePlayersBalance("STORY_TOKENS", tokens += i);
        PurchasingPanel.SetActive(false);
        
    }

    public int getTokens()
    {
        return (int)tokens;
    }

    public void showTokenOptions()
    {
        PurchasingPanel.SetActive(true);
    }

    public void SetPointsToZero()
    {
        UpdatePlayersBalance("STORY_POINTS", 0);
    }
}
