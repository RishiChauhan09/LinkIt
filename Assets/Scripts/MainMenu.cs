using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public void ChangeScene(string name) {
        SceneManager.LoadScene(name);
    }

}