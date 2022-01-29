using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class HUDImage:Image
{

    private int meshId;
    private ActorUIMesh hudMesh;
    public void SetActorHUDMesh(int id, ActorUIMesh mesh)
    {
        this.meshId = id;
        this.hudMesh = mesh;
    }

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
                GenerateSimpleSprite(m_PreserveAspect);
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
    [NonSerialized]
    private Sprite m_OverrideSprite;
    [SerializeField] private bool m_PreserveAspect = false;

    private Sprite activeSprite { get { return m_OverrideSprite != null ? m_OverrideSprite : sprite; } }

    private void PreserveSpriteAspectRatio(ref Rect rect, Vector2 spriteSize)
    {
        var spriteRatio = spriteSize.x / spriteSize.y;
        var rectRatio = rect.width / rect.height;

        if (spriteRatio > rectRatio)
        {
            var oldHeight = rect.height;
            rect.height = rect.width * (1.0f / spriteRatio);
            rect.y += (oldHeight - rect.height) * rectTransform.pivot.y;
        }
        else
        {
            var oldWidth = rect.width;
            rect.width = rect.height * spriteRatio;
            rect.x += (oldWidth - rect.width) * rectTransform.pivot.x;
        }
    }    

    /// Image's dimensions used for drawing. X = left, Y = bottom, Z = right, W = top.
    private Vector4 GetDrawingDimensions(bool shouldPreserveAspect)
    {
        var padding = activeSprite == null ? Vector4.zero : UnityEngine.Sprites.DataUtility.GetPadding(activeSprite);
        var size = activeSprite == null ? Vector2.zero : new Vector2(activeSprite.rect.width, activeSprite.rect.height);

        Rect r = GetPixelAdjustedRect();
        // Debug.Log(string.Format("r:{2}, size:{0}, padding:{1}", size, padding, r));

        int spriteW = Mathf.RoundToInt(size.x);
        int spriteH = Mathf.RoundToInt(size.y);

        var v = new Vector4(
            padding.x / spriteW,
            padding.y / spriteH,
            (spriteW - padding.z) / spriteW,
            (spriteH - padding.w) / spriteH);

        if (shouldPreserveAspect && size.sqrMagnitude > 0.0f)
        {
            PreserveSpriteAspectRatio(ref r, size);
        }

        v = new Vector4(
            r.x + r.width * v.x,
            r.y + r.height * v.y,
            r.x + r.width * v.z,
            r.y + r.height * v.w
        );

        return v;
    }

    protected virtual void GenerateSimpleSprite(bool lPreserveAspect) 
    {
        Vector4 v = GetDrawingDimensions(lPreserveAspect);
        var uv = (activeSprite != null) ? UnityEngine.Sprites.DataUtility.GetOuterUV(activeSprite) : Vector4.zero;

        hudMesh.UpdataColor(meshId, color);
        hudMesh.UpdataUV(meshId,new Vector2(uv.x, uv.y), new Vector2(uv.x, uv.w), new Vector2(uv.z, uv.w), new Vector2(uv.z, uv.y));
        hudMesh.UpdataVertices(meshId,new Vector3(v.x, v.y), new Vector3(v.x, v.w),new Vector3(v.z, v.w), new Vector3(v.z, v.y));
        hudMesh.FillQuad(meshId);
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
