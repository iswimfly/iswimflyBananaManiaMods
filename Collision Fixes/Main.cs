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
            try
            {
                // OG Long Torus
                if (FindObjectOfType<MainGameStage>().m_StageIndex == 2327 && ObjectDestroyed == false)
                {
                    Object = GameObject.Find("col2");
                    Destroy(Object);
                    ObjectDestroyed = true;
                }
                // Double Spiral
                if (FindObjectOfType<MainGameStage>().m_StageIndex == 2287 && ObjectDestroyed == false)
                {
                    Object = GameObject.Find("col5");
                    Destroy(Object);
                    ObjectDestroyed = true;
                }
                // Swing Shaft (Story)
                if (FindObjectOfType<MainGameStage>().m_StageIndex == 2057 && ObjectDestroyed == false)
                {
                    Object = GameObject.Find("col7");
                    Destroy(Object);
                    ObjectDestroyed = true;
                }
                // Swing Shaft (SMB2 Expert)
                if (FindObjectOfType<MainGameStage>().m_StageIndex == 2057 && ObjectDestroyed == false)
                {
                    Object = GameObject.Find("col7");
                    Destroy(Object);
                    ObjectDestroyed = true;
                }
                // Linear Seesaws (Story)
                if (FindObjectOfType<MainGameStage>().m_StageIndex == 2058 && ObjectDestroyed == false)
                {
                    GameObject.Find("Collision").gameObject.transform.Find("SEESAW_1_1_1Z1Z").transform.localPosition = new Vector3(0f, -0.06f, GameObject.Find("Collision").gameObject.transform.GetChild(5).localPosition.z);
                    GameObject.Find("Collision").gameObject.transform.Find("SEESAW_1_1_1Z2Z").transform.localPosition = new Vector3(0f, -0.06f, GameObject.Find("Collision").gameObject.transform.GetChild(6).localPosition.z);
                    GameObject.Find("Collision").gameObject.transform.Find("SEESAW_1_1_1Z3Z").transform.localPosition = new Vector3(0f, -0.06f, GameObject.Find("Collision").gameObject.transform.GetChild(7).localPosition.z);
                    ObjectDestroyed = true;                                                                           
                }   
                // Linear Seesaws (SMB2 Casual)
                if (FindObjectOfType<MainGameStage>().m_StageIndex == 2219 && ObjectDestroyed == false)               
                {
                    GameObject.Find("Collision").gameObject.transform.Find("SEESAW_1_1_1Z1Z").transform.localPosition = new Vector3(0f, -0.06f, GameObject.Find("Collision").gameObject.transform.GetChild(5).localPosition.z);
                    GameObject.Find("Collision").gameObject.transform.Find("SEESAW_1_1_1Z2Z").transform.localPosition = new Vector3(0f, -0.06f, GameObject.Find("Collision").gameObject.transform.GetChild(6).localPosition.z);
                    GameObject.Find("Collision").gameObject.transform.Find("SEESAW_1_1_1Z3Z").transform.localPosition = new Vector3(0f, -0.06f, GameObject.Find("Collision").gameObject.transform.GetChild(7).localPosition.z);
                    ObjectDestroyed = true;                                                                           
                }    
                // 8 Seesaws
                if (FindObjectOfType<MainGameStage>().m_StageIndex == 2336 && ObjectDestroyed == false)               
                {
                    GameObject.Find("Collision").gameObject.transform.Find("SEESAW_1_1_1Z1").transform.localPosition = new Vector3(0f, -0.26f, GameObject.Find("Collision").gameObject.transform.GetChild(4).localPosition.z);
                    ObjectDestroyed = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
