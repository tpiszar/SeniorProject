using UnityEngine;
using UnityEngine.SceneManagement;

namespace kroland.fantasymonsters
{
    public class SimpleSceneManager : MonoBehaviour
    {
        public string nextSceneName = "";
        public string prevSceneName = "";
        private void Update() {
            if (Input.GetKeyDown(KeyCode.RightArrow)){
                nextScene();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow)){
                prevScene();
            }
        }
        public void nextScene(){
            SceneManager.LoadScene(nextSceneName);
        }
        public void prevScene(){
            SceneManager.LoadScene(prevSceneName);
        }
        public void changeScene(string sceneName){
            SceneManager.LoadScene(sceneName);
        }
    }
}