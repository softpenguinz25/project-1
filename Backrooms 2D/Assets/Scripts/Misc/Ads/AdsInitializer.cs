using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
	[Header("Setup")]
	[SerializeField] string _androidGameId;
	[SerializeField] string _iOSGameId;
	[SerializeField] bool _testMode = true;
	private string _gameId;

	public static bool adsInitialized;

	void Awake()
	{
		if (adsInitialized)
		{
			LoadAds();
			return;
		}

		//Debug.Log("Initializing Ads...");
		InitializeAds();
	}

	public void InitializeAds()
	{
		_gameId = (Application.platform == RuntimePlatform.IPhonePlayer)
			? _iOSGameId
			: _androidGameId;
		Advertisement.Initialize(_gameId, _testMode, this);
		//Debug.Log("Initializing Ads With Game Id: " + _gameId, gameObject);
		adsInitialized = true;
	}

	public void OnInitializationComplete()
	{
		Debug.Log("Unity Ads initialization complete.");

		LoadAds();
	}

	private static void LoadAds()
	{
		foreach (RewardedAdsMenuManager rewardedAdMenuManager in FindObjectsOfType<RewardedAdsMenuManager>(true))
		{
			var old = rewardedAdMenuManager.gameObject.activeSelf;
			rewardedAdMenuManager.gameObject.SetActive(true);

			//Debug.Log("Loading Ad Attached to this GameObject: " + rewardedAdMenuManager.gameObject, rewardedAdMenuManager);

			rewardedAdMenuManager.LoadAd();

			rewardedAdMenuManager.gameObject.SetActive(old);
		}
	}

	public void OnInitializationFailed(UnityAdsInitializationError error, string message)
	{
		Debug.Log($"Unity Ads Initialization Failed: {error} - {message}");
	}
}
