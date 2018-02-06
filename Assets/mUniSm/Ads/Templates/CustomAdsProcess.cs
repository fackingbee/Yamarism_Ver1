#if UNITY_ADS || ENABLE_ADMOB
using System.Collections.Generic;
using UnityEngine;
using mUniSm.Ads;
/// <summary>
/// Ads process for user defines
/// </summary>
public static class CustomAdsProcess {
   /// <summary>
   /// Reward process
   /// </summary>
   /// <param name="result">result</param>
   /// <param name="options">options</param>
   /// <returns>mTask</returns>
   public static mTask RewardProcess( mAdsShowResult result, mAdsOptions options ) {
      if( result == mAdsShowResult.Finished ) {
         return mTask.CreateEmpty( );
      } else {
         return mTask.CreateEmpty( );
      }
   }
}
#endif
