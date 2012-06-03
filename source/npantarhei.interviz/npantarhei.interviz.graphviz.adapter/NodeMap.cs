using System.Collections.Generic;
using System.Drawing;

namespace npantarhei.interviz.graphviz.adapter
{
    public class NodeMap
    {
        public class NodeArea
        {
            public NodeArea(string name, Rectangle rectangle)
            {
                Rectangle = rectangle;
                Name = name;
            }

            public string Name { get; private set; }
            public Rectangle Rectangle { get; private set; }
        }
        
        public NodeMap(IEnumerable<NodeArea> areas)
        {
            this.NodeAreas = areas;
        }

        public IEnumerable<NodeArea> NodeAreas { get; private set; }
    }
}