using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template_P3
{
    public class TreeNode
    {        
        //Een List die bijhoudt welke meshes de kinderen zijn van de huidige mesh.
        public List<Mesh> nodeChildren = new List<Mesh>();
        //Naam van de huidige mesh.
        Mesh nodeMesh;

        public TreeNode(Mesh mesh)
        {
            nodeMesh = mesh;
        }
    }
}
