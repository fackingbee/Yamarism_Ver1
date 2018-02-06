#if UNITY_IOS || UNITY_IPHONE
using mUniSm.Asset;
/// <summary>
/// Asset adapter for Unity
/// </summary>
public class AssetIOSAdapter : IAssetAdapter {
   public static AssetIOSAdapter Instance {
      get {
         return new AssetIOSAdapter( );
      }
   }
   public void Initialize( ) { }
   public void SetNoBackupFlag( string path ) {
      UnityEngine.iOS.Device.SetNoBackupFlag( path );
   }
}
#endif
