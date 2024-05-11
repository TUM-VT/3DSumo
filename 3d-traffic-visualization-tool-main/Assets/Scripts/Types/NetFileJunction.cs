using System;
using System.Collections.Generic;

namespace Assets.Scripts.SUMOImporter.NetFileComponents
{
    public class NetFileJunction
    {
        public string id;
        public junctionTypeType type;
        public float x;
        public float y;
        public float z;

        public List<NetFileLane> incLanes;
        public List<NetFileLane> intLanes;
        public List<NetFileEdge> incEdges = new List<NetFileEdge>();
        public List<double[]> shape;

        public NetFileJunction(string id, junctionTypeType type, float x, float y, string incLanes, string intLanes, string shape)
        {
            this.id = id;
            this.type = type;
            this.x = x;
            this.y = y;
            this.z = 0;

            // Get incoming Lanes
            this.incLanes = new List<NetFileLane>();
            foreach (string stringPiece in incLanes.Split(' '))
            {
                NetFileLane l = new NetFileLane(stringPiece);
                this.incLanes.Add(l);
            }

            this.intLanes = new List<NetFileLane>();
            foreach (string stringPiece in intLanes.Split(' '))
            {
                NetFileLane l = new NetFileLane(stringPiece);
                this.intLanes.Add(l);
            }

            // Get shape coordinates as List of tuple-arrays
            this.shape = new List<double[]>();
            foreach (string stringPiece in shape.Split(' '))
            {
                var values = stringPiece.Split(',');
                double xC = Convert.ToDouble(values[0]);
                double yC = Convert.ToDouble(values[1]);
                double zC = 0;
                if (values.Length > 2)
                {
                    zC = Convert.ToDouble(values[2]) > 0 ? Convert.ToDouble(values[2]) : 0;
                }
                this.shape.Add(new double[] { xC, yC, zC });
            }
        }
    }
}