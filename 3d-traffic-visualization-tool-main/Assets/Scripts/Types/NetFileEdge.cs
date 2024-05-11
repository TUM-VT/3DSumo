using System;
using System.Collections.Generic;

namespace Assets.Scripts.SUMOImporter.NetFileComponents
{
    public class NetFileEdge
    {
        string id;
        NetFileJunction from;
        NetFileJunction to;
        int priority;
        List<NetFileLane> lanes;

        public edgeTypeFunction function;

        public NetFileEdge(string id, NetFileJunction from, NetFileJunction to, string priority, string shape, edgeTypeFunction function)
        {
            this.id = id;
            this.priority = Convert.ToInt32(priority);

            this.lanes = new List<NetFileLane>();

            this.from = from;
            this.to = to;

            this.function = function;
        }

        public int getPriority()
        {
            return this.priority;
        }

        public void addLane(NetFileLane lane, string index, float speed, float length, float width, string shape, string allow)
        {
            lane.update(Convert.ToInt32(index), Convert.ToDouble(speed), Convert.ToDouble(length), shape, allow);
            this.lanes.Add(new NetFileLane(lane.id, Convert.ToInt32(index), speed, length, width, shape, allow));
        }

        public List<NetFileLane> getLanes()
        {
            return this.lanes;
        }

        public NetFileJunction getFrom()
        {
            return this.from;
        }

        public NetFileJunction getTo()
        {
            return this.to;
        }

        public string getId()
        {
            return this.id;
        }
    }


}