using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class HUDImage:Image
{
    protected override void OnPopulateMesh(VertexHelper toFill)
    {
#if UNITY_EDITOR 
        // 给编辑时用
        if (!Application.isPlaying)
        {
            base.OnPopulateMesh(toFill);
        }
#endif

    }

}
