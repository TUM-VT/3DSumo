using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Assets.Scripts.SUMOImporter.NetFileComponents;
using UnityEngine;

public class SumoNetworkVisualizer : MonoBehaviour
{
    static GameObject network;

    public static float minY;

    public static Dictionary<string, NetFileJunction> junctions;
    public static Dictionary<string, NetFileLane> lanes;
    public static Dictionary<string, NetFileEdge> edges;
    public static Dictionary<string, Shape> shapes;

    public static Dictionary<string, NetFileTrafficLightSystem> trafficLightSystems;

    public static float xmin;
    public static float xmax;
    public static float ymin;
    public static float ymax;

    public Material roadMaterial;
    public Material junctionMaterial;
    public Material bicycleMaterial;
    public Material pavementMaterial;
    public Material railMaterial;


    public UnityTrafficLight trafficLightObj;

    public void generateNetwork()
    {
        parseXMLfiles();
        drawStreetNetwork();
    }

    public void parseXMLfiles()
    {

        network = new GameObject("StreetNetwork");
        network.transform.position = new Vector3(0, 0.01f, 0);

        string executablePath = Menu.executablePath;
        string configPath = Menu.configPath;

        configurationType configuration;
        XmlSerializer serializer1 = new XmlSerializer(typeof(configurationType));
        FileStream fs1 = new FileStream(configPath, FileMode.OpenOrCreate);
        TextReader rd1 = new StreamReader(fs1);
        configuration = (configurationType)serializer1.Deserialize(rd1);
        string netFileName = configuration.input.netfile.value;



        string netFilePath = Path.GetDirectoryName(configPath) + "\\" + netFileName;

        lanes = new Dictionary<string, NetFileLane>();
        edges = new Dictionary<string, NetFileEdge>();
        junctions = new Dictionary<string, NetFileJunction>();
        shapes = new Dictionary<string, Shape>();
        trafficLightSystems = new Dictionary<string, NetFileTrafficLightSystem>();

        netType netFile;
        XmlSerializer serializer = new XmlSerializer(typeof(netType));
        FileStream fs = new FileStream(netFilePath, FileMode.OpenOrCreate);
        TextReader rd = new StreamReader(fs);
        netFile = (netType)serializer.Deserialize(rd);

        foreach (junctionType junction in netFile.junction)
        {
            if (junction.type != junctionTypeType.@internal)
            {
                NetFileJunction j = new NetFileJunction(junction.id, junction.type, float.Parse(junction.x), float.Parse(junction.y), junction.incLanes, junction.intLanes, junction.shape);

                if (!junctions.ContainsKey(j.id))
                    junctions.Add(j.id, j);

                foreach (var lane in j.incLanes)
                {
                    if (!lanes.ContainsKey(lane.id))
                        lanes.Add(lane.id, lane);
                }
                foreach (var lane in j.intLanes)
                {
                    if (!lanes.ContainsKey(lane.id))
                        lanes.Add(lane.id, lane);
                }
            }
        }

        foreach (edgeType edge in netFile.edge)
        {
            if (!edge.functionSpecified || true)
            {
                var to = edge.to != null ? junctions[edge.to] : null;
                var from = edge.from != null ? junctions[edge.from] : null;

                NetFileEdge e = new NetFileEdge(edge.id, from, to, edge.priority, edge.shape, edge.function);

                if (to != null)
                {
                    to.incEdges.Add(e);
                }

                if (!edges.ContainsKey(edge.id))
                    edges.Add(edge.id, e);

                if (edge.Items != null)
                {
                    foreach (var l in edge.Items)
                    {
                        if (l.GetType().ToString() == "laneType")
                        {
                            laneType lane = (laneType)l;
                            if (lanes.ContainsKey(lane.id))
                            {
                                e.addLane(lanes[lane.id], lane.index, lane.speed, lane.length, lane.widthSpecified ? lane.width : 3.2f, lane.shape, lane.allow);
                            }
                        }
                    }
                }
            }
        }

        if (netFile.connection != null)
        {
            foreach (connectionType connection in netFile.connection)
            {
                NetFileTrafficLightSystem trafficLightSystem;
                if (connection.tl != null)
                {
                    if (trafficLightSystems.TryGetValue(connection.tl, out trafficLightSystem))
                    {
                        trafficLightSystem.AddTrafficLight(new NetFileTrafficLight(int.Parse(connection.linkIndex), edges[connection.from], connection.dir.ToString().ToLower()));
                    }
                    else
                    {
                        var tlSystem = new NetFileTrafficLightSystem(connection.tl);
                        tlSystem.AddTrafficLight(new NetFileTrafficLight(int.Parse(connection.linkIndex), edges[connection.from], connection.dir.ToString().ToLower()));
                        trafficLightSystems.Add(connection.tl, tlSystem);
                    }
                }

            }
        }


        // Get map boundaries
        string[] boundaries = netFile.location.convBoundary.Split(',');
        xmin = float.Parse(boundaries[0]);
        ymin = float.Parse(boundaries[1]);
        xmax = float.Parse(boundaries[2]);
        ymax = float.Parse(boundaries[3]);
    }

    public void drawStreetNetwork()
    {
        minY = 99999.0f;
        if (Menu.showElevation)
        {
            foreach (NetFileEdge e in edges.Values)
            {
                foreach (NetFileLane l in e.getLanes())
                {
                    foreach (var shape in l.shape)
                    {
                        var z = (float)shape[2];
                        if (z < minY)
                        {
                            minY = z;
                        }
                    }
                }
            }
        }
        else
        {
            minY = 0;
        }

        foreach (NetFileEdge e in edges.Values)
        {
            if (e.function != edgeTypeFunction.@internal)
            {
                foreach (NetFileLane l in e.getLanes())
                {
                    var shapes = l.shape.Select(coord => new Vector3((float)coord[0], Menu.showElevation ? (float)coord[2] - minY : 0, (float)coord[1])).ToList();
                    var mesh = LineMeshGenerator.CreateLineMesh(shapes, (float)l.width);
                    var go = new GameObject(l.id);
                    go.transform.SetParent(network.transform);
                    go.transform.localPosition = new Vector3();
                    var mr = go.AddComponent<MeshRenderer>();
                    var mf = go.AddComponent<MeshFilter>();
                    mf.sharedMesh = mesh;
                    mr.material = roadMaterial;
                    if(l.allow != null){
                        if(l.allow == "bicycle"){
                            mr.material = bicycleMaterial;
                            go.transform.localPosition = new Vector3(0, 0.03f, 0);
                        }else if(l.allow.Contains("pedestrian")){
                            mr.material = pavementMaterial;
                            go.transform.localPosition = new Vector3(0, 0.02f, 0);
                        }else if(l.allow.Contains("rail") || l.allow.Contains("tram")){
                            mr.material = railMaterial;
                            go.transform.localPosition = new Vector3(0, 0.04f, 0);
                        }
                    }
                   
                    var mc = go.AddComponent<MeshCollider>();
                    mc.sharedMesh = mesh;

                }
            }



        }

        foreach (NetFileJunction j in junctions.Values)
        {
            if (j.shape.Count > 2)
            {
                List<int> indices = new List<int>();

                Vector2[] vertices2D = new Vector2[j.shape.Count];
                for (int i = 0; i < j.shape.Count; i++)
                {
                    vertices2D[i] = new Vector2((float)(j.shape[i])[0], (float)(j.shape[i])[1]);
                }

                Triangulator tr = new Triangulator(vertices2D);
                List<int> bottomIndices = new List<int>(tr.Triangulate());
                indices.AddRange(bottomIndices);

                Vector3[] vertices = new Vector3[vertices2D.Length];
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] = new Vector3(vertices2D[i].x,  Menu.showElevation ? (float)(j.shape[i])[2] - minY : 0, vertices2D[i].y);
                }

                Mesh mesh = new Mesh();
                mesh.Clear();
                mesh.vertices = vertices;
                mesh.triangles = indices.ToArray();
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();

                Bounds bounds = mesh.bounds;
                Vector2[] uvs = new Vector2[vertices.Length];
                for (int i = 0; i < vertices.Length; i++)
                {
                    uvs[i] = new Vector2(vertices[i].x / bounds.size.x, vertices[i].z / bounds.size.z);
                }
                mesh.uv = uvs;

                GameObject junction3D = new GameObject(j.id);
                MeshRenderer r = (MeshRenderer)junction3D.AddComponent(typeof(MeshRenderer));
                var collider = junction3D.AddComponent<MeshCollider>();
                r.material = junctionMaterial;
                MeshFilter filter = junction3D.AddComponent(typeof(MeshFilter)) as MeshFilter;
                filter.mesh = mesh;
                collider.sharedMesh = mesh;
                junction3D.transform.SetParent(network.transform);
                junction3D.transform.localPosition = new Vector3(0, 0, 0);

                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
            }
        }


        var trafficLightsParent = new GameObject("Traffic Lights");
        foreach (var tlSystem in trafficLightSystems.Values)
        {
            var go = new GameObject(tlSystem.id);
            go.name = tlSystem.id;
            go.transform.SetParent(trafficLightsParent.transform);
            var tlSystemComp = go.AddComponent<UnityTrafficLightSystem>();
            tlSystemComp.lights = tlSystem.trafficLights;
            tlSystemComp.trafficLightObj = trafficLightObj;
        };

    }

    void OnApplicationQuit()
    {
        SumoRunner.sumoProcess?.Kill();
        Debug.Log("Application ending after " + Time.time + " seconds");
    }
}
