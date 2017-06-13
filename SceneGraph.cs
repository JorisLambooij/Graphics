using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template_P3
{
    class SceneGraph
    {
        //TO DO
        //Matrix4 en TreeNode worden niet herkend om een of andere reden, maar de libraries staan er wel.

        public void Render(Matrix4 ouder, TreeNode treeNode)
        {
            //foreach loop vervangen door deze for loop, zodat er geen exception ontstaat als een node geen kinderen heeft.
            for (int i = 0; i < treeNode.nodeChildren.Length; i++)
            {
                Matrix4 = matrix;
                //Matrix van ouder en kind vermenigvuldigen.
                matrix = ouder * treeNode.nodeChildren[i].meshTransform;
                //Uitkomst Matrix4 doorgeven aan kind.
                Render(matrix, treeNode.nodeChildren[i]);
            }
            //Als een node geen kind heeft, wordt de for loop niet uitgevoerd en houdt de recursie dus op.
        }
    }
}