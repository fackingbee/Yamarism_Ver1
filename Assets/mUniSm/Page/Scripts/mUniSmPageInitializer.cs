using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mUniSm.Core;

/// <summary>
/// Initializer of mUniSm Page
/// </summary>
public class mUniSmPageInitializer : mMonoBehaviour {

   /// <summary>
   /// Initialize
   /// </summary>
   /// <returns></returns>
   public IEnumerator Start( ) {
      while( !mUniSmCore.IsInitialized ) yield return null;
#if ENABLE_OTHERSTORE
      mUniSmCore.Instance.AddSymbol( "ENABLE_OTHERSTORE" );
#elif ENABLE_DEVELOPER
      mUniSmCore.Instance.AddSymbol( "ENABLE_DEVELOPER" );
#endif
      while( !mUniSmCore.IsActivated ) yield return null;
#if UNITY_IOS || UNITY_IPHONE
      yield return mAssetCore.Initialize( AssetIOSAdapter.Instance );
#else
      yield return mAssetCore.Initialize( );
#endif
#if UNITY_WEBGL
      yield return mDatabaseCore.Initialize( DatabasePlayerprefsAdapter.Instance );
#else
#if ENABLE_FIREBASE
      yield return mDatabaseCore.Initialize( DatabaseFirebaseAdapter.Instance );
#elif ENABLE_PLAYERPREFS
      yield return mDatabaseCore.Initialize( DatabasePlayerprefsAdapter.Instance );
#else
      yield return mDatabaseCore.Initialize( DatabaseLocalAdapter.Instance );
#endif
#endif
#if UNITY_PURCHASING
      mScene.Load( mUniSmCore.Config.Module.PurchasingScene );
#endif
#if ENABLE_ADMOB
      if( !string.IsNullOrEmpty( mUniSmCore.Config.AdsGameID ) && !string.IsNullOrEmpty( mUniSmCore.Config.AdsUnitID ) ) {
         yield return mAdsCore.Initialize( AdsAdmobAdapter.Instance, mAdsOptions.Create( mUniSmCore.Config.AdsGameID, mUniSmCore.Config.AdsUnitID ) );
      }
#elif UNITY_ADS
      if( !string.IsNullOrEmpty( mUniSmCore.Config.AdsGameID ) && !string.IsNullOrEmpty( mUniSmCore.Config.AdsUnitID ) ) {
         yield return mAdsCore.Initialize( AdsUnityAdapter.Instance, mAdsOptions.Create( mUniSmCore.Config.AdsGameID, mUniSmCore.Config.AdsUnitID ) );
      }
#endif
#if ENABLE_LOCATION
#endif
#if ENABLE_PHOTON
#endif
   }

}
