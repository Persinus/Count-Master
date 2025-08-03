using UnityEngine;
using GoogleMobileAds.Api;
using TMPro;

public class RewardAdManager : MonoBehaviour
{
    private RewardedAd rewardedAd;

    public string androidAdUnitId = "ca-app-pub-9273469039395735/4707490988"; // test ID
    public string iosAdUnitId = "ca-app-pub-3940256099942544/1712485313"; // test ID

    public TextMeshProUGUI coinText;
    private int currentCoins = 100;

    void Start()
    {
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("AdMob initialized");
            LoadRewardedAd(); // Load ad sau khi init xong
        });

        UpdateCoinUI();
    }

    public void LoadRewardedAd()
    {
        string adUnitId =
#if UNITY_ANDROID
            androidAdUnitId;
#elif UNITY_IOS
            iosAdUnitId;
#else
            "unused";
#endif

        AdRequest request = new AdRequest();

        RewardedAd.Load(adUnitId, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError($"Failed to load rewarded ad: {error}");
                return;
            }

            Debug.Log("Rewarded ad loaded");
            rewardedAd = ad;

            RegisterEventHandlers(ad);
        });
    }

    public void ShowRewardAd()
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"User earned reward: {reward.Amount}");
                AddCoins(100);
            });
        }
        else
        {
            Debug.LogWarning("Rewarded ad is not ready.");
        }
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Ad opened");
        };

        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Ad closed");
            LoadRewardedAd(); // Load lại sau khi xem xong
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError($"Ad failed to show: {error}");
            LoadRewardedAd();
        };
    }

    private void AddCoins(int amount)
    {
        currentCoins += amount;
        UpdateCoinUI();
    }

    private void UpdateCoinUI()
    {
        coinText.text = $"Coins: {currentCoins}";
    }
}
