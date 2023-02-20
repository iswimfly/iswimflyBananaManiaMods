using UnityEngine;
using Console = System.Console;
using Flash2;

namespace FixingOGLongTorus
{
   
    public class Main : MonoBehaviour
    {
        private static bool FrancisDestroyed = false;
        private static bool FranciscoDestroyed = false;
        private static GameObject Francis = null;
        private static GameObject LongTorus = null;
        private static GameObject Francisco = null;
        private static GameObject DoubleSpiral = null;
        public static void OnModUpdate()
        {
            if (MainGame.Exists == true)
            {
                if (LongTorus != null)
                {
                    if (FrancisDestroyed == false)
                    {
                        Francis = GameObject.Find("col2");
                        Destroy(Francis);
                        FrancisDestroyed = true;
                    }
                }
                else
                {
                    LongTorus = GameObject.Find("NODISP_STAGE2327");
                }
                if (DoubleSpiral != null)
                {
                    if (FranciscoDestroyed == false)
                    {
                        Francisco = GameObject.Find("col5");
                        Destroy(Francisco);
                        FranciscoDestroyed = true;
                    }
                }
                else
                {
                    DoubleSpiral = GameObject.Find("NODISP_STAGE2287");
                }
            }
            else
            {
                FrancisDestroyed = false;
                FranciscoDestroyed = false;
            }
        }
    }
}
