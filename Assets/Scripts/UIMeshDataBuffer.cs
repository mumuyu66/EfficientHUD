using System;
using System.Collections.Generic;
using UnityEngine;

public class QuadMesh
{
    public int buffIndex;
    public int indicesIndex;
}

public class ActorUIMesh
{
    private UIMeshBuffer buffer;
    private List<QuadMesh> quads = new List<QuadMesh>();

    public ActorUIMesh(int quadNum, UIMeshBuffer buffer)
    {
        for (int i = 0; i < quadNum; i++)
        {
            Vector3[] vertices = new Vector3[] {
                new Vector3(0,0,0),
                new Vector3(0,0,0),
                new Vector3(0,0,0),
                new Vector3(0,0,0)
            };
            Vector2[] uvs = new Vector2[] {
                new Vector2(0,0),
                new Vector2(0,0),
                new Vector2(0,0),
                new Vector2(0,0),
            };
            int bi, ii;
            bi = buffer.AddQuad(vertices, new Color(0, 0, 0, 0.5f), uvs,out ii, true);
            quads.Add(new QuadMesh() { buffIndex = bi, indicesIndex = ii });
        }
        this.buffer = buffer;
    }

    public void CollapseQuadPostion(int index)
    {
        if (index >= quads.Count || index < 0) return;
        QuadMesh quad = quads[index];
        buffer.CollapseQuad(quad.indicesIndex);
    }

    public void FillQuad(int index)
    {
        if (index >= quads.Count || index < 0) return;

        buffer.FillQuad(quads[index].indicesIndex, quads[index].buffIndex);
    }

    public void UpdataColor(int index, Color color)
    {
        if (index >= quads.Count || index < 0) return;
        QuadMesh quad = quads[index];
        buffer.UpdataColor(quad.buffIndex,color);
    }

    public void UpdataVertices(int index, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
    {
        if (index >= quads.Count || index < 0) return;
        QuadMesh quad = quads[index];
        buffer.UpdataVertices(quad.buffIndex, v0,v1,v2,v3);
    }

    public void UpdataUV(int index, Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3)
    {
        if (index >= quads.Count || index < 0) return;
        QuadMesh quad = quads[index];
        buffer.UpdataUV(quad.buffIndex, v0, v1, v2, v3);
    }

    public void FillMesh(Mesh mesh)
    {
        buffer.FillMesh(mesh);
    }
}

public class ActorUIMeshProvider
{
    private readonly Stack<ActorUIMesh> stack = new Stack<ActorUIMesh>();
    private static readonly Lazy<ActorUIMeshProvider> lazy = new Lazy<ActorUIMeshProvider>(() => new ActorUIMeshProvider());
    public static ActorUIMeshProvider Instance { get { return lazy.Value; }}

    public int CountAll { get; private set; }
    public int CountActive { get { return CountAll - CountInactive; } }
    public int CountInactive { get { return stack.Count; } }
    private UIMeshBuffer buffer;
    private int actorHUDQuadNum = 4;
    public ActorUIMeshProvider()
    {
        buffer = new UIMeshBuffer();
    }

    private bool inited = false;
    public void InitBuff(int actorNum,int actorQuadNum)
    {
        if (!inited)
        {
            this.actorHUDQuadNum = actorQuadNum;
            for (int i = 0; i < actorNum; i++)
            {
                ActorUIMesh element = new ActorUIMesh(actorHUDQuadNum, buffer);
                stack.Push(element);
                CountAll++;
            }
            inited = true;
        }
    }

    public ActorUIMesh Get()
    {
        ActorUIMesh element;
        if (stack.Count == 0)
        {
            element = new ActorUIMesh(actorHUDQuadNum, buffer);
            CountAll++;
        }
        else
        {
            element = stack.Pop();
        }
        return element;
    }

    public void Release(ActorUIMesh element)
    {
        if (stack.Count > 0 && ReferenceEquals(stack.Peek(), element))
            Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
        stack.Push(element);
    }
}

public class UIMeshBuffer
{
    private List<Vector3> mVertices = ListPool<Vector3>.Get();
    private List<Color32> mColors = ListPool<Color32>.Get();
    private List<Vector2> mUv0s = ListPool<Vector2>.Get();
    private List<Vector2> mUv1s = ListPool<Vector2>.Get();
    private List<Vector2> mUv2s = ListPool<Vector2>.Get();
    private List<Vector2> mUv3s = ListPool<Vector2>.Get();
    private List<Vector3> mNormals = ListPool<Vector3>.Get();
    private List<int> mIndices = ListPool<int>.Get();

    public int AddQuad(Vector3[] vertices, Color32 color, Vector2[] uvs, out int indicesIndex, bool collapse = false)
    {
        int startIndex = mVertices.Count;

        mVertices.AddRange(vertices);
        mUv0s.AddRange(uvs);

        mColors.Add(color);
        mColors.Add(color);
        mColors.Add(color);
        mColors.Add(color);

        indicesIndex = mIndices.Count;

        if (collapse)
        {
            mIndices.Add(startIndex);
            mIndices.Add(startIndex );
            mIndices.Add(startIndex );

            mIndices.Add(startIndex);
            mIndices.Add(startIndex);
            mIndices.Add(startIndex);
        }
        else
        {
            mIndices.Add(startIndex);
            mIndices.Add(startIndex + 1);
            mIndices.Add(startIndex + 2);

            mIndices.Add(startIndex + 1);
            mIndices.Add(startIndex + 3);
            mIndices.Add(startIndex + 2);
        }
        return startIndex;
    }

    public void CollapseQuad(int indicesIndex)
    {
        for (int i = indicesIndex; i < indicesIndex + 6; i++)
        {
            mIndices[i] = indicesIndex;
        }
    }

    public void FillQuad(int indicesIndex,int vindex)
    {
        mIndices[indicesIndex] = vindex;
        mIndices[indicesIndex + 1] = vindex + 1;
        mIndices[indicesIndex + 2] = vindex + 2;

        mIndices[indicesIndex + 3] = vindex + 2;
        mIndices[indicesIndex + 4] = vindex + 3;
        mIndices[indicesIndex + 5] = vindex + 0;
    }

    public void UpdataColor(int index,Color color)
    {
        for (int i = index; i < index + 4; i++)
        {
            mColors[i] = color;
        }
    }

    public void UpdataVertices(int index, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
    {
        mVertices[index] = v0;
        mVertices[index + 1] = v1;
        mVertices[index + 2] = v2;
        mVertices[index + 3] = v3;
    }

    public void UpdataUV(int index, Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3)
    {
        mUv0s[index] = v0;
        mUv0s[index + 1] = v1;
        mUv0s[index + 2] = v2;
        mUv0s[index + 3] = v3;
    }

    public void FillMesh(Mesh mesh)
    {
        mesh.SetVertices(mVertices);
        mesh.SetColors(mColors);
        mesh.SetUVs(0, mUv0s);
       // mesh.SetUVs(1, mUv1s);
       // mesh.SetUVs(2, mUv2s);
       // mesh.SetUVs(3, mUv3s);
      //  mesh.SetNormals(mNormals);
        mesh.SetTriangles(mIndices, 0);
      //  mesh.RecalculateBounds();
    }
}

