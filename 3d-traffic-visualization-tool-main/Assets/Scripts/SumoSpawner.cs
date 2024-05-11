using System.Collections.Generic;
using UnityEngine;
using CodingConnected.TraCI.NET;
using CodingConnected.TraCI.NET.Commands;

public class SumoSpawner : MonoBehaviour
{
    public static TraCIClient client;
    private Queue<GameObject> carPool = new Queue<GameObject>();
    private Queue<GameObject> bicyclePool = new Queue<GameObject>();
    private Dictionary<string, GameObject> vehicles = new Dictionary<string, GameObject>();

    public GameObject[] vehiclePrefabs;
    public GameObject bicyclePrefab;
    public static ControlCommands clientControl;
    public static VehicleCommands clientVehicle;
    public static TrafficLightCommands clientTrafficLight;


    private int carPoolSize = 500;
    private int bicyclePoolSize = 50;

    float time = 0;

    public static bool initialized = false;

    public void StartClient()
    {
        client = new TraCIClient();
        client.Connect("127.0.0.1", 4001);
        clientControl = client.Control;
        clientVehicle = client.Vehicle;
        clientTrafficLight = client.TrafficLight;

        var Vehicles = new GameObject("Vehicles");

        for (int i = 0; i < carPoolSize; i++)
        {
            var randomIndex = UnityEngine.Random.Range(0, vehiclePrefabs.Length);
            var vehiclePrefab = vehiclePrefabs[randomIndex];
            GameObject vehicle = Instantiate(vehiclePrefab, new Vector3(0, -20, 0), Quaternion.identity);
            vehicle.transform.SetParent(Vehicles.transform);
            vehicle.SetActive(false);
            carPool.Enqueue(vehicle);
        }

        for (int i = 0; i < bicyclePoolSize; i++)
        {
            GameObject bicycle = Instantiate(bicyclePrefab, new Vector3(0, -20, 0), Quaternion.identity);
            bicycle.transform.SetParent(Vehicles.transform);
            bicycle.SetActive(false);
            bicyclePool.Enqueue(bicycle);
        }

        initialized = true;
    }

    void FixedUpdate()
    {
        if (initialized)
        {
            clientControl.SimStep();
        }
    }

    void Update()
    {
        if (initialized)
        {
            time += Time.deltaTime;
            if (time > 0.02f)
            {
                var idRequest = clientVehicle.GetIdList();

                if (idRequest != null)
                {
                    var vehicleIds = idRequest.Content;

                    foreach (var id in vehicleIds)
                    {
                        if (!vehicles.ContainsKey(id))
                        {
                            var type = clientVehicle.GetTypeID(id).Content;
                            GameObject vehicle;
                            if(type == "bicycle"){
                                vehicle = GetBicycleFromPool();
                            }else{
                                vehicle = GetCarFromPool();
                            }
                            
                            vehicle.name = id;
                            vehicle.SetActive(true);
                            vehicles.Add(id, vehicle);
                        }
                    }
                }
                time = 0;
            }
        }
    }

    private GameObject GetCarFromPool()
    {
        if (carPool.Count > 0)
        {
            return carPool.Dequeue();
        }
        else
        {
            var randomIndex = UnityEngine.Random.Range(0, vehiclePrefabs.Length);
            var vehiclePrefab = vehiclePrefabs[randomIndex];
            var newVehicle = Instantiate(vehiclePrefab);
            newVehicle.SetActive(false);
            return newVehicle;
        }
    }

    private GameObject GetBicycleFromPool()
    {
        if (bicyclePool.Count > 0)
        {
            return bicyclePool.Dequeue();
        }
        else
        {
            var newBicycle = Instantiate(bicyclePrefab);
            newBicycle.SetActive(false);
            return newBicycle;
        }
    }
}