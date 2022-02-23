using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TestIMeshModifier : MonoBehaviour, IMeshModifier
{
    public void ModifyMesh(Mesh mesh)
    {
        Debug.Log(String.Format("TestIMeshModifier.ModifyMesh mesh.vertices = {0}", mesh.vertices));
    }

    public void ModifyMesh(VertexHelper vh)
    {
        var verts = ListPool<UIVertex>.Get();
        vh.GetUIVertexStream(verts);
        foreach (var v in verts)
        {
            Debug.Log(String.Format("VertexHelper.ModifyMesh mesh.vertices = {0}  {1}", v.position,v.tangent));
            
        }    
        
    }
}
