using System.Collections.Generic;
using Assets.Scripts.SUMOImporter.NetFileComponents;
using CodingConnected.TraCI.NET.Commands;
using UnityEngine;

public class UnityTrafficLightSystem : MonoBehaviour
{
    float time = 0;
    public string stateStr = "";
    public List<NetFileTrafficLight> lights;
    public Dictionary<string, UnityTrafficLight> lightObjects;

    public UnityTrafficLight trafficLightObj;

    void Start()
    {
        lightObjects = new Dictionary<string, UnityTrafficLight>();
        GenerateLights();
    }
    void Update()
    {
        TrafficLightCommands clientTrafficLight = SumoSpawner.clientTrafficLight;
        if (SumoSpawner.initialized && clientTrafficLight != null)
        {
            time += Time.deltaTime;
            if (time > 0.02f)
            {
                var clientState = clientTrafficLight.GetState(this.name);
                if (clientState != null)
                {
                    var state = clientTrafficLight.GetState(this.name).Content;
                    stateStr = state.ToLower();
                    var index = 0;
                    foreach (var tl in lights)
                    {
                        var go = lightObjects[tl.edge.getId()];
                        if (stateStr.Length > tl.linkIndex)
                        {
                            go.SwitchLight(tl.dir, stateStr[tl.linkIndex]);
                        }
                        index++;
                    }
                    time = 0;
                }
            }
        }
    }

    void GenerateLights()
    {
        foreach (var tl in lights)
        {
            if (!lightObjects.ContainsKey(tl.edge.getId()))
            {
                var shape = tl.edge.getLanes()[0].shape;
                var go = Object.Instantiate(trafficLightObj);
                go.dirs = new List<string>();
                go.dirs.Add(tl.dir);
                go.transform.SetParent(this.transform);
                var pos = shape[shape.Count - 1];
                var prevPos = shape[shape.Count - 2];
                var posV = new Vector3((float)pos[0], 0, (float)pos[1]);
                var prevPosV = new Vector3((float)prevPos[0], 0, (float)prevPos[1]);
                Vector3 direction = (posV - prevPosV).normalized;
                var angle = Vector3.Angle(Vector3.right, direction);
                var cross = Vector3.Cross(Vector3.right, direction);
                if (cross.y < 0) angle = -angle;
                Vector3 normal = new Vector3(-direction.z, 0, direction.x);
                go.transform.position = new Vector3(posV.x, Menu.showElevation ? 2 + (float)pos[2] - SumoNetworkVisualizer.minY : 2, posV.z) - normal * (float)tl.edge.getLanes()[0].width / 2;
                go.transform.localEulerAngles = new Vector3(0, angle - 160, 0);
                lightObjects[tl.edge.getId()] = go;
            }
            else
            {
                lightObjects[tl.edge.getId()].dirs.Add(tl.dir);
            }

        }
    }
}
