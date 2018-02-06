#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
#define LEGACY_LEVEL_LOADER
#endif

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
#if !LEGACY_LEVEL_LOADER
using UnityEngine.SceneManagement;
#endif

namespace KetosGames.SceneTransition
{
    public class SceneLoader : MonoBehaviour
    {
        private static SceneLoader SceneLoaderInstance;
        
        public GameObject LoadingScreen;
        public Image FadeImage;
        [Tooltip("Use with VR (For best results, use a loading scene or a solid color loading screen with VR.")]
        public bool VRMode = false;
        [Tooltip("When checked, use the Loading scene as the Loading screen (instead of the Loading UI).")]
        public bool UseSceneForLoadingScreen = true;
        [Tooltip("The name of the Loading scene to load.")]
        public string LoadingSceneName = "Loading";
        [Tooltip("When checked, fade in the loading screen.")]
        public bool FadeInLoadingScreen = true;
        [Tooltip("When checked, fade out the loading screen.")]
        public bool FadeOutLoadingScreen = true;
        [Tooltip("The number of seconds to animate the fade.")]
        public float FadeSeconds = 1f;
        [Tooltip("The number of seconds to show the loading screen after fade in. Set it to 0 to go to the new scene as soon as it's ready.")]
        public float MinimumLoadingScreenSeconds = 1f;
        [Tooltip("The color to use in the fade animation.")]
        public Color FadeColor = Color.black;

        private AsyncOperation SceneLoadingOperation;

        private bool FadingIn = true;
        private bool FadingOut = false;
        private float FadeTime = 0;
        private Color FadeClearColor;
        private bool Loading = false;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static SceneLoader Instance
        {
            get
            {
                if (SceneLoaderInstance == null)
                {
                    SceneLoader sceneLoader = (SceneLoader)GameObject.FindObjectOfType(typeof(SceneLoader));
                    if (sceneLoader != null)
                    {
                        SceneLoaderInstance = sceneLoader;
                    }
                    else
                    {
                        GameObject SceneLoaderPrefab = Resources.Load<GameObject>("KetosGames/SceneTransition/Prefabs/SceneLoader");
                        SceneLoaderInstance = (GameObject.Instantiate(SceneLoaderPrefab)).GetComponent<SceneLoader>();
                    }
                }
                return SceneLoaderInstance;
            }
        }

        /// <summary>
        /// Loads a scene.
        /// </summary>
        /// <param name="name">Name of the scene to load</param>
        public static void LoadScene(string name)
        {
            Instance.Load(name);
        }

        /// <summary>
        /// Awake
        /// </summary>
        public void Awake()
        {
            Object.DontDestroyOnLoad(this.gameObject);

            // Get rid of any old SceneLoaders
            if (SceneLoaderInstance != null && SceneLoaderInstance != this)
            {
                Destroy(SceneLoaderInstance.gameObject);
                SceneLoaderInstance = this;
            }
        }

        /// <summary>
        /// Start
        /// </summary>
        public void Start()
        {
            FadeImage.gameObject.SetActive(true);
            LoadingScreen.SetActive(false);
            BeginFadeIn();
        }

        /// <summary>
        /// Update 
        /// </summary>
        void Update()
        {
            if (FadeImage != null)
            {
                if (FadingIn)
                {
                    UpdateFadeIn();
                }
                else if (FadingOut)
                {
                    UpdateFadeOut();
                }
            }
        }

        /// <summary>
        /// Late Update 
        /// </summary>
        void LateUpdate()
        {
            if (VRMode && Camera.main != null && (FadingIn || FadingOut || Loading))
            {
                // Stay in front of the camera
                // This is preferred to making it a child of the camera due to the fact that the camera can get destroyed when the scene changes.
                transform.position = Camera.main.transform.position + (Camera.main.transform.forward * (Camera.main.nearClipPlane + 1));
                transform.LookAt(Camera.main.transform);
            }
        }

        /// <summary>
        /// Begins the fade out.
        /// </summary>
        public void BeginFadeOut()
        {
            UpdateCamera();
            if (FadingIn && FadeTime > 0)
            {
                FadeTime = 1 / FadeTime; // Reverse fade
            }
            else
            {
                FadeTime = 0;
                FadeImage.color = Color.clear;
            }
            FadeImage.enabled = true;
            FadingIn = false;
            FadingOut = true;
            FadeClearColor = FadeColor;
            FadeClearColor.a = 0;
        }

        /// <summary>
        /// Begins the fade in.
        /// </summary>
        public void BeginFadeIn()
        {
            UpdateCamera();
            if (FadingOut && FadeTime > 0)
            {
                FadeTime = 1 / FadeTime; // Reverse fade
            }
            else
            {
                FadeTime = 0;
                FadeImage.color = FadeColor;
            }
            FadeImage.enabled = true;
            FadingIn = true;
            FadingOut = false;
            FadeClearColor = FadeColor;
            FadeClearColor.a = 0;
        }

        /// <summary>
        /// Ends the fade in.
        /// </summary>
        private void EndFadeIn()
        {
            FadeImage.color = Color.clear;
            FadeImage.enabled = false;
            FadingIn = false;
        }

        /// <summary>
        /// Ends the fade out.
        /// </summary>
        private void EndFadeOut()
        {
            FadeImage.color = FadeColor;
            FadingOut = false;
        }
        
        /// <summary>
        /// Fade in as a scene is starting
        /// </summary>
        private void UpdateFadeIn()
        {
            FadeTime += Time.deltaTime / FadeSeconds;
            FadeImage.color = Color.Lerp(FadeColor, FadeClearColor, FadeTime);
            
            if (FadeTime > 1)
            {
                EndFadeIn();
            }
        }
        
        /// <summary>
        /// Fade out as a scene is ending
        /// </summary>
        private void UpdateFadeOut()
        {
            FadeTime += Time.deltaTime / FadeSeconds;
            FadeImage.color = Color.Lerp(FadeClearColor, FadeColor, FadeTime);
            
            if (FadeTime > 1)
            {
                EndFadeOut();
            }
        }
        
        /// <summary>
        /// Loads a scene
        /// </summary>
        /// <param name="name">Name of the scene to load</param>
        public void Load(string name)
        {
            if (!Loading)
            {
                StartCoroutine(InnerLoad(name));
            }
        }
        
        /// <summary>
        /// Coroutine for loading the scene
        /// </summary>
        /// <returns>The load.</returns>
        /// <param name="name">Name of the scene to load</param>
        IEnumerator InnerLoad(string name)
        {
            Loading = true;
            if (UseSceneForLoadingScreen)
            {
                // Fade out
                BeginFadeOut();
                while (FadingOut)
                {
                    yield return 0;
                }

                //Show loading scene
                #if LEGACY_LEVEL_LOADER
                Application.LoadLevel(LoadingSceneName);
                #else
                SceneManager.LoadScene(LoadingSceneName);
                #endif

                yield return 0;

                //Start load the level we want in the background
                #if LEGACY_LEVEL_LOADER
                SceneLoadingOperation = Application.LoadLevelAsync(name);
                #else
                SceneLoadingOperation = SceneManager.LoadSceneAsync(name);
                #endif
                SceneLoadingOperation.allowSceneActivation = false;
            }
            else
            {
                //Start load the level we want in the background
                #if LEGACY_LEVEL_LOADER
                SceneLoadingOperation = Application.LoadLevelAsync(name);
                #else
                SceneLoadingOperation = SceneManager.LoadSceneAsync(name);
                #endif
                SceneLoadingOperation.allowSceneActivation = false;

                // Fade out
                BeginFadeOut();
                while (FadingOut)
                {
                    yield return 0;
                }

                LoadingScreen.SetActive(true);
            }

            // Fade in
            if (FadeInLoadingScreen)
            {
                BeginFadeIn();
                while (FadingIn)
                {
                    if (MinimumLoadingScreenSeconds == 0f && SceneLoadingOperation.progress >= 0.9f)
                    {
                        break; // the scene is finished loading, lets get out of here
                    }
                    yield return 0;
                }
            }
            else
            {
                EndFadeIn();
            }

            yield return new WaitForSeconds(MinimumLoadingScreenSeconds);
            
            //Wait for the level to finish loading
            while (SceneLoadingOperation.progress < 0.9f)
            {
                yield return 0;
            }

            // Fade out
            if (FadeOutLoadingScreen)
            {
                BeginFadeOut();
                while (FadingOut)
                {
                    yield return 0;
                }
            }
            else
            {
                EndFadeOut();
            }

            SceneLoadingOperation.allowSceneActivation = true;
            
            while (!SceneLoadingOperation.isDone)
            {
                yield return 0;
            }
            LoadingScreen.SetActive(false);

            // Fade in
            BeginFadeIn();

            Loading = false; // At this point is should be safe to start a new load even though it's still fading in
        }

        /// <summary>
        /// Setup the sceneLoader canvas based on the VR Support
        /// </summary>
        private void UpdateCamera()
        {
            if (VRMode)
            {
                GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            }
            else
            {
                GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            }
        }
    }
}
