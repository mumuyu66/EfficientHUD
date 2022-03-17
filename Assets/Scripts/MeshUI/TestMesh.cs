using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using Unity.Collections;

public class TestMesh : MonoBehaviour
{
    public int m_TriangleCount;
    private Mesh m_Mesh;
    public Sprite activeSprite;
    private MeshFilter meshFilter;
    NativeArray<Vector3> m_VertexPos;
    NativeArray<Vector3> m_VertexNor;
    void Start()
    {
        Material mat = new Material(Shader.Find("UI/Default"));
        mat.SetTexture("_MainTex", activeSprite.texture);

        m_Mesh = new Mesh();
#if UNITY_2021_2_OR_NEWER // Mesh GPU buffer access is since 2021.2            
            m_Mesh.vertexBufferTarget |= GraphicsBuffer.Target.Raw;
#endif
        // specify vertex count and layout
        var layout = new[]
        {
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float16, 2),
            new VertexAttributeDescriptor(VertexAttribute.Tangent, VertexAttributeFormat.UNorm8, 4),
        };

        m_Mesh.SetVertexBufferParams(m_TriangleCount * 3, new VertexAttributeDescriptor(VertexAttribute.Position, stream: 0), new VertexAttributeDescriptor(VertexAttribute.Normal, stream: 1));
        m_VertexPos = new NativeArray<Vector3>(m_TriangleCount * 3, Allocator.Persistent);
        m_VertexNor = new NativeArray<Vector3>(m_TriangleCount * 3, Allocator.Persistent);

        m_Mesh.SetIndexBufferParams(m_TriangleCount * 3, IndexFormat.UInt32);
        var ib = new NativeArray<int>(m_TriangleCount * 3, Allocator.Temp);
        for (var i = 0; i < m_TriangleCount * 3; ++i)
            ib[i] = i;
        m_Mesh.SetIndexBufferData(ib, 0, 0, ib.Length, MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices);
        ib.Dispose();
        var submesh = new SubMeshDescriptor(0, m_TriangleCount * 3, MeshTopology.Triangles);
        submesh.bounds = new Bounds(Vector3.zero, new Vector3(10, 10, 10));
        m_Mesh.SetSubMesh(0, submesh);
        m_Mesh.bounds = submesh.bounds;






        meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        meshFilter.mesh = m_Mesh;

        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            renderer = gameObject.AddComponent<MeshRenderer>();
        }
        renderer.material = mat;
        renderer.sortingOrder = 1;
        renderer.receiveShadows = false;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

    }

    // Update is called once per frame
    void Update()
    {

    }
}
