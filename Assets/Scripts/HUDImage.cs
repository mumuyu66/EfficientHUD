using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class HUDImage:Image
{
    public UIMeshDataBuffer meshDataBuffer { get; set; }


    protected override void OnPopulateMesh(VertexHelper toFill)
    {
#if UNITY_EDITOR 
        // 给编辑时用
        if (!Application.isPlaying)
        {
            base.OnPopulateMesh(toFill);
        }
#endif
        if (sprite == null)
        {
            return;
        }
        switch (type)
        {
            case Type.Simple:
                GenerateSimpleSprite();
                break;
            case Type.Sliced:
                GenerateSlicedSprite();
                break;
            case Type.Tiled:
                break;
            case Type.Filled:
                break;
        }
    }

    /*
     *         /// <summary>
        /// Generate vertices for a simple Image.
        /// </summary>
        void GenerateSimpleSprite(VertexHelper vh, bool lPreserveAspect)
        {
            Vector4 v = GetDrawingDimensions(lPreserveAspect);
            var uv = (activeSprite != null) ? Sprites.DataUtility.GetOuterUV(activeSprite) : Vector4.zero;

            var color32 = color;
            vh.Clear();
            vh.AddVert(new Vector3(v.x, v.y), color32, new Vector2(uv.x, uv.y));
            vh.AddVert(new Vector3(v.x, v.w), color32, new Vector2(uv.x, uv.w));
            vh.AddVert(new Vector3(v.z, v.w), color32, new Vector2(uv.z, uv.w));
            vh.AddVert(new Vector3(v.z, v.y), color32, new Vector2(uv.z, uv.y));

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);
        }*/

    protected virtual void GenerateSimpleSprite() 
    { 
        
    }

    protected virtual void GenerateSlicedSprite()
    {

    }

    protected virtual void GenerateTiledSprite()
    {

    }

    protected virtual void GenerateFilledSprite()
    {
        
    }

}
