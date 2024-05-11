using System.Collections.Generic;

namespace Assets.Scripts.SUMOImporter.NetFileComponents
{
    public class NetFileTrafficLightSystem
    {
        public string id;
        public List<NetFileTrafficLight> trafficLights;
        public float y;

        public NetFileTrafficLightSystem(string id)
        {
            this.id = id;
            this.trafficLights = new List<NetFileTrafficLight>();
        }

        public void AddTrafficLight(NetFileTrafficLight trafficLight)
        {
            this.trafficLights.Add(trafficLight);
        }
    }
}
