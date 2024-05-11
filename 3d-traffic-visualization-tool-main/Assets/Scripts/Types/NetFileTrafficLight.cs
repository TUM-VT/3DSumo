namespace Assets.Scripts.SUMOImporter.NetFileComponents
{
    public class NetFileTrafficLight
    {
        public int linkIndex;
        public NetFileEdge edge;

        public string dir;

        public NetFileTrafficLight(int linkIndex, NetFileEdge edge, string dir)
        {
            this.linkIndex = linkIndex;
            this.edge = edge;
            this.dir = dir;
        }
    }
}
