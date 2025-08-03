using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class AdManager : MonoBehaviour
{
    private static AdManager _instance;

    // Singleton property
    public static AdManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Create a new GameObject with the AdsManager script if it doesn't exist
                _instance = new GameObject("AdManager").AddComponent<AdManager>();
            }
            return _instance;
        }
    }

    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-9273469039395735/2469808759";
#elif UNITY_IPHONE
    private string _adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
    private string _adUnitId = "unused";
#endif

    private BannerView _bannerView;

    // Start is called before the first frame update
    //private void Start()
    //{
    //    // Initialize the Google Mobile Ads SDK
    //    MobileAds.Initialize(initStatus => {
    //        Debug.Log("Google Mobile Ads SDK initialized.");
    //        LoadAd();
    //    });
    //}

    /// <summary>
    /// Creates the banner view (if not already created) and loads the ad.
    /// </summary>
    public void LoadAd()
    {
        // create an instance of a banner view first.
        if (_bannerView == null)
        {
            CreateBannerView();
        }

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }

    /// <summary>
    /// Creates a banner view and listens to ad events.
    /// </summary>
    private void CreateBannerView()
    {
        Debug.Log("Creating banner view");

        // Destroy existing banner if present
        if (_bannerView != null)
        {
            DestroyAd();
        }

        // Create a new banner with a specific size and position
        _bannerView = new BannerView(_adUnitId, AdSize.Banner, AdPosition.Top);
        ListenToAdEvents();
    }

    /// <summary>
    /// Listens to banner ad events.
    /// </summary>
    private void ListenToAdEvents()
    {
        _bannerView.OnBannerAdLoaded += () => {
            Debug.Log("Banner ad loaded successfully.");
        };

        _bannerView.OnBannerAdLoadFailed += error => {
            Debug.LogError($"Banner ad failed to load: {error}");
            LoadAd();  // Try to load again if failed
        };

        _bannerView.OnAdClicked += () => {
            Debug.Log("Banner ad was clicked.");
        };

        _bannerView.OnAdImpressionRecorded += () => {
            Debug.Log("Banner ad impression recorded.");
        };

        _bannerView.OnAdFullScreenContentOpened += () => {
            Debug.Log("Banner ad opened full screen content.");
        };

        _bannerView.OnAdFullScreenContentClosed += () => {
            Debug.Log("Banner ad closed full screen content.");
        };
    }

    /// <summary>
    /// Destroys the banner ad if present.
    /// </summary>
    public void DestroyAd()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner view.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }
    public void ShowAd()
    {
        // Initialize the Google Mobile Ads SDK
        MobileAds.Initialize(initStatus => {
            Debug.Log("Google Mobile Ads SDK initialized.");
            LoadAd();
        });
    }
}