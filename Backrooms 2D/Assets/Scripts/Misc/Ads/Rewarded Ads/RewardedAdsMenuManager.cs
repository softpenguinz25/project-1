using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using System;
using System.Collections.Generic;

public class RewardedAdsMenuManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] List<Button> _showAdButtons = new List<Button>();
    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] string _iOSAdUnitId = "Rewarded_iOS";
    string _adUnitId = null; // This will remain null for unsupported platforms

    Action CurrentRewardAction;

    void Awake()
    {
#if !(UNITY_IOS || UNITY_ANDROID)
        enabled = false;
        return;
#endif

		// Get the Ad Unit ID for the current platform:
#if UNITY_IOS
        _adUnitId = _iOSAdUnitId;
#elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
#endif

		//Disable the button until the ad is ready to show:
		foreach (Button _showAdButton in _showAdButtons) _showAdButton.interactable = false;
    }

    // Load content to the Ad Unit:
    public void LoadAd()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        //Debug.Log("Loading Ad: " + _adUnitId);        
        Advertisement.Load(_adUnitId, this);
    }

    // If the ad successfully loads, add a listener to the button and enable it:
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        //Debug.Log("Ad Loaded: " + adUnitId);

		if (adUnitId.Equals(_adUnitId))
		{
			// Configure the button to call the ShowAd() method when clicked:
			//foreach (Button _showAdButton in _showAdButtons) _showAdButton.onClick.AddListener(ShowAd);
			// Enable the button for users to click:
			foreach (Button _showAdButton in _showAdButtons) _showAdButton.interactable = true;
		}
	}

    // Implement a method to execute when the user clicks the button:
    public void ShowAd(Action rewardAction)
    {
        CurrentRewardAction = rewardAction;
        // Disable the button:
        foreach (Button _showAdButton in _showAdButtons) _showAdButton.interactable = false;
        // Then show the ad:
        Advertisement.Show(_adUnitId, this);
    }

    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            //Debug.Log("Unity Ads Rewarded Ad Completed");
            // Grant a reward.
            CurrentRewardAction();

            // Load another ad:
            Advertisement.Load(_adUnitId, this);
        }
    }

    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }

    void OnDestroy()
    {
        // Clean up the button listeners:
        foreach (Button _showAdButton in _showAdButtons) _showAdButton.onClick.RemoveAllListeners();
    }
}