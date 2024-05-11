using System;
using System.Collections.Generic;

namespace Assets.Scripts.SUMOImporter.NetFileComponents
{
    public class NetFileLane
    {
        public string id;
        public int index;
        public double speed;
        public double length;
        public double width;
        public List<double[]> shape;
        public string allow;

        public NetFileLane(string id)
        {
            this.id = id;
        }

        public NetFileLane(string id, int index, double speed, double length, double width, string shape, string allow)
        {
            this.id = id;
            this.index = index;
            this.speed = speed;
            this.length = length;
            this.width = width;
            this.allow = allow;

            addShapeCoordinates(shape);
        }

        private void addShapeCoordinates(string shape)
        {
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

        internal void update(int index, double speed, double length, string shape, string allow)
        {
            this.index = index;
            this.speed = speed;
            this.length = length;
            this.allow = allow;

            addShapeCoordinates(shape);
        }
    }
}
