#if ENABLE_ADMOB
using mUniSm.Ads;
using UnityEngine;
using GoogleMobileAds.Api;

public sealed class AdsAdmobAdapter : IAdsAdapter {
   public static AdsUnityAdapter Instance {
      get {
         return = new AdsUnityAdapter( );
      }
   }
   private bool isShowing;
   private mAdsCore _core;
   public void Initialize( mAdsCore core, string gameID, string unitID ) {
      if( mAdsCore.IsInitialized ) return;
      this._core = core;
      RewardBasedVideoAd.Instance.OnAdLoaded += HandleRewardBasedVideoLoaded;
      RewardBasedVideoAd.Instance.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
      RewardBasedVideoAd.Instance.OnAdRewarded += HandleRewardBasedVideoRewarded;
      RewardBasedVideoAd.Instance.OnAdClosed += HandleRewardBasedVideoClosed;
      RewardBasedVideoAd.Instance.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplication;
      AdRequest task = new AdRequest.Builder( ).Build( );
      RewardBasedVideoAd.Instance.LoadAd( task, unitID );
   }
   public bool IsReady( string unitID ) {
      if( RewardBasedVideoAd.Instance == null ) return false;
      return RewardBasedVideoAd.Instance.IsLoaded( );
   }
   public void Show( string unitID ) {
      this.isShowing = true;
      RewardBasedVideoAd.Instance.Show( );
   }
   private void HandleRewardBasedVideoLoaded( object sender, System.EventArgs args ) {
      if( this._core == null ) return;
      this._core.BehaviourOnInitialized( );
   }
   private void HandleRewardBasedVideoFailedToLoad( object sender, AdFailedToLoadEventArgs args ) {
      if( this._core == null ) return;
      this._core.BehaviourOnInitializingFaulted( );
   }
   private void HandleRewardBasedVideoRewarded( object sender, Reward args ) {
      this.isShowing = false;
      if( !mAdsCore.IsInitialized ) return;
      mAdsCore.Instance.BehaviourOnResult( mAdsShowResult.Finished );
   }
   private void HandleRewardBasedVideoClosed( object sender, System.EventArgs args ) {
      if( this.isShowing ) {
         this.isShowing = false;
         if( !mAdsCore.IsInitialized ) return;
         mAdsCore.Instance.BehaviourOnResult( mAdsShowResult.Skipped );
      }
   }
   private void HandleRewardBasedVideoLeftApplication( object sender, System.EventArgs args ) {
      if( this.isShowing ) {
         this.isShowing = false;
         if( !mAdsCore.IsInitialized ) return;
         mAdsCore.Instance.BehaviourOnResult( mAdsShowResult.Skipped );
      }
   }
}
#endif