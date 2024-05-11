using CodingConnected.TraCI.NET.Commands;
using UnityEngine;

public class SumoVehicle : MonoBehaviour
{
    float time = 0;
    Vector3 position = new Vector3(0, 0, 0);
    VehicleCommands clientVehicle = SumoSpawner.clientVehicle;

    void Update()
    {
        time += Time.deltaTime;
        if (time > 0.02f)
        {
            var pos = clientVehicle.GetPosition3D(this.name).Content;
            var angle = clientVehicle.GetAngle(this.name).Content;

            if (pos == null)
            {
                var childCamera = GetComponentInChildren<Camera>();
                if (childCamera != null)
                {
                    childCamera.gameObject.transform.SetParent(null);
                }
                Destroy(this.gameObject);
            }
            else
            {
                position.x = (float)pos.X;
                position.z = (float)pos.Y;
                var y = Menu.showElevation ? (float)pos.Z - SumoNetworkVisualizer.minY : 0;
                position.y = y > 0 ? y : 0;
                var rot = Quaternion.Euler(0, (float)angle, 0);

                this.transform.position = position;

                if (Menu.showElevation)
                {
                    RaycastHit hit;
                    Ray ray = new Ray(transform.position + new Vector3(0, 10, 0), Vector3.down);

                    if (Physics.Raycast(ray, out hit, 100))
                    {
                        var angle2 = Vector3.SignedAngle(hit.normal, Vector3.up, Vector3.right);
                        rot = Quaternion.Euler(angle2, (float)angle, 0);
                    }
                }

                this.transform.rotation = rot;



                time = 0;
            }

        }

    }
}
