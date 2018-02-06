#if UNITY_PURCHASING
using UnityEngine;
using mUniSm.Purchasing;
using UnityEngine.Purchasing;
using System.Collections.Generic;
public sealed class PurchaseUnityAdapter : IPurchaseAdapter {

   public static PurchaseUnityAdapter Instance {
      get {
         return new PurchaseUnityAdapter( );
      }
   }

   private PurchaseUnityBehaviour _behaviour;
   public void Initialize( mPurchaseOptions options ) {
      if( mPurchaseCore.IsInitialized ) return;
      this._behaviour = mUniSmAppManager.FindObjectOfTypeAndInitialize<PurchaseUnityBehaviour>( );
      ConfigurationBuilder builder = ConfigurationBuilder.Instance( StandardPurchasingModule.Instance( ) );
      List<mPurchaseProductItem> products = options.itemDefine( );
      foreach( mPurchaseProductItem product in products ) {
         if( product == null ) continue;
         IDs ids = new IDs( );
         if( product.storeIDs != null ) {
            foreach( KeyValuePair<string, string> tmp in product.storeIDs ) {
               ids.Add( tmp.Key, tmp.Value );
            }
         }
         builder.AddProduct( product.ID, (ProductType)(int)product.productType, ids );
      }
      if( builder == null ) throw new System.NullReferenceException( );
      UnityPurchasing.Initialize( this._behaviour, builder );
   }
   public bool Purchase( mPurchaseProductItem define ) {
      Product product = this._behaviour.SearchProduct( define );
      if( product != null ) {
         if( Application.platform == RuntimePlatform.Android ) {
            this._behaviour.InitiatePurchase( product, mUtil.Core.GenerateCode( 16 ) );
         } else {
            this._behaviour.InitiatePurchase( product );
         }
         return true;
      }
      return false;
   }
   public void RestoreTransactions( ) {
      this._behaviour.RestoreTransaction( );
   }
   public void ConfirmPendingPurchase( mPurchaseProductItem define ) {
      this._behaviour.ConfirmPendingPurchase( define );
   }

   public string GetProductReceipt( mPurchaseProductItem define ) {
      Product p = this._behaviour.SearchProduct( define );
      if( p == null ) return null;
      return p.receipt;
   }
   public bool GetProductHasReceipt( mPurchaseProductItem define ) {
      Product p = this._behaviour.SearchProduct( define );
      if( p == null ) return false;
      return p.hasReceipt;
   }
   public string GetProductTransactionID( mPurchaseProductItem define ) {
      Product p = this._behaviour.SearchProduct( define );
      if( p == null ) return null;
      return p.transactionID;
   }
   public string GetProductName( mPurchaseProductItem define ) {
      Product p = this._behaviour.SearchProduct( define );
      if( p == null ) return null;
      return p.metadata.localizedTitle;
   }
   public string GetProductDescription( mPurchaseProductItem define ) {
      Product p = this._behaviour.SearchProduct( define );
      if( p == null ) return null;
      return p.metadata.localizedDescription;
   }
   public float GetProductPrice( mPurchaseProductItem define ) {
      Product p = this._behaviour.SearchProduct( define );
      if( p == null ) return 0;
      return (float)p.metadata.localizedPrice;
   }
   public string GetProductPriceText( mPurchaseProductItem define ) {
      Product p = this._behaviour.SearchProduct( define );
      if( p == null ) return null;
      return p.metadata.localizedPriceString;
   }
   public string GetProductStoreSpecificID( mPurchaseProductItem define ) {
      Product p = this._behaviour.SearchProduct( define );
      if( p == null ) return null;
      return p.definition.storeSpecificId;
   }
}
#endif
