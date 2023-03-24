using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flash2;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LimyReq
{
    public class Main : MonoBehaviour
    {
        private static GameObject mainGameController;
        private static GameObject thePlayer;
        private static bool goaldone;
        private static Vector3 storedSpeed;

        public static void OnModUpdate()
        {
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                storedSpeed = new Vector3(0, 0, 0);
            }
            if (MainGame.Exists != true) return;
            if (FindObjectOfType<Player>() == null) return;
            // MainGameStage
            mainGameController = FindObjectOfType<MainGameStage>().gameObject;
            // PlayerBall
            thePlayer = FindObjectOfType<Player>().gameObject;
            if (thePlayer.GetComponent<MainGamePlayerBall>().m_Flags == Flash2.PhysicsBall.Flags.M_BALL_GROUND_ON && goaldone == true)
            {
                thePlayer.GetComponent<MainGamePlayerBall>().m_Velocity = storedSpeed;
                goaldone = false;
            }
            if (mainGameController.GetComponent<MainGameStage>().m_State == MainGameStage.State.GOAL && goaldone != true)
            {
                goaldone = true;
                storedSpeed = thePlayer.GetComponent<MainGamePlayerBall>().m_Velocity;
            }
        }
    }
}
