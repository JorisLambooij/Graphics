using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template_P3
{
    public class SceneGraph
    {
        ////Een List die bijhoudt welke meshes de kinderen zijn van de huidige mesh.
        //public List<Mesh> nodeChildren = new List<Mesh>();

        SceneGraph treeNodeParent;

        public List<SceneGraph> treeNodeChildren = new List<SceneGraph>();

        //Dit is nodig voor TreeNodes met meshes.
        //Naam van de huidige mesh.
        public Mesh treeNodeMesh;
        //Texture van huidige mesh.
        Texture treeNodeTexture;
        Matrix4 nodeMatrix;

        public SceneGraph(SceneGraph parent, Mesh mesh = null, Texture texture = null)
        {
            treeNodeParent = parent;
            treeNodeMesh = mesh;
            treeNodeTexture = texture;

            //Je zit in een kind.
            //Voeg het kind toe aan NodeChildren van parent.
            if (parent != null)
                parent.treeNodeChildren.Add(this);

            nodeMatrix = Matrix4.Identity;
        }

        public void TreeNodeRender(Shader shader, Matrix4 transform, Matrix4 worldTransform, Vector4[] lightData)
        {
            //Het object in de TreeNode is een mesh.
            if (treeNodeMesh != null)
                treeNodeMesh.Render(shader, transform, worldTransform, treeNodeTexture, lightData);

            //foreach loop vervangen door deze for loop, zodat er geen exception ontstaat als een node geen kinderen heeft.
            for (int i = 0; i < treeNodeChildren.Count; i++)
            {
                Matrix4 matrix;// = Matrix4.CreateFromAxisAngle(new Vector3(0,1,0), 0);
                //Matrix van ouder en kind vermenigvuldigen.
                matrix = treeNodeChildren[i].treeNodeMesh.meshTransform * transform;
                //Uitkomst Matrix4 doorgeven aan kind.
                treeNodeChildren[i].TreeNodeRender(shader, matrix, worldTransform, lightData);
            }
            //Als een node geen kind heeft, wordt de for loop niet uitgevoerd en houdt de recursie dus op.
        }
    }
}
