using System.Collections.Generic;
using UnityEngine.UI.Collections;

namespace UnityEngine.UI
{
    public class MeshUIRegistry
    {
        private static MeshUIRegistry s_Instance;

        private readonly Dictionary<UIMeshBuffer, IndexedSet<MeshUI>> m_MeshUIs = new Dictionary<UIMeshBuffer, IndexedSet<MeshUI>>();

        protected MeshUIRegistry()
        {
            // This is needed for AOT on IOS. Without it the compile doesn't get the definition of the Dictionarys
#pragma warning disable 168
            Dictionary<Graphic, int> emptyUIDic;
            Dictionary<ICanvasElement, int> emptyElementDic;
#pragma warning restore 168
        }

        public static MeshUIRegistry instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = new MeshUIRegistry();
                return s_Instance;
            }
        }

        public static void RegisterGraphicForCanvas(MeshUI ui)
        {
            if (ui == null)
                return;
           
        }

        public static void UnregisterGraphicForCanvas(Canvas c, MeshUI ui)
        {
            if (c == null)
                return;

          
        }

        private static readonly List<MeshUI> s_EmptyList = new List<MeshUI>();
        public static IList<MeshUI> GetGraphicsForCanvas(Canvas canvas)
        {
           

            return s_EmptyList;
        }
    }
}
