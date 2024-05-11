using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    public GameObject[] trees;

    public void spawnTrees()
    {
        var treeContainer = new GameObject("Trees");

        float boundXmin = SumoNetworkVisualizer.xmin - 500;
        float boundXmax = SumoNetworkVisualizer.xmax + 500;
        float boundYmin = SumoNetworkVisualizer.ymin - 500;
        float boundYmax = SumoNetworkVisualizer.ymax + 500;

        int treeCount = (int)(Mathf.Abs(boundXmax - boundXmin) * Mathf.Abs(boundYmax - boundYmin)) / 500;

        for (var i = 0; i < treeCount; i++)
        {
            var randomIndex = UnityEngine.Random.Range(0, trees.Length);
            var tree = trees[randomIndex];

            var x = Random.Range(boundXmin, boundXmax);
            var z = Random.Range(boundYmin, boundYmax);
            var pos = new Vector3(x, 0, z);

            bool validSpawnPoint = !IsPointInsideMesh(pos);

            if (validSpawnPoint)
            {
                var treeObj = Object.Instantiate(tree, pos, Quaternion.identity);
                treeObj.transform.SetParent(treeContainer.transform);
            }

        }
    }

    private bool IsPointInsideMesh(Vector3 point)
    {
        return Physics.CheckSphere(new Vector3(point.x, 3, point.z), 10);
    }
}
