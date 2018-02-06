using System.Collections.Generic;
using UnityEngine;
using mUniSm.Core;
using System.Collections;
#if UNITY_PURCHASING
using UnityEngine.Purchasing;

/// <summary>
/// Systems supporting UnityPurchase
/// Initialization, item list display, purchase processing, restoration, subscription checking can be done
/// Processing on server side is necessary for purchase
/// </summary>
public sealed class PurchaseUnityBehaviour : mMonoBehaviour, IStoreListener {

   /// <summary>
   /// Systems supporting UnityPurchase
   /// Initialization, item list display, purchase processing, restoration, subscription checking can be done
   /// Processing on server side is necessary for purchase
   /// </summary>
   private static PurchaseUnityBehaviour _instance;

   /// <summary>
   /// Systems supporting UnityPurchase
   /// Initialization, item list display, purchase processing, restoration, subscription checking can be done
   /// Processing on server side is necessary for purchase
   /// </summary>
   public static PurchaseUnityBehaviour Instance {
      get {
         if( _instance == null ) {
            _instance = MonoBehaviour.FindObjectOfType<PurchaseUnityBehaviour>( );
         }
         return _instance;
      }
   }
   private PurchaseUnityBehaviour( ) { }

   /// <summary>
   /// Charging controller
   /// </summary>
   private IStoreController _controller;
   /// <summary>
   /// Charging provider
   /// </summary>
   private IExtensionProvider _provider;


   /// <summary>
   /// Billing item list
   /// </summary>
   [SerializeField]
   public mDataDefineList purchaseDefines;

   /// <summary>
   /// Purchase log path
   /// </summary>
   public string privateUserDataPath {
      get { return this._privateUserDataPath; }
      set {
         this._privateUserDataPath = value;
         if( string.IsNullOrEmpty( this._privateUserDataPath ) ) this._privateUserDataPath = "/User/Data/[ID]/Info";
      }
   }
   [SerializeField]
   private string _privateUserDataPath = "/User/Data/[ID]/Info";

   /// <summary>
   /// Purchase log path
   /// </summary>
   public string logPath {
      get { return this._logPath; }
      set {
         this._logPath = value;
         if( string.IsNullOrEmpty( this._logPath ) ) this._logPath = "/User/Data/[ID]/Log/Purchase";
      }
   }
   [SerializeField]
   private string _logPath = "/User/Data/[ID]/Log/Purchase";

   /// <summary>
   /// Purchase log path
   /// </summary>
   public string subscriptionPath {
      get { return this._subscriptionPath; }
      set {
         this._subscriptionPath = value;
         if( string.IsNullOrEmpty( this._subscriptionPath ) ) this._subscriptionPath = "/User/Data/[ID]/Plan/[Product]";
      }
   }
   [SerializeField]
   private string _subscriptionPath = "/User/Data/[ID]/Plan/[Product]";

   /// <summary>
   /// Prizm key
   /// </summary>
   [SerializeField]
   public string prizmKey = "Prizm";

   /// <summary>
   /// Purchased Time
   /// </summary>
   [SerializeField]
   public string purchaseTimeKey = "PurchasedTime";

   /// <summary>
   /// PurchasedAmount 
   /// </summary>
   [SerializeField]
   public string purchaseAmountKey = "PurchasedAmount";

   /// <summary>
   /// Purchase core
   /// </summary>
   public mPurchaseCore purchase { get; private set; }
   
   /// <summary>
   /// Get Purchase Products
   /// </summary>
   /// <returns></returns>
   private List<mPurchaseProductItem> GetPurchaseProducts( ) {
      List<mPurchaseProductItem> list = mPoolList<mPurchaseProductItem>.Get( );
      if( this.purchaseDefines == null || this.purchaseDefines.data == null ) return list;
      foreach( mDefineItem item in this.purchaseDefines.data ) {
         if( !item.ContainsKey( "ID" ) || !item.ContainsKey( "Type" ) ) continue;
         Dictionary<string, string> tmp = mPoolDictionary<string, string>.Get( );
         if( item.ContainsKey( GooglePlay.Name ) ) { tmp[GooglePlay.Name] = item.GetString( GooglePlay.Name ); }
         if( item.ContainsKey( AppleAppStore.Name ) ) { tmp[AppleAppStore.Name] = item.GetString( AppleAppStore.Name ); }
         if( item.ContainsKey( AmazonApps.Name ) ) { tmp[AmazonApps.Name] = item.GetString( AmazonApps.Name ); }
         list.Add( mPurchaseProductItem.Create( item.GetString( "ID" ), (mPurchaseProductType)item.GetInt( "Type" ), item.GetInt( "Value" ), tmp ) );
         mPoolDictionary<string, string>.Release( tmp );
      }
      return list;
   }

   /// <summary>
   /// Purchase apply process
   /// </summary>
   /// <param name="product">Purchasing product</param>
   /// <param name="infomation">Purchasing infomation</param>
   /// <param name="options">options</param>
   /// <returns></returns>
   private mTask ApplyPurchase( mPurchaseProductItem product, Dictionary<string, object> infomation, mPurchaseOptions options ) {
      if( string.IsNullOrEmpty( this.privateUserDataPath ) ) return mTask.CreateEmpty( );
      if( mUtil.Core.IsEditor( ) ) {
         return mDatabaseItem.Request( this.privateUserDataPath.Replace( "[ID]", mDatabaseAuth.Instance.ID ) ).OnTaskSuccess(
            r => {
               System.DateTime dateTime = mUtil.Core.UnixTime2DateTime( r.GetLong( this.purchaseTimeKey, 0 ) );
               r[this.prizmKey] = r.GetInt( this.prizmKey, 0 ) + product.value;
               r[this.purchaseAmountKey] = dateTime.Month != System.DateTime.UtcNow.Month ? product.value : ( r.GetInt( this.purchaseAmountKey ) + product.value );
               r[this.purchaseTimeKey] = mUtil.Core.DateTime2UnixTime( );
               return r.Save( );
            }
         );
      } else {
      }
      return mTask.CreateEmpty( );
   }

   /// <summary>
   /// Subscription apply process
   /// </summary>
   /// <param name="product">Purchasing product</param>
   /// <param name="infomation">Purchasing infomation</param>
   /// <param name="options">options</param>
   /// <returns></returns>
   private mTask ApplySubscription( mPurchaseProductItem product, string token, mPurchaseOptions options ) {
      if( mUtil.Core.IsEditor( ) ) {
      } else {
      }
      return mTask.CreateEmpty( );
   }
   /// <summary>
   /// User purchasing log data path
   /// </summary>
   /// <param name="ID">ID</param>
   /// <returns></returns>
   private string UserPurchasingLogPath( string ID ) {
      return ( string.IsNullOrEmpty( this.logPath ) ? "/User/Data/[ID]/Log/Purchase" : this.logPath ).Replace( "[ID]", ID );
   }
   /// <summary>
   /// User subscription list path
   /// </summary>
   /// <param name="ID">ID</param>
   /// <returns></returns>
   private string UserSubscriptionListPath( string ID ) {
      return this.UserSubscriptionItemPath( ID, "" ).Replace( "//", "/" ).TrimEnd( '/' );
   }
   /// <summary>
   /// User subscription list path
   /// </summary>
   /// <param name="ID">ID</param>
   /// <returns></returns>
   private string UserSubscriptionItemPath( string ID, string product ) {
      return ( string.IsNullOrEmpty( this.logPath ) ? "/User/Data/[ID]/Plan/[Product]" : this.logPath ).Replace( "[ID]", ID ).Replace( "[Product]", product );
   }


   /// <summary>
   /// OnStart
   /// </summary>
   /// <returns></returns>
   private IEnumerator Start( ) {
      while( this.purchaseDefines == null || this.purchaseDefines.data == null ) yield return mUtil.Core.IntervalShort;
      while( !mUniSmCore.IsInitialized || !mDatabaseCore.IsInitialized || !mPlayerprefsCore.IsInitialized || !mAssetCore.IsInitialized || !mDatabaseAuth.IsInitialized || !mDatabaseAuth.IsLoggedin ) yield return mUtil.Core.IntervalShort;
      DontDestroyOnLoad( this );
      this.purchase = mPurchaseCore.Initialize(
         PurchaseUnityAdapter.Instance,
         mPurchaseOptions.Create(
            mUniSmCore.Config.PackageName,
            this.GetPurchaseProducts, this.ApplyPurchase, this.ApplySubscription,
            this.UserPurchasingLogPath, this.UserSubscriptionListPath, this.UserSubscriptionItemPath
         )
      );
   }

   /// <summary>
   /// Product search
   /// </summary>
   /// <param name="product">mPurchaseProductItem item</param>
   /// <returns>product</returns>
   public Product SearchProduct( mPurchaseProductItem product ) {
      if( !mPurchaseCore.IsInitialized ) return null;
      if( product == null || this._controller == null || this._controller.products == null ) return null;
      Product[] products = this._controller.products.all;
      for( int i = 0; i < products.Length; i++ ) {
         Product p = products[i];
         if( p == null ) continue;
         if( p.definition.id == product.ID ) return p;
      }
      return null;
   }
   /// <summary>
   /// Product search
   /// </summary>
   /// <param name="product">Product item</param>
   /// <returns>product</returns>
   public mPurchaseProductItem SearchProduct( Product product ) {
      if( !mPurchaseCore.IsInitialized ) return null;
      if( product == null || this._controller == null || this._controller.products == null ) return null;
      foreach( mPurchaseProductItem p in mPurchaseCore.Instance.ProductList ) {
         if( p == null ) continue;
         if( p.ID == product.definition.id ) return p;
      }
      return null;
   }
   /// <summary>
   /// Start purchasing
   /// </summary>
   /// <param name="product">product</param>
   /// <param name="developerPayload">payload</param>
   public void InitiatePurchase( Product product, string developerPayload = null ) {
      if( !mPurchaseCore.IsInitialized ) return;
      if( this._controller == null || product == null ) return;
      this._controller.InitiatePurchase( product, developerPayload );
   }
   
   /// <summary>
   /// Billing system initialization
   /// </summary>
   /// <param name="controller">Store controller</param>
   /// <param name="extensions">Extensions</param>
   public void OnInitialized( IStoreController controller, IExtensionProvider extensions ) {
      if( controller == null || extensions == null ) return;
      this._controller = controller;
      this._provider = extensions;
      mPurchaseCore.Instance.BehaviourOnInitialized( );
   }


   /// <summary>
   /// Restore transaction
   /// </summary>
   public void RestoreTransaction( ) {
      if( !mPurchaseCore.IsInitialized ) return;
      this._provider.GetExtension<IAppleExtensions>( ).RestoreTransactions( result => {
         if( result ) {
         } else {
         }
      } );
   }
   
   /// <summary>
   /// Processing after completion of purchase
   /// </summary>
   /// <param name="e">PurchaseEventArgs</param>
   /// <returns>PurchaseProcessingResult</returns>
   public PurchaseProcessingResult ProcessPurchase( PurchaseEventArgs e ) {
      if( !mPurchaseCore.IsInitialized || e == null ) return PurchaseProcessingResult.Pending;
      mPurchaseProductItem product = this.SearchProduct( e.purchasedProduct );
      if( product == null ) return PurchaseProcessingResult.Pending;
      if( e.purchasedProduct.definition.type == ProductType.Consumable ) {
         mPurchaseCore.Instance.OnPurchased( product, mPurchaseCore.Instance.GetProductInfomation( product ) );
      } else if( e.purchasedProduct.definition.type == ProductType.Subscription ) {
         mPurchaseCore.Instance.OnPurchased( product, mPurchaseCore.Instance.GetProductInfomation( product ) );
      }
      return PurchaseProcessingResult.Complete;
   }

   /// <summary>
   /// Billing Initialization Failed
   /// </summary>
   /// <param name="error">error contents</param>
   public void OnInitializeFailed( InitializationFailureReason error ) {
      if( !mPurchaseCore.IsInitialized ) return;
      mPurchaseCore.Instance.BehaviourOnInitializeFailed( error.ToString() );
   }

   /// <summary>
   /// Charging failure processing
   /// </summary>
   /// <param name="product">product</param>
   /// <param name="reason">reason</param>
   public void OnPurchaseFailed( Product product, PurchaseFailureReason reason ) {
      if( !mPurchaseCore.IsInitialized ) return;
      mPurchaseCore.Instance.BehaviourOnPurchaseFailed( reason.ToString( ) );
   }
   /// <summary>
   /// Confirm Pending Purchase
   /// </summary>
   /// <param name="product">product</param>
   public void ConfirmPendingPurchase( mPurchaseProductItem product ) {
      if( !mPurchaseCore.IsInitialized ) return;
      Product p = this.SearchProduct( product );
      if( p == null ) return;
      this._controller.ConfirmPendingPurchase( p );
      UnityPurchasing.ClearTransactionLog( );
   }
   

}
#else
/// <summary>
/// Systems supporting UnityPurchase
/// Initialization, item list display, purchase processing, restoration, subscription checking can be done
/// Processing on server side is necessary for purchase
/// </summary>
public sealed class PurchaseUnityBehaviour : mMonoBehaviour {

   /// <summary>
   /// Billing item list
   /// </summary>
   [SerializeField]
   public mDataDefineList purchaseDefines;

   /// <summary>
   /// Purchase modal scene
   /// </summary>
   [SerializeField]
   public string PurchaseModalScene = "PurchaseModal";

   /// <summary>
   /// Purchase log path
   /// </summary>
   public string logPath {
      get { return this._logPath; }
      set {
         this._logPath = value;
         if( string.IsNullOrEmpty( this._logPath ) ) this._logPath = "/User/Data/[ID]/Log/Purchase";
      }
   }
   [SerializeField]
   private string _logPath = "/User/Data/[ID]/Log/Purchase";

   /// <summary>
   /// Purchase log path
   /// </summary>
   public string subscriptionPath {
      get { return this._subscriptionPath; }
      set {
         this._subscriptionPath = value;
         if( string.IsNullOrEmpty( this._subscriptionPath ) ) this._subscriptionPath = "/User/Data/[ID]/Plan/[Product]";
      }
   }
   [SerializeField]
   private string _subscriptionPath = "/User/Data/[ID]/Plan/[Product]";

}
#endif