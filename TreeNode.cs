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
        ////Een List die bijhoudt welke meshes de kinderen zijn van de huidige mesh.
        //public List<Mesh> nodeChildren = new List<Mesh>();

        public List<TreeNode> treeNodeChildren = new List<TreeNode>();
        //Naam van de huidige mesh.
        Mesh treeNodeMesh;

        public TreeNode(Mesh mesh)
        {
            treeNodeMesh = mesh;
        }

        public void Render(Matrix4 ouder, TreeNode treeNode)
        {
            //foreach loop vervangen door deze for loop, zodat er geen exception ontstaat als een node geen kinderen heeft.
            for (int i = 0; i < treeNode.treeNodeChildren.Count; i++)
            {
                Matrix4 matrix;
                //Matrix van ouder en kind vermenigvuldigen.
                matrix = ouder * treeNode.treeNodeChildren[i].treeNodeMesh.meshTransform;
                //Uitkomst Matrix4 doorgeven aan kind.
                Render(matrix, treeNode.treeNodeChildren[i]);
            }
            //Als een node geen kind heeft, wordt de for loop niet uitgevoerd en houdt de recursie dus op.
        }
    }
}
