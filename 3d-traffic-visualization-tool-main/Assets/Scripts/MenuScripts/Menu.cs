using SFB;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public TMPro.TextMeshProUGUI executablePathText;
    public TMPro.TextMeshProUGUI sumoConfigPathText;

    public Button playButton;

    public static string executablePath;
    public static string configPath;
    public static bool showTrees;
    public static bool showBuildings;
    public static bool showElevation;

    void Start()
    {
        executablePath = "sumo";
        configPath = "";
        showTrees = true;
        showBuildings = true;
        showElevation = true;
        executablePathText.text = executablePath;
        sumoConfigPathText.text = configPath;
    }

    public void playSimulation()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void updatePlayButtonInteractivity()
    {
        playButton.interactable = executablePath != "" && configPath != "";
    }

    public void selectSumoExecutable()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Select sumo executable", "", "exe", false);
        if (paths.Length > 0)
        {
            executablePath = paths[0];
            executablePathText.text = executablePath;
            updatePlayButtonInteractivity();
        }

    }

    public void selectSumoConfiguration()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Select sumo configuration", "", "sumocfg", false);
        if (paths.Length > 0)
        {
            configPath = paths[0];
            sumoConfigPathText.text = configPath;
            updatePlayButtonInteractivity();
        }

    }

    public void toggleTrees()
    {
        showTrees = !showTrees;
    }

    public void toggleBuildings()
    {
        showBuildings = !showBuildings;
    }

    public void toggleElevation()
    {
        showElevation = !showElevation;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
