using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(RectTransform))]
    [ExecuteInEditMode]
    public class MeshImage:MeshUI
    {
        public Sprite activeSprite;
        protected override void OnPopulateMesh()
        {
            var r = GetPixelAdjustedRect();
            Vector3 p = rectTransform.position;
            var v = new Vector4(p.x, p.y, p.x + r.width, p.y + r.height);
            var uv = (activeSprite != null) ? Sprites.DataUtility.GetOuterUV(activeSprite) : Vector4.zero;
            meshBuffer.UpdataColor(qmesh.buffIndex, color);
            meshBuffer.UpdataUV(qmesh.buffIndex, new Vector2(uv.x, uv.y), new Vector2(uv.z, uv.w), new Vector2(uv.z, uv.w), new Vector2(uv.z, uv.y));
            meshBuffer.UpdataVertices(qmesh.buffIndex, new Vector3(v.x, v.y), new Vector3(v.x, v.w), new Vector3(v.z, v.w), new Vector3(v.z, v.y));
            meshBuffer.FillQuad(qmesh.indicesIndex, qmesh.buffIndex);
        }
    }
}
