using UnityEngine;
using System.Collections;
using KetosGames.SceneTransition;

namespace KetosGames.SceneTransition.Example {
	
    public class GoScript : MonoBehaviour {

		public TransitionDemo transitionDemo;
        public string ToScene;

        public void GoToNextScene() {
			transitionDemo.OnStartAnimation ();
			Invoke ("OnSceneLoadStart", 0.5f);
            //SceneLoader.LoadScene(ToScene);
        }

		public void OnSceneLoadStart(){
			SceneLoader.LoadScene(ToScene);

		}
    }
}
