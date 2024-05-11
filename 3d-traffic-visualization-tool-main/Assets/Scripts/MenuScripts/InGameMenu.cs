using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    public void returnToMenu() {
        SceneManager.LoadScene(0);
    }
}
