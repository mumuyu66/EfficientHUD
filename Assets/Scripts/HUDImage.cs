using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class HUDImage: MaskableGraphic
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
    }

    [SerializeField] public Sprite activeSprite;

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


    public virtual void OnFillMesh(ActorUIMesh mesh)
    {
        //Vector4 v = GetDrawingDimensions(false);
        Rect r = rectTransform.rect;
        Vector4 v = new Vector4(r.xMin,r.yMin,r.xMax,r.yMax);
        var uv = (activeSprite != null) ? UnityEngine.Sprites.DataUtility.GetOuterUV(activeSprite) : Vector4.zero;
        mesh.UpdataColor(meshId, color);
        mesh.UpdataUV(meshId, new Vector2(uv.x, uv.y), new Vector2(uv.x, uv.w), new Vector2(uv.z, uv.w), new Vector2(uv.z, uv.y));
        mesh.UpdataVertices(meshId, new Vector3(v.x, v.y), new Vector3(v.x, v.w), new Vector3(v.z, v.w), new Vector3(v.z, v.y));
        mesh.FillQuad(meshId);
    }

    public virtual void OnRenderer(Renderer r)
    {
        material.SetTexture("_MainTex", activeSprite.texture);
        r.material = material;
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
