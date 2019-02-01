using UnityEngine;
using System;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using UnityEngine.UI;

public class AdmobController : MonoBehaviour {
    private BannerView bannerView;
    public InterstitialAd interstitial;
    public RewardBasedVideoAd rewardBasedVideo;

    [SerializeField] string androidAppId;
    [SerializeField] string iosAppId;

    [Header("Interstitial")]
    public bool usingInterstitial;
    [SerializeField] string androidInterstitial;
    [SerializeField] string iosInterstitial;

    [Header("Banner")]
    public bool usingBannerAds;
    [SerializeField] string androidBanner;
    [SerializeField] string iosBanner;
    bool loadBannerSuccessfully;

    [Header("Rewarded Video")]
    public bool usingVideoAds;
    [SerializeField] string androidRewardedVideo;
    [SerializeField] string iosRewardedVideo;

    bool showVideoSucessfully;
    private System.Action onShowBasedVideoRewardedFinished, onCloseVideoRewarded;
    private System.Action onShowInterstitialFinished, onCloseInterstitial;

    public static AdmobController instance{
        get
        {
            return ins;
        }
    }
    private static AdmobController ins;

    private void Awake() {
        if (ins != null && ins != this){
            Destroy(this.gameObject);
            return;
        }
        ins = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start() {
        // Initialize the Google Mobile Ads SDK.

        string _appId = string.Empty;
#if UNITY_ANDROID
        _appId = androidAppId.Trim();
#elif UNITY_IOS
        _appId = iosAppId.Trim();
#else
        _appId = "unexpected_platform";
#endif
        MobileAds.Initialize(_appId);

        InitBanner();
        InitInterstitial();
        InitRewardedVideo();

        RequestBanner();
        RequestInterstitial();
        RequestRewardBasedVideo();
    }

    private void InitBanner(){
        if(!usingBannerAds){
            return;
        }

        // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = androidBanner.Trim();
#elif UNITY_IOS
        string adUnitId = iosBanner.Trim();
#else
        string adUnitId = "unexpected_platform";
#endif

        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);

        // Register for ad events.
        bannerView.OnAdLoaded += HandleAdLoaded;
        bannerView.OnAdFailedToLoad += HandleAdFailedToLoad;
        bannerView.OnAdOpening += HandleAdOpened;
        bannerView.OnAdClosed += HandleAdClosed;
        bannerView.OnAdLeavingApplication += HandleAdLeftApplication;
    }

    public void InitInterstitial() {
        if(!usingInterstitial){
            return;
        }

         // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = androidInterstitial.Trim();
#elif UNITY_IOS
        string adUnitId = iosInterstitial.Trim();
#else
        string adUnitId = "unexpected_platform";
#endif

        // Create an interstitial.
        interstitial = new InterstitialAd(adUnitId);

        // Register for ad events.
        interstitial.OnAdLoaded += HandleInterstitialLoaded;
        interstitial.OnAdFailedToLoad += HandleInterstitialFailedToLoad;
        interstitial.OnAdOpening += HandleInterstitialOpened;
        interstitial.OnAdClosed += HandleInterstitialClosed;
        interstitial.OnAdLeavingApplication += HandleInterstitialLeftApplication;
    }

    private void InitRewardedVideo() {
        if(!usingVideoAds){
            return;
        }

        // Get singleton reward based video ad reference.
        rewardBasedVideo = RewardBasedVideoAd.Instance;

        // RewardBasedVideoAd is a singleton, so handlers should only be registered once.
        rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;
        rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
        rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoOpened;
        rewardBasedVideo.OnAdStarted += HandleRewardBasedVideoStarted;
        rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
        rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
        rewardBasedVideo.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplication;
    }

    public void RequestBanner() {
        if(!usingBannerAds){
            return;
        }
        // Load an banner ad.
        bannerView.LoadAd(CreateAdRequest());
    }

    public void RequestInterstitial() {
        if(!usingInterstitial){
            return;
        }

        // Load an interstitial ad.
        interstitial.LoadAd(CreateAdRequest());
    }

    public void RequestRewardBasedVideo() {
        if(!usingVideoAds){
            return;
        }

#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = androidRewardedVideo.Trim();
#elif UNITY_IOS
        string adUnitId = iosRewardedVideo.Trim();
#else
        string adUnitId = "unexpected_platform";
#endif

        rewardBasedVideo.LoadAd(CreateAdRequest(), adUnitId);
    }

    // Returns an ad request with custom ad targeting.
    private AdRequest CreateAdRequest() {
        return new AdRequest.Builder()
                // .AddTestDevice(AdRequest.TestDeviceSimulator)
                // .AddTestDevice("0123456789ABCDEF0123456789ABCDEF")
                // .AddKeyword("game")
                // .TagForChildDirectedTreatment(false)
                // .AddExtra("color_bg", "9B30FF")
                .Build();
    }

    // public void ShowInterstitial(InterstitialAd ad) {
    //     if(!usingInterstitial){
    //         return;
    //     }

    //     if (ad != null && ad.IsLoaded()) {
    //         ad.Show();
    //     }
    // }

    public void ShowBanner() {
        if(!usingBannerAds){
            return;
        }

        if (bannerView != null) {
            if(loadBannerSuccessfully){
                bannerView.Show();
            }else{
                #if TEST
                Debug.LogError("Load Banner Again");
                #endif
                RequestBanner();
            }
        } else {
            #if TEST
            Debug.LogError("(Show) BannerView is not ready yet");
            #endif
            InitBanner();
            RequestBanner();
        }
    }

    public void HideBanner() {
        if(!usingBannerAds){
            return;
        }

        if (bannerView != null) {
            bannerView.Hide();
            if(!loadBannerSuccessfully){
                #if TEST
                Debug.LogError("Load Banner Again");
                #endif
                RequestBanner();
            }
        }else {
            #if TEST
            Debug.LogError("(Hide) BannerView is not ready yet");
            #endif
            InitBanner();
            RequestBanner();
        }
    }

    public void ShowInterstitial(System.Action _onShowFinished = null, System.Action _onClosed = null) {
        if(!usingInterstitial){
            return;
        }

        if (interstitial.IsLoaded()) {
            onShowInterstitialFinished = _onShowFinished;
            onCloseInterstitial = _onClosed;
            #if UNITY_EDITOR
            if(onShowInterstitialFinished != null){
                onShowInterstitialFinished();
                onShowInterstitialFinished = null;
            }
            if(onCloseInterstitial != null){
                onCloseInterstitial();
                onCloseInterstitial = null;
            }
            #else
            interstitial.Show();
            #endif
        }else{
            #if TEST
            Debug.LogError("Interstitial is not ready yet");
            #endif
            RequestInterstitial();
            onShowInterstitialFinished = null;
            onCloseInterstitial = null;
        }
    }

    public void ShowRewardBasedVideo(System.Action _onShowFinished = null, System.Action _onClosed = null) {
        if(!usingVideoAds){
            return;
        }

        if (rewardBasedVideo.IsLoaded()) {
            showVideoSucessfully = false;
            onShowBasedVideoRewardedFinished = _onShowFinished;
            onCloseVideoRewarded = _onClosed;
            #if UNITY_EDITOR
            if(onShowBasedVideoRewardedFinished != null){
                onShowBasedVideoRewardedFinished();
                onShowBasedVideoRewardedFinished = null;
            }
            if(onCloseVideoRewarded != null){
                onCloseVideoRewarded();
                onCloseVideoRewarded = null;
            }
            #else
            rewardBasedVideo.Show();
            #endif
        } else {
            #if TEST
            Debug.LogError("Reward based video ad is not ready yet");
            #endif
            RequestRewardBasedVideo();
            showVideoSucessfully = false;
            onShowBasedVideoRewardedFinished = null;
            onCloseVideoRewarded = null;
        }
    }

    #region Banner callback handlers
    public void HandleAdLoaded(object sender, EventArgs args) {
        if(!usingBannerAds){
            return;
        }
        Debug.Log("HandleAdLoaded event received.");
        loadBannerSuccessfully = true;
    }

    public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args) {
        if(!usingBannerAds){
            return;
        }
        Debug.Log("HandleFailedToReceiveAd event received with message: " + args.Message);
        loadBannerSuccessfully = false;
    }

    public void HandleAdOpened(object sender, EventArgs args) {
        if(!usingBannerAds){
            return;
        }
        Debug.Log("HandleAdOpened event received");
    }

    public void HandleAdClosed(object sender, EventArgs args) {
        if(!usingBannerAds){
            return;
        }
        Debug.Log("HandleAdClosed event received");
    }

    public void HandleAdLeftApplication(object sender, EventArgs args) {
        if(!usingBannerAds){
            return;
        }
        Debug.Log("HandleAdLeftApplication event received");
    }

    #endregion

    #region Interstitial callback handlers
    public void HandleInterstitialLoaded(object sender, EventArgs args) {
        if(!usingInterstitial){
            return;
        }
        Debug.Log("HandleInterstitialLoaded event received.");
    }

    public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args) {
        if(!usingInterstitial){
            return;
        }
        Debug.Log("HandleInterstitialFailedToLoad event received with message: " + args.Message);
    }

    public void HandleInterstitialOpened(object sender, EventArgs args) {
        if(!usingInterstitial){
            return;
        }
        Debug.Log("HandleInterstitialOpened event received");
    }

    public void HandleInterstitialClosed(object sender, EventArgs args) {
        if(!usingInterstitial){
            return;
        }
        Debug.Log("HandleInterstitialClosed event received");
        RequestInterstitial();
        if(onShowInterstitialFinished != null){
            onShowInterstitialFinished();
            onShowInterstitialFinished = null;
        }
        if(onCloseInterstitial != null){
            onCloseInterstitial();
            onCloseInterstitial = null;
        }
    }

    public void HandleInterstitialLeftApplication(object sender, EventArgs args) {
        if(!usingInterstitial){
            return;
        }
        Debug.Log("HandleInterstitialLeftApplication event received");
    }

    #endregion

    #region RewardBasedVideo callback handlers
    public void HandleRewardBasedVideoLoaded(object sender, EventArgs args) {
        if(!usingVideoAds){
            return;
        }
        Debug.Log("HandleRewardBasedVideoLoaded event received");
    }

    public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args) {
        if(!usingVideoAds){
            return;
        }
        Debug.Log(
            "HandleRewardBasedVideoFailedToLoad event received with message: " + args.Message);
    }

    public void HandleRewardBasedVideoOpened(object sender, EventArgs args) {
        if(!usingVideoAds){
            return;
        }
        Debug.Log("HandleRewardBasedVideoOpened event received");
    }

    public void HandleRewardBasedVideoStarted(object sender, EventArgs args) {
        if(!usingVideoAds){
            return;
        }
        Debug.Log("HandleRewardBasedVideoStarted event received");
    }

    public void HandleRewardBasedVideoClosed(object sender, EventArgs args) {
        if(!usingVideoAds){
            return;
        }
        RequestRewardBasedVideo();
        Debug.Log("HandleRewardBasedVideoClosed event received");
        if(showVideoSucessfully){
            if(onShowBasedVideoRewardedFinished != null){
                onShowBasedVideoRewardedFinished();
                onShowBasedVideoRewardedFinished = null;
            }
            showVideoSucessfully = false;
        }
        if(onCloseVideoRewarded != null){
            onCloseVideoRewarded();
            onCloseVideoRewarded = null;
        }
    }

    public void HandleRewardBasedVideoRewarded(object sender, Reward args) {
        if(!usingVideoAds){
            return;
        }
        string type = args.Type;
        double amount = args.Amount;
        Debug.Log(
            "HandleRewardBasedVideoRewarded event received for " + amount.ToString() + " " + type);
        showVideoSucessfully = true;
    }

    public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args) {
        if(!usingVideoAds){
            return;
        }
        Debug.Log("HandleRewardBasedVideoLeftApplication event received");
    }

    #endregion
}
