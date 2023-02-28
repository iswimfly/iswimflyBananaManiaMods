using UnityEngine;
using Flash2;
using Framework.UI;

namespace PracticeMod
{

    public class Main : MonoBehaviour
    {
        private static GameObject maingame = null;
        private static GameObject pausemenu = null;
        private static GameObject player = null;
        private static GameObject playercamera = null;
        public static void OnModUpdate()
        {
            if (MainGame.Exists != true) return;

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                playercamera = GameObject.Find("ObjRoot").transform.GetChild(5).gameObject;
                if (playercamera.name == "GameCameraShimmer(Clone)")
                {
                    playercamera = GameObject.Find("CameraMain");
                }
                pausemenu = FindObjectOfType<Pause>().gameObject;
                player = GameObject.Find("Player(Clone)");
                maingame = FindObjectOfType<MainGame>().gameObject;

                if (playercamera.GetComponent<Camera>().isActiveAndEnabled && pausemenu.GetComponent<Pause>().currentPlayerIndex != 0 && maingame != null && player != null)
                {
                    maingame.transform.GetComponent<MainGame>().m_isRequestRecreateStage = true;
                }
            }
        }
    }
}
