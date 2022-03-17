using System;
using System.Collections.Generic;
using UnityEngine;

public class QuadMesh
{
    public int id;
    public int buffIndex;
    public int indicesIndex;
}

public class SharedQuadMesh
{
    private UIMeshBuffer buffer;
    private QuadMesh[] meshs;
    private HashSet<int> idHashSet = new HashSet<int>();
    private List<int> ids = null;
    private int index = 0;
    public QuadMesh Get()
    {
        int key = ids[index];
        if (key >= 0)
        {
            idHashSet.Remove(key);
            ids[index++] = -1;
            return meshs[key];
        }
        index++;
        return Get();
    }

    public void Release(QuadMesh mesh)
    {
        if (!idHashSet.Contains(mesh.id))
        {
            ids[mesh.id] = mesh.id;
            if (mesh.id < index)
            {
                index = mesh.id;
            }
            buffer.CollapseQuad(mesh.buffIndex,mesh.indicesIndex);
        }
    }

    public SharedQuadMesh(int quadNum, UIMeshBuffer buffer)
    {
        meshs = new QuadMesh[quadNum];
        ids = new List<int>(quadNum);
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
            meshs[i] = (new QuadMesh() {id = i, buffIndex = bi, indicesIndex = ii });
            ids.Add(i);
            idHashSet.Add(i);
        }
        this.buffer = buffer;
    }

    public void FillQuad(int index)
    {
        buffer.FillQuad(meshs[index].indicesIndex, meshs[index].buffIndex);
    }

    public void UpdataColor(int index, Color color)
    {
        QuadMesh quad = meshs[index];
        buffer.UpdataColor(quad.buffIndex,color);
    }

    public void UpdataVertices(int index, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
    {
        QuadMesh quad = meshs[index];
        buffer.UpdataVertices(quad.buffIndex, v0,v1,v2,v3);
    }

    public void UpdataUV(int index, Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3)
    {
        QuadMesh quad = meshs[index];
        buffer.UpdataUV(quad.buffIndex, v0, v1, v2, v3);
    }

    public void FillMesh(Mesh mesh)
    {
        buffer.FillMesh(mesh);
    }
}

public class ActorUIMeshProvider
{
    private readonly Stack<SharedQuadMesh> stack = new Stack<SharedQuadMesh>();
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
                SharedQuadMesh element = new SharedQuadMesh(actorHUDQuadNum, buffer);
                stack.Push(element);
                CountAll++;
            }
            inited = true;
        }
    }

    public SharedQuadMesh Get()
    {
        SharedQuadMesh element;
        if (stack.Count == 0)
        {
            element = new SharedQuadMesh(actorHUDQuadNum, buffer);
            CountAll++;
        }
        else
        {
            element = stack.Pop();
        }
        return element;
    }

    public void Release(SharedQuadMesh element)
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

    public int AddQuad(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector2 uv3, Color32 color, out int indicesIndex)
    {
        int startIndex = mVertices.Count;
        indicesIndex = mIndices.Count;
        mVertices.Add(v0);
        mVertices.Add(v1);
        mVertices.Add(v2);
        mVertices.Add(v3);

        mUv0s.Add(uv0);
        mUv0s.Add(uv1);
        mUv0s.Add(uv2);
        mUv0s.Add(uv3);

        mColors.Add(color);
        mColors.Add(color);
        mColors.Add(color);
        mColors.Add(color);

        mIndices.Add(startIndex);
        mIndices.Add(startIndex + 1);
        mIndices.Add(startIndex + 2);

        mIndices.Add(startIndex + 1);
        mIndices.Add(startIndex + 3);
        mIndices.Add(startIndex + 2);

        return startIndex;
    }

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

    public void CollapseQuad(int buffId,int indicesIndex)
    {
        for (int i = indicesIndex; i < indicesIndex + 6; i++)
        {
            mIndices[i] = 0;
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

    public void UpdateVert(int index, Vector3 position, Color32 color, Vector2 uv0)
    {
        mVertices[index] = position;
        mColors[index] = color;
        mUv0s[index] = uv0;
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
     //   Debug.Log(string.Format("UpdataVertices index = {0} v0 = {1} v1 = {2} v2 = {3} v3 = {4}",index,v0,v1,v2,v3));
        mVertices[index] = v0;
        mVertices[index + 1] = v1;
        mVertices[index + 2] = v2;
        mVertices[index + 3] = v3;
    }

    public void UpdataVertices(int index, Vector3[] v)
    {
        for (int i = 0; i < v.Length; i++)
        {
            mVertices[index + i] = v[i];
        }
    }

    public void UpdataUV(int index, Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3)
    {
        mUv0s[index] = v0;
        mUv0s[index + 1] = v1;
        mUv0s[index + 2] = v2;
        mUv0s[index + 3] = v3;
    }

    public void UpdataUV(int index, Vector2[] v)
    {
        for (int i = 0; i < v.Length; i++)
        {
            mUv0s[index + i] = v[i];
        }
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
    }

}

