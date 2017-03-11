using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ChangeScene : MonoBehaviour {

    public void changeToScene(int changeTheScene) {
        SceneManager.LoadScene(changeTheScene);
    }

    public void quitScene() {
        Application.Quit();
    }
	
}
