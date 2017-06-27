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

            nodeMatrix = Matrix4.Identity;

            //Je zit in een kind.
            //Voeg het kind toe aan NodeChildren van parent.
            if (parent != null)
                parent.treeNodeChildren.Add(this);

        }

        public void TreeNodeRender(Shader shader, Matrix4 transform, Matrix4 worldTransform, Vector4[] lightData, Vector4 camDir)
        {
            transform = nodeMatrix * treeNodeMesh.meshTransform * transform;
            worldTransform = nodeMatrix * treeNodeMesh.meshTransform * worldTransform;
            
            if (treeNodeMesh != null)
                treeNodeMesh.Render(shader, transform, worldTransform, treeNodeTexture, lightData, camDir);
            
            for (int i = 0; i < treeNodeChildren.Count; i++)
                treeNodeChildren[i].TreeNodeRender(shader, transform, worldTransform, lightData, camDir);
        }
    }
}
