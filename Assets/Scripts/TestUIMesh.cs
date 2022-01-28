using DaVikingCode.AssetPacker;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.UI
{
    public struct HUDData
    {
        public int index;
        public int uuid;
        public Vector3 hpos;
    }

    public class TestUIMesh : MonoBehaviour
    {
        private Dictionary<int, HUDData> datas = new Dictionary<int, HUDData>();

        public Material uiMat;
        public AssetPacker assetPacker;
        private UIMeshDataBuffer buffer;


        void Start()
        {
            string[] files = Directory.GetFiles(Application.dataPath + "/UI", "*.png");

            assetPacker = GetComponent<AssetPacker>();

            assetPacker.OnProcessCompleted.AddListener(run);

            assetPacker.AddTexturesToPack(files);
            assetPacker.Process();
        }

        private Mesh mesh;

        private void run() 
        {
            Material material = new Material(Shader.Find("UI/Default"));
            mesh = new Mesh();
            buffer = new UIMeshDataBuffer();
            material.SetTexture("_MainTex", assetPacker.texture);

            for (int i = 0; i < 5; i++)
            {
                AddActorHUD(i, new Vector3(i * 3,0,0));
            }
            drawMesh(this.gameObject,mesh, material,1);
        }

        public int AddActorHUD(int uuid, Vector3 pos)
        {
            float hp_bg_h = 0.5f;
            float hp_h = 0.4f;
            float hp_bg_w = 2f;
            float hp_w = 1.9f;
            int i;
            Rect rect = new Rect(pos.x, pos.z,hp_bg_w,hp_bg_h);
            Vector3[] vertices =  new Vector3[] {
                new Vector3(rect.xMin,pos.y,rect.yMin),
                new Vector3(rect.xMin,pos.y,rect.yMax),
                new Vector3(rect.xMax,pos.y,rect.yMin),
                new Vector3(rect.xMax,pos.y,rect.yMax),
            };

            Vector4 uv4 = UnityEngine.Sprites.DataUtility.GetOuterUV(assetPacker.GetSprite("common_bg"));
            rect.Set(uv4.x, uv4.y, uv4.z - uv4.x, uv4.w - uv4.y);
            Vector2[]  uvs = new Vector2[] {
                new Vector2(rect.xMin,rect.yMin),
                new Vector2(rect.xMin,rect.yMax),
                new Vector2(rect.xMax,rect.yMin),
                new Vector2(rect.xMax,rect.yMax),
            };
            int index = buffer.AddQuad(vertices,new Color(0,0,0,0.5f), uvs,out i, uuid == 2);

            rect.Set(pos.x+ (hp_bg_w - hp_w)/2, pos.z + (hp_bg_h - hp_h)/2, hp_w, hp_h);
            vertices = new Vector3[] {
                new Vector3(rect.xMin,pos.y,rect.yMin),
                new Vector3(rect.xMin,pos.y,rect.yMax),
                new Vector3(rect.xMax,pos.y,rect.yMin),
                new Vector3(rect.xMax,pos.y,rect.yMax),
            };
            uv4 = UnityEngine.Sprites.DataUtility.GetOuterUV(assetPacker.GetSprite("Hp_R"));
            rect.Set(uv4.x, uv4.y, uv4.z - uv4.x, uv4.w - uv4.y);
            uvs = new Vector2[] {
                new Vector2(rect.xMin,rect.yMin),
                new Vector2(rect.xMin,rect.yMax),
                new Vector2(rect.xMax,rect.yMin),
                new Vector2(rect.xMax,rect.yMax),
            };
            buffer.AddQuad(vertices, Color.white, uvs, out i, uuid == 2);

            float mp_bg_h = 0.2f;
            float mp_h = 0.15f;
            float mp_bg_w = 2f;
            float mp_w = 1.9f;
            rect.Set(pos.x , pos.z + hp_bg_h + 0.05f, mp_bg_w, mp_bg_h);
            vertices = new Vector3[] {
                new Vector3(rect.xMin,pos.y,rect.yMin),
                new Vector3(rect.xMin,pos.y,rect.yMax),
                new Vector3(rect.xMax,pos.y,rect.yMin),
                new Vector3(rect.xMax,pos.y,rect.yMax),
            };

            uv4 = UnityEngine.Sprites.DataUtility.GetOuterUV(assetPacker.GetSprite("common_bg"));
            rect.Set(uv4.x, uv4.y, uv4.z - uv4.x, uv4.w - uv4.y);
            uvs = new Vector2[] {
                new Vector2(rect.xMin,rect.yMin),
                new Vector2(rect.xMin,rect.yMax),
                new Vector2(rect.xMax,rect.yMin),
                new Vector2(rect.xMax,rect.yMax),
            };
            
            buffer.AddQuad(vertices, new Color(0, 0, 0, 0.5f), uvs, out i, uuid == 2);

            rect.Set(pos.x + (mp_bg_w - mp_w) / 2, pos.z + hp_bg_h + 0.05f + (mp_bg_h - mp_h) / 2, mp_w, mp_h);
            vertices = new Vector3[] {
                new Vector3(rect.xMin,pos.y,rect.yMin),
                new Vector3(rect.xMin,pos.y,rect.yMax),
                new Vector3(rect.xMax,pos.y,rect.yMin),
                new Vector3(rect.xMax,pos.y,rect.yMax),
            };
            uv4 = UnityEngine.Sprites.DataUtility.GetOuterUV(assetPacker.GetSprite("Mp_W"));
            rect.Set(uv4.x, uv4.y, uv4.z - uv4.x, uv4.w - uv4.y);
            uvs = new Vector2[] {
                new Vector2(rect.xMin,rect.yMin),
                new Vector2(rect.xMin,rect.yMax),
                new Vector2(rect.xMax,rect.yMin),
                new Vector2(rect.xMax,rect.yMax),
            };
            buffer.AddQuad(vertices, new Color(113.0f/255, 226.0f/255, 217.0f/255,1), uvs, out i, uuid == 2);



            return index;
        }

        private GameObject drawMesh(GameObject obj, Mesh mesh, Material mat, int sortingOrder, int initVertNum = 256)
        {
            buffer.FillMesh(mesh);
            MeshFilter filter = obj.AddComponent<MeshFilter>();
            filter.mesh = mesh;

            MeshRenderer renderer = obj.AddComponent<MeshRenderer>();
            renderer.material = mat;
            renderer.sortingOrder = sortingOrder;
            renderer.receiveShadows = false;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            return obj;
        }

        public static GameObject CreateBaseUI(GameObject parent,Sprite sprite ,string name, Material mat, int sortingOrder, int initVertNum = 256)
        {
            GameObject obj = new GameObject(name);
            Transform ct = obj.transform;
            Transform pt = parent.transform;
            ct.parent = pt;
            ct.position = pt.position;
            ct.rotation = pt.rotation;

            obj.layer = LayerMask.GetMask("HUD");
            Vector4 uv4 = UnityEngine.Sprites.DataUtility.GetOuterUV(sprite);
            Rect uv = new Rect(uv4.x, uv4.y, uv4.z - uv4.x, uv4.w - uv4.y);

            Mesh mesh = new Mesh();
            mesh.name = name;
            mesh.vertices = new Vector3[] {
                new Vector3(0,0,0),
                new Vector3(0,0,1),
                new Vector3(1,0,0),
                new Vector3(1,0,1),
            };
            //mesh.triangles
            mesh.uv = new Vector2[] {
                new Vector2(uv.xMin,uv.yMin),
                new Vector2(uv.xMin,uv.yMax),
                new Vector2(uv.xMax,uv.yMin),
                new Vector2(uv.xMax,uv.yMax),
            };
            mesh.triangles = new int[]
            {
                0,1,2,
                1,3,2
            };

            MeshFilter filter = obj.AddComponent<MeshFilter>();
            filter.mesh = mesh;

            MeshRenderer renderer = obj.AddComponent<MeshRenderer>();
            renderer.material = mat;
            renderer.sortingOrder = sortingOrder;
            renderer.receiveShadows = false;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

            return obj;
        }

   
    }

}