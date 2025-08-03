using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using System.Collections.Generic;

public class InterstitialAdExample : MonoBehaviour
{
    private InterstitialAd _interstitialAd;

    // Ad unit ID for testing purposes. Replace with your own ad unit ID.
#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-9273469039395735/1473826825";
#elif UNITY_IPHONE
    private string _adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
    private string _adUnitId = "unused";
#endif

    void Start()
    {
 


        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("Google Mobile Ads SDK initialized.");
        });

        // Load the interstitial ad.
        LoadInterstitialAd();
        
    }

    public void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("Loading interstitial ad.");

        var adRequest = new AdRequest();

        InterstitialAd.Load(_adUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Failed to load interstitial ad: " + error);
                return;
            }

            Debug.Log("Interstitial ad loaded successfully.");
            _interstitialAd = ad;

            // Register for ad events.
            RegisterAdEvents(ad);
        });
    }

    public void ShowInterstitialAd()
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            _interstitialAd.Show();
        }
        else
        {
            Debug.LogError("Interstitial ad is not ready yet.");
        }
    }

    private void RegisterAdEvents(InterstitialAd interstitialAd)
    {
        interstitialAd.OnAdPaid += adValue =>
        {
            Debug.Log($"Interstitial ad paid: {adValue.Value} {adValue.CurrencyCode}");
        };

        interstitialAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };

        interstitialAd.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad clicked.");
        };

        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full-screen content opened.");
        };

        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full-screen content closed.");
            LoadInterstitialAd(); // Load a new ad for the next opportunity.
        };

        interstitialAd.OnAdFullScreenContentFailed += adError =>
        {
            Debug.LogError("Interstitial ad failed to open full-screen content: " + adError);
            LoadInterstitialAd(); // Reload the ad on failure.
        };
    }

    private void OnDestroy()
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
        }
    }
}