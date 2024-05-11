using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    public GameObject[] buildings;
    public void SpawnBuildings()
    {
        var parent = new GameObject("Buildings");
        foreach (var e in SumoNetworkVisualizer.edges.Values)
        {
            if (e.getLanes().Count > 0)
            {
                var width = (float)e.getLanes().Select(l => l.width).Sum();
                var lane = e.getLanes()[0];
                var previousBuildingPos = new Vector3();
                if (lane.length > 3 && lane.shape.Count > 3)
                {
                    for (int i = 3; i < lane.shape.Count; i += 3)
                    {
                        var pos = lane.shape[i];
                        var posV = new Vector3((float)pos[0], 0, (float)pos[1]);
                        if ((posV - previousBuildingPos).magnitude < 15)
                        {
                            continue;
                        }
                        previousBuildingPos = posV;
                        var prevPos = lane.shape[i - 1];
                        var difV = posV - new Vector3((float)prevPos[0], 0, (float)prevPos[1]);
                        var buildingPos = posV - new Vector3(-difV.z, 0, difV.x).normalized * (width / 2.0f + 15f);
                        if (Random.value < 0.8f)
                        {
                            continue;
                        }
                        if (!Physics.CheckSphere(buildingPos, 15))
                        {
                            var randomIndex = UnityEngine.Random.Range(0, buildings.Length);
                            var building = buildings[randomIndex];
                            var angle = Vector3.Angle(Vector3.right, difV);
                            var cross = Vector3.Cross(Vector3.right, difV);
                            if (cross.y < 0) angle = -angle;
                            var go = Object.Instantiate(building, buildingPos, Quaternion.identity, parent.transform);
                            go.transform.localEulerAngles = new Vector3(0, angle - 90, 0);
                        }

                    }

                }
            }

        }
    }
}
