using System;
using UnityEngine;
using Console = System.Console;
using Flash2;

namespace FixingOGLongTorus
{
   
    public class Main : MonoBehaviour
    {
        private static bool ObjectDestroyed = false;
        private static GameObject Object = null;
        public static void OnModUpdate()
        {
            if (FindObjectOfType<MainGameStage>() == null)
            {
                ObjectDestroyed = false;
            }
            // Only attempt stage fixes if the Main Game is being played
            if (FindObjectOfType<MainGameStage>() == null) return;
            if (ObjectDestroyed) return;

            switch (FindObjectOfType<MainGameStage>().m_StageIndex)
            {
                // OG Long Torus
                case 2327:
                    Object = GameObject.Find("col2");
                    Destroy(Object);
                    ObjectDestroyed = true;
                    break;
                // Double Spiral
                case 2287:
                    Object = GameObject.Find("col5");
                    Destroy(Object);
                    ObjectDestroyed = true;
                    break;
                // Swing Shaft
                case 2057:
                    Object = GameObject.Find("col7");
                    Destroy(Object);
                    ObjectDestroyed = true;
                    break;
                // Swing Shaft (SMB2 Expert)
                case 2299:
                    Object = GameObject.Find("col7");
                    Destroy(Object);
                    ObjectDestroyed = true;
                    break;
                // Linear Seesaws (Story)
                case 2058:
                    GameObject.Find("Collision").gameObject.transform.Find("SEESAW_1_1_1Z1Z").transform.localPosition = new Vector3(0f, -0.06f, GameObject.Find("Collision").gameObject.transform.GetChild(5).localPosition.z);
                    GameObject.Find("Collision").gameObject.transform.Find("SEESAW_1_1_1Z2Z").transform.localPosition = new Vector3(0f, -0.06f, GameObject.Find("Collision").gameObject.transform.GetChild(6).localPosition.z);
                    GameObject.Find("Collision").gameObject.transform.Find("SEESAW_1_1_1Z3Z").transform.localPosition = new Vector3(0f, -0.06f, GameObject.Find("Collision").gameObject.transform.GetChild(7).localPosition.z);
                    ObjectDestroyed = true;
                    break;
                // Linear Seesaws (SMB2 Casual)
                case 2219:
                    GameObject.Find("Collision").gameObject.transform.Find("SEESAW_1_1_1Z1Z").transform.localPosition = new Vector3(0f, -0.06f, GameObject.Find("Collision").gameObject.transform.GetChild(5).localPosition.z);
                    GameObject.Find("Collision").gameObject.transform.Find("SEESAW_1_1_1Z2Z").transform.localPosition = new Vector3(0f, -0.06f, GameObject.Find("Collision").gameObject.transform.GetChild(6).localPosition.z);
                    GameObject.Find("Collision").gameObject.transform.Find("SEESAW_1_1_1Z3Z").transform.localPosition = new Vector3(0f, -0.06f, GameObject.Find("Collision").gameObject.transform.GetChild(7).localPosition.z);
                    ObjectDestroyed = true;
                    break;
                // 8 Seesaws
                case 2336:
                    GameObject.Find("Collision").gameObject.transform.Find("SEESAW_1_1_1Z1").transform.localPosition = new Vector3(0f, -0.26f, GameObject.Find("Collision").gameObject.transform.GetChild(4).localPosition.z);
                    ObjectDestroyed = true;
                    break;
            }
        }
    }
}
