using UnityEngine;
using Console = System.Console;
using Flash2;

namespace FixingOGLongTorus
{

    public class Main : MonoBehaviour
    {
        private static GameObject Timer = null;
        public static void OnModUpdate()
        {
            if (MainGame.Exists == true)
            {
                if (Timer != null)
                {
                    if (Timer.transform.GetComponent<MainGameStage>().m_GameTimer == 3600)
                    {
                        Console.WriteLine(Timer.transform.GetComponent<MainGameStage>().m_StateFrame);
                    }
                }
                else
                {
                    Timer = FindObjectOfType<MainGameStage>().gameObject;
                }
            }
            else
            {
                FranciscoDestroyed = false;
            }
        }
    }
}
