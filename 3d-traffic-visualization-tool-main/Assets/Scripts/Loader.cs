using System.Linq;
using System.Threading;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public SumoNetworkVisualizer networkVisualizer;
    public SumoSpawner sumoSpawner;
    public TreeSpawner treeSpawner;
    public BuildingSpawner buildingSpawner;

    public CameraController cameraController;
    void Start()
    {
        SumoRunner.RunSumo();
        Thread.Sleep(3000);
        networkVisualizer.generateNetwork();

        var pos = SumoNetworkVisualizer.lanes.Values.ToList()[SumoNetworkVisualizer.lanes.Count / 2].shape[1];

        cameraController.transform.position = new Vector3((float)pos[0], 10, (float)pos[1]);

        if (Menu.showBuildings)
        {
            buildingSpawner.SpawnBuildings();
        }
        if (Menu.showTrees)
        {
            treeSpawner.spawnTrees();
        }

        sumoSpawner.StartClient();
    }

    void OnDestroy()
    {
        if (SumoSpawner.client != null)
        {
            SumoSpawner.client.Dispose();
        }
    }
}


