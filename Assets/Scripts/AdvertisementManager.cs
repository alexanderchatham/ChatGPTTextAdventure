using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class AdvertisementManager : MonoBehaviour, IUnityAdsInitializationListener,IUnityAdsLoadListener,IUnityAdsShowListener
{
    [SerializeField] string _iOSGameId;
    [SerializeField] string _iOSAdUnitId = "Rewarded_iOS";
    [SerializeField] string _androidGameId;
    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] bool _testMode = true;
    private string _adUnitId;
    private string _gameId;

    public Button _showAdButton;
    void Awake()
    {
        InitializeAds();
        
        _showAdButton.interactable = false;
    }
 
    public void InitializeAds()
    {
    #if UNITY_IOS
            _gameId = _iOSGameId;
            _adUnitId = _iOSAdUnitId;
    #elif UNITY_ANDROID
            _gameId = _androidGameId;
            _adUnitId = _androidAdUnitId;
    #elif UNITY_EDITOR
            _gameId = _androidGameId; //Only for testing the functionality in the Editor
            _adUnitId = _androidAdUnitId;
    #endif
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, _testMode, this);
        }
    }

 
    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
        LoadAd();
    }
 
    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
    
    
    // Call this public method when you want to get an ad ready to show.
    public void LoadAd()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log("Loading Ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }
 
    // If the ad successfully loads, add a listener to the button and enable it:
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);
 
        if (adUnitId.Equals(_adUnitId))
        {
            // Configure the button to call the ShowAd() method when clicked:
            _showAdButton.onClick.AddListener(ShowAd);
            // Enable the button for users to click:
            _showAdButton.interactable = true;
        }
    }
 
    // Implement a method to execute when the user clicks the button:
    private bool rewardGranted = false;
    public void ShowAd()
    {
        // Disable the button:
        _showAdButton.interactable = false;
        rewardGranted = false;
        // Then show the ad:
        Advertisement.Show(_adUnitId, this);
    }
 
    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED) && rewardGranted == false)
        {
            rewardGranted = true;
            Debug.Log("Unity Ads Rewarded Ad Completed");
            // Grant a reward.
            EconomyCode.instance.WatchedRewardedAd();
            AdCooldown();
        }
    }

    private void AdCooldown()
    {
        Invoke("LoadAd",3f);
    }

    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }
 
    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }
 
    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }
 
    void OnDestroy()
    {
        // Clean up the button listeners:
        _showAdButton.onClick.RemoveAllListeners();
    }
}