using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour{
    void Update() {
        if (Input.anyKey) {
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
    }
}