#if UNITY_ADS
using UnityEngine.Advertisements;
using mUniSm.Ads;
using System.Collections;

/// <summary>
/// AdsAdapter for Unity Ads
/// </summary>
public sealed class AdsUnityAdapter : IAdsAdapter {
   public static AdsUnityAdapter Instance {
      get {
         return new AdsUnityAdapter( );
      }
   }
   public void Initialize( mAdsCore core, string gameID, string unitID ) {
      if( mAdsCore.IsInitialized ) return;
      mTaskScheduler.Instance.AddMainThreadNotCleared( this.InitializingProcess( core, gameID, unitID ) );
   }
   private IEnumerator InitializingProcess( mAdsCore core, string gameID, string unitID ) {
      Advertisement.Initialize( gameID );
      if( Advertisement.isInitialized ) {
         if( mUtil.Core.IsEditor( ) ) {
            while( !Advertisement.IsReady( ) ) yield return null;
         } else {
            while( !Advertisement.IsReady( string.IsNullOrEmpty( unitID ) ? "video" : unitID ) ) yield return null;
         }
      }
      core.BehaviourOnInitialized( );
   }
   public bool IsReady( string unitID ) {
      if( mUtil.Core.IsEditor( ) ) {
         return Advertisement.isInitialized && Advertisement.IsReady( );
      } else {
         return Advertisement.isInitialized && Advertisement.IsReady( string.IsNullOrEmpty( unitID ) ? "video" : unitID );
      }
   }
   public void Show( string unitID ) {
      if( mUtil.Core.IsEditor( ) ) {
         Advertisement.Show( new ShowOptions { resultCallback = this.HandleShowResult } );
      } else {
         Advertisement.Show( string.IsNullOrEmpty( unitID ) ? "video" : unitID, new ShowOptions { resultCallback = this.HandleShowResult } );
      }
   }
   private void HandleShowResult( ShowResult result ) {
      if( !mAdsCore.IsInitialized ) return;
      mAdsCore.Instance.BehaviourOnResult( ( mAdsShowResult )(int)result );
   }
}
#endif
