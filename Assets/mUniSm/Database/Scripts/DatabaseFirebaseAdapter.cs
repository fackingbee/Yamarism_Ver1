#if ENABLE_FIREBASE
using mUniSm.Database;
using Firebase;
using System.Collections.Generic;
using Firebase.Unity.Editor;
using UnityEngine;
using System.Runtime.InteropServices;

/// <summary>
/// DatabaseAdapter for Firebase
/// </summary>
public sealed class DatabaseFirebaseAdapter : IDatabaseAdapter {

   private static DatabaseFirebaseAdapter _instance;
   public static DatabaseFirebaseAdapter Instance {
      get {
         if( _instance == null ) _instance = new DatabaseFirebaseAdapter( );
         return _instance;
      }
   }

   private FirebaseApp _app;
   public FirebaseApp App {
      get {
         if( this._app == null ) this._app = Firebase.FirebaseApp.DefaultInstance;
         return this._app;
      }
      private set { this._app = value; }
   }
   private Firebase.Database.FirebaseDatabase _database;
   public Firebase.Database.FirebaseDatabase Database {
      get {
         if( this._database == null ) this._database = Firebase.Database.FirebaseDatabase.DefaultInstance;
         return this._database;
      }
      private set { this._database = value; }
   }
   private Firebase.Auth.FirebaseAuth _auth;
   public Firebase.Auth.FirebaseAuth Auth {
      get {
         if( this._auth == null ) this._auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
         return this._auth;
      }
      private set { this._auth = value; }
   }
   private Firebase.Auth.FirebaseUser _user;
   public Firebase.Auth.FirebaseUser User {
      get {
         if( this._user == null && this.Auth != null ) this._user = this.Auth.CurrentUser;
         return this._user;
      }
   }
#if UNITY_IOS || UNITY_IPHONE && !UNITY_EDITOR
   [DllImport( "__Internal" )]
   private static extern void OpenGoogleSignIn_IOS( );
   [DllImport( "__Internal" )]
   private static extern void OpenGoogleSignOut_IOS( );
#endif
#if UNITYANDROID && !UNITY_EDITOR
   private static Google.Auth.GoogleAuth GoogleAuth;
   private static bool InitGoogleAuth( mDatabaseAuth auth ) {
      if( GoogleAuth != null ) return true;
      try {
         GoogleAuth = new Google.Auth.GoogleAuth( ) {
            OnSuccessSignIn = mEventManager.Instance.OnLoginSuccessReceived,
            OnErroSignIn = mEventManager.Instance.OnLoginFailedReceived,
            OnSignOut = mEventManager.Instance.OnLogoutReceived,
            OnRevoke = mEventManager.Instance.OnLogoutReceived
         };
         GoogleAuth.InitAuth( );
         return true;
      } catch( System.Exception e ) { auth.Error( mException.Core.Exception( e.Message ) ); }
      return false;
   }
#endif

   private System.Action<string> _tokenCallback;
   public bool Initialize( mDatabaseCore core, mDatabaseOptions options, System.Action<mDatabaseCore> callback ) {
      bool isError = false;
      DependencyStatus fbDependencyStatus = FirebaseApp.CheckDependencies( );
      if( fbDependencyStatus != DependencyStatus.Available ) {
         FirebaseApp.FixDependenciesAsync( ).ContinueWith( task => {
            fbDependencyStatus = FirebaseApp.CheckDependencies( );
            if( fbDependencyStatus == DependencyStatus.Available ) {
               callback( core );
            } else {
               isError = true;
            }
         } );
      } else {
         callback( core );
      }
      return !isError;
   }
   public bool IsRequiredAuth( ) => true;
   public bool SetOptions( mDatabaseOptions options ) {
      Firebase.AppOptions appoptions = new AppOptions( ) {
         ApiKey = options == null || string.IsNullOrEmpty( options.appKey ) ? Firebase.FirebaseApp.DefaultInstance.Options.ApiKey : options.appKey,
         DatabaseUrl = options == null || string.IsNullOrEmpty( options.databaseUrl ) ? Firebase.FirebaseApp.DefaultInstance.Options.DatabaseUrl : new System.Uri( options.databaseUrl ),
         AppId = options == null || string.IsNullOrEmpty( options.appID ) ? Firebase.FirebaseApp.DefaultInstance.Options.AppId : options.appID,
         MessageSenderId = options == null || string.IsNullOrEmpty( options.messageSenderID ) ? Firebase.FirebaseApp.DefaultInstance.Options.MessageSenderId : options.messageSenderID,
         StorageBucket = options == null || string.IsNullOrEmpty( options.strageBucket ) ? Firebase.FirebaseApp.DefaultInstance.Options.StorageBucket : options.strageBucket
      };
      this.App = options == null || string.IsNullOrEmpty( options.databaseName ) ? FirebaseApp.Create( appoptions ) : FirebaseApp.Create( appoptions, options.databaseName );
      this.Auth = Firebase.Auth.FirebaseAuth.GetAuth( this.App );
      this.Database = Firebase.Database.FirebaseDatabase.GetInstance( this.App );
      this.App.SetEditorDatabaseUrl( options.databaseUrl );
      if( mUtil.Core.IsEditor( ) ) {
         this.App.SetEditorP12FileName( options == null || string.IsNullOrEmpty( options.editorP12FileName ) ? Firebase.FirebaseApp.DefaultInstance.GetEditorP12FileName( ) : options.editorP12FileName );
         this.App.SetEditorServiceAccountEmail( options == null || string.IsNullOrEmpty( options.editorServiceAccountEmail ) ? Firebase.FirebaseApp.DefaultInstance.GetEditorServiceAccountEmail( ) : options.editorServiceAccountEmail );
         this.App.SetEditorP12Password( "notasecret" );
         this.App.SetEditorAuthUserId( options == null || string.IsNullOrEmpty( options.editorAuthUserID ) ? Firebase.FirebaseApp.DefaultInstance.GetEditorAuthUserId( ) : options.editorAuthUserID );
      }
      if( this.Auth == null || this.Database == null ) {
         return false;
      }
      if( mUtil.Core.IsEditor( ) ) {
         this.Database.PurgeOutstandingWrites( );
      }
      return true;
   }
   public void SetPushSetting( ) {
      Firebase.Invites.FirebaseInvites.InviteReceived += OnInviteReceived;
      Firebase.Invites.FirebaseInvites.InviteNotReceived += OnInviteNotReceived;
      Firebase.Invites.FirebaseInvites.ErrorReceived += OnErrorReceived;
   }
   private static void OnInviteReceived( object sender, Firebase.Invites.InviteReceivedEventArgs e ) {
      if( e.DeepLink != null ) {
         Debug.Log( "####[FB]#### Invite received: Deep Link: " + e.DeepLink );
         mEventManager.Instance.OnURLSchemeReceived( e.DeepLink.ToString( ) );
      }
   }
   private static void OnInviteNotReceived( object sender, System.EventArgs e ) { Debug.Log( "####[FB]#### No Invite or Deep Link received on start up" ); }
   private static void OnErrorReceived( object sender, Firebase.Invites.InviteErrorReceivedEventArgs e ) { Debug.LogError( "####[FB]#### Error occurred received the invite: " + e.ErrorMessage ); }
   public string GetUserUID( ) {
      if( this.User == null ) return null;
      return this.User.UserId;
   }
#if UNITYANDROID && !UNITY_EDITOR
   public bool InitializeGoogle( mDatabaseAuth auth ) { return InitGoogleAuth( auth ); }
#else
   public bool InitializeGoogle( mDatabaseAuth auth ) { return true; }
#endif
   public void GoogleSignInInternal( string UID ) {
#if UNITY_IOS || UNITY_IPHONE && !UNITY_EDITOR
      OpenGoogleSignIn_IOS( );
#elif UNITYANDROID && !UNITY_EDITOR
      GoogleAuth.SignIn( );
#else
      LoginStatus = Status.GoogleLogedin;
      AccessToken = UID;
#endif
   }
   public void GoogleSignOutInternal( ) {
#if UNITY_IOS || UNITY_IPHONE && !UNITY_EDITOR
      OpenGoogleSignOut_IOS( );
#elif UNITYANDROID && !UNITY_EDITOR
      GoogleAuth.SignOut( );
#else
      LoginStatus = Status.NotLogin;
      AccessToken = null;
#endif
   }
   public void GoogleLogin( string token, System.Action successCallback, System.Action<string> faultedCallback ) {
      Firebase.Auth.Credential credential = null;
      try {
         if( Application.platform == RuntimePlatform.Android ) {
            credential = Firebase.Auth.GoogleAuthProvider.GetCredential( token, null );
         } else {
            string[] tokens = token.Split( ";"[0] );
            credential = Firebase.Auth.GoogleAuthProvider.GetCredential( tokens[0], tokens[1] );
         }
      } catch { credential = null; };
      if( credential == null ) {
         faultedCallback( mException.Core.Null( "Credential data" ) );
         return;
      }
      this.Auth.SignInWithCredentialAsync( credential ).ContinueWith(
         ( task ) => {
            try {
               if( task.IsCompleted && !task.IsCanceled && !task.IsFaulted ) successCallback( );
               else faultedCallback( mException.Core.Fault( "Authentication process" ) );
            } catch( System.Exception e ) {
               faultedCallback( mException.Core.Exception( e.Message ) );
            }
         }
      );
   }
   public void GoogleLink( string token, System.Action successCallback, System.Action<string> faultedCallback ) {
      Firebase.Auth.Credential credential = null;
      try {
         if( Application.platform == RuntimePlatform.Android ) {
            credential = Firebase.Auth.GoogleAuthProvider.GetCredential( token, null );
         } else {
            string[] tokens = token.Split( ";"[0] );
            credential = Firebase.Auth.GoogleAuthProvider.GetCredential( tokens[0], tokens[1] );
         }
      } catch { credential = null; };
      if( credential == null ) {
         faultedCallback( mException.Core.Null( "Credential data" ) );
         return;
      }
      this.User.LinkWithCredentialAsync( credential ).ContinueWith(
         ( task ) => {
            try {
               if( task.IsCompleted && !task.IsCanceled && !task.IsFaulted ) successCallback( );
               else faultedCallback( mException.Core.Fault( "Authentication process" ) );
            } catch( System.Exception e ) {
               faultedCallback( mException.Core.Exception( e.Message ) );
            }
         }
      );
   }
   public void AnonymousLogin( string token, System.Action successCallback, System.Action<string> faultedCallback ) {
      this.Auth.SignInAnonymouslyAsync( ).ContinueWith(
         ( task ) => {
            try {
               if( task.IsCompleted && !task.IsCanceled && !task.IsFaulted ) successCallback( );
               else faultedCallback( mException.Core.Fault( "Authentication process" ) );
            } catch( System.Exception e ) {
               faultedCallback( mException.Core.Exception( e.Message ) );
            }
         }
      );
   }
   public void Logout( ) {
      this.Auth.SignOut( );
   }

   public void SetMessagingInit( System.Func<SystemLanguage, string> getUserTopicCallback, System.Action<string> getTokenCallback ) {
      this._tokenCallback = getTokenCallback;
      try {
         string topic = getUserTopicCallback( Application.systemLanguage );
         if( !string.IsNullOrEmpty( topic ) ) Firebase.Messaging.FirebaseMessaging.Subscribe( topic );
      } catch( System.Exception e ) { Debug.LogError( mException.Core.Exception( e.Message ) ); }
      Firebase.Messaging.FirebaseMessaging.TokenReceived += this.OnTokenReceived;
      Firebase.Messaging.FirebaseMessaging.MessageReceived += this.OnMessageReceived;
   }
   private void OnTokenReceived( object sender, Firebase.Messaging.TokenReceivedEventArgs token ) {
      if( this._tokenCallback == null ) return;
      this._tokenCallback( token.Token );
   }
   private void OnMessageReceived( object sender, Firebase.Messaging.MessageReceivedEventArgs arg ) {
      if( arg == null || arg.Message == null || arg.Message.Data == null || arg.Message.Data.Count <= 0 ) return;
      Debug.Log( "####[FB]#### Received a new message from: " + arg.Message.From + " " + arg.Message.RawData );
      try {
         mEventManager.Instance.OnPushReceived( (Dictionary<string, string>)arg.Message.Data );
      } catch( System.Exception e ) { Debug.LogError( mException.Core.Exception( e.Message ) ); };
   }

   public object CreateItemUpdatingEvent( string path, System.Action<Dictionary<string, object>> successCallback, System.Action<string> faultedCallback ) {
      System.EventHandler<Firebase.Database.ValueChangedEventArgs> handler = ( object sender, Firebase.Database.ValueChangedEventArgs task ) => {
         try {
            if( task.Snapshot != null && task.Snapshot.Value != null ) {
               successCallback( task.Snapshot.Value as Dictionary<string, object> );
            } else {
               successCallback( null );
            }
         } catch( System.Exception e ) {
            faultedCallback( mException.Core.Exception( e.Message ) );
         }
      };
      return (object)handler;
   }
   public object CreateListUpdatingEvent( string path, System.Action<List<mDatabaseItem>> successCallback, System.Action<string> faultedCallback ) {
      System.EventHandler<Firebase.Database.ValueChangedEventArgs> handler = ( object sender, Firebase.Database.ValueChangedEventArgs task ) => {
         try {
            if( task.Snapshot != null && task.Snapshot.Children != null && task.Snapshot.ChildrenCount > 0 ) {
               List<mDatabaseItem> list = mPoolList<mDatabaseItem>.Get( );
               foreach( Firebase.Database.DataSnapshot snap in task.Snapshot.Children ) {
                  if( snap == null ) continue;
                  list.Add( mDatabaseItem.Create( path + "/" + snap.Key, snap.Value as Dictionary<string, object> ) );
               }
               successCallback( list );
            } else {
               successCallback( null );
            }
         } catch( System.Exception e ) {
            faultedCallback( mException.Core.Exception( e.Message ) );
         }
      };
      return (object)handler;
   }
   public void SetUpdatingEvent( string path, mDatabaseQuery query, object eventHandler ) {
      ( query != null ? BuildQueryInternal( query, this.Database.GetReference( path ) ) : this.Database.GetReference( path ) ).ValueChanged +=(System.EventHandler<Firebase.Database.ValueChangedEventArgs>)eventHandler;
   }
   public void UnsetUpdatingEvent( string path, object eventHandler ) {
      this.Database.GetReference( path ).ValueChanged -= (System.EventHandler<Firebase.Database.ValueChangedEventArgs>)eventHandler;
   }

   public void GetItemValue( string path, System.Action<Dictionary<string, object>> successCallback, System.Action<string> faultedCallback ) {
      this.Database.GetReference( path ).GetValueAsync( ).ContinueWith(
         ( task ) => {
            try {
               if( task.IsCompleted ) {
                  if( task.Result != null && task.Result.Value != null ) {
                     successCallback( task.Result.Value as Dictionary<string, object> );
                  } else {
                     successCallback( null );
                  }
               } else {
                  faultedCallback( mException.Core.Null( "Data from: " + path ) );
               }
            } catch( System.Exception e ) {
               faultedCallback( mException.Core.Exception( e.Message ) );
            }
         }
      );
   }
   public void GetListValue( string path, mDatabaseQuery query, System.Action<List<mDatabaseItem>> successCallback, System.Action<string> faultedCallback ) {
      ( query != null ? BuildQueryInternal( query, this.Database.GetReference( path ) ) : this.Database.GetReference( path ) ).GetValueAsync( ).ContinueWith(
         ( task ) => {
            try {
               if( task.IsCompleted ) {
                  if( task.Result != null && task.Result.Children != null && task.Result.ChildrenCount > 0 ) {
                     List<mDatabaseItem> list = mPoolList<mDatabaseItem>.Get( );
                     foreach( Firebase.Database.DataSnapshot snap in task.Result.Children ) {
                        if( snap == null ) continue;
                        list.Add( mDatabaseItem.Create( path + "/" + snap.Key, snap.Value as Dictionary<string, object> ) );
                     }
                     successCallback( list );
                  } else {
                     successCallback( null );
                  }
               } else {
                  faultedCallback( mException.Core.Null( "Data from: " + path ) );
               }
            } catch( System.Exception e ) {
               faultedCallback( mException.Core.Exception( e.Message ) );
            }
         }
      );
   }
   public void UpdateValue( string path, Dictionary<string, object> data, System.Action successCallback, System.Action<string> faultedCallback ) {
      this.Database.GetReference( path ).UpdateChildrenAsync( data ).ContinueWith(
         ( task ) => {
            try {
               if( !task.IsCompleted ) {
                  faultedCallback( mException.Core.Null( "Data from: " + path ) );
               } else {
                  successCallback( );
               }
            } catch( System.Exception e ) {
               faultedCallback( mException.Core.Exception( e.Message ) );
            }
         }
      );
   }
   public void ClearListValue( List<string> paths, System.Action successCallback, System.Action<string> faultedCallback ) {
      List<string> tmp = mPoolList<string>.Get( );
      foreach( string path in paths ){
         tmp.Add( path );
         this.Database.GetReference( path ).SetValueAsync( null ).ContinueWith(
            ( task ) => {
               try {
                  tmp.Remove( path );
                  if( tmp.Count > 0 ) return;
                  mPoolList<string>.Release( tmp );
                  if( !task.IsCompleted ) {
                     faultedCallback( mException.Core.Fault( "Clearing data at: " + path ) );
                  } else {
                     successCallback( );
                  }
               } catch( System.Exception e ) {
                  faultedCallback( mException.Core.Exception( e.Message ) );
               }
            }
         );
      }
   }
   public void ClearItemValue( string path, System.Action successCallback, System.Action<string> faultedCallback ) {
      this.Database.GetReference( path ).SetValueAsync( null ).ContinueWith(
         ( task ) => {
            try {
               if( !task.IsCompleted ) {
                  faultedCallback( mException.Core.Fault( "Clearing data at: " + path ) );
               } else {
                  successCallback( );
               }
            } catch( System.Exception e ) {
               faultedCallback( mException.Core.Exception( e.Message ) );
            }
         }
      );
   }

   private Firebase.Database.Query BuildQueryInternal( mDatabaseQuery query, Firebase.Database.DatabaseReference reference ) {
      if( query.mode == mDatabaseQuery.Mode.IDList && query.idList != null && query.idList.Count > 0 ) {
         return (Firebase.Database.Query)reference;
      } else if( string.IsNullOrEmpty( query.key ) ) {
         return (Firebase.Database.Query)reference;
      } else {
         Firebase.Database.Query fquery = reference.OrderByChild( query.key );
         if( query.queryType == mDatabaseQuery.QueryType.EqualTo ) {
            if( query.value01 == null ) {
            } else if( query.value01 is string ) {
               fquery = fquery.EqualTo( (string)query.value01 );
            } else if( query.value01 is double ) {
               fquery = fquery.EqualTo( (double)query.value01 );
            } else if( query.value01 is bool ) {
               fquery = fquery.EqualTo( (bool)query.value01 );
            }
         } else if( query.queryType == mDatabaseQuery.QueryType.HigherThan ) {
            fquery = fquery.StartAt( (double)query.value01 );
         } else if( query.queryType == mDatabaseQuery.QueryType.LowerThan ) {
            fquery = fquery.EndAt( (double)query.value01 );
         } else if( query.queryType == mDatabaseQuery.QueryType.Range ) {
            fquery = fquery.StartAt( (double)query.value01 ).EndAt( (double)query.value02 );
         }
         if( query.orderBy == mDatabaseQuery.OrderBy.Asc ) {
            fquery = fquery.LimitToFirst( query.limit );
         } else if( query.orderBy == mDatabaseQuery.OrderBy.Desc ) {
            fquery = fquery.LimitToLast( query.limit );
         }
         return fquery;
      }
   }
}
#endif