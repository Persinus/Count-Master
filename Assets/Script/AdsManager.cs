using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using System;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    private InterstitialAd interstitialAd;
    private string adUnitId = "ca-app-pub-9273469039395735/2986381058"; // TEST ID

    void Start()
    {
        MobileAds.Initialize(initStatus => { });
        LoadInterstitialAd();
    }

    public void LoadInterstitialAd()
    {
        AdRequest adRequest = new AdRequest();

        InterstitialAd.Load(adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Interstitial ad failed to load: " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded successfully.");
                interstitialAd = ad;

                interstitialAd.OnAdFullScreenContentClosed += () =>
                {
                    Debug.Log("Ad closed.");
                    LoadInterstitialAd(); // Load lại
                };
            });
    }

    public void ShowInterstitialAd()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
        }
        else
        {
            Debug.Log("Interstitial ad is not ready yet.");
        }
    }
}
