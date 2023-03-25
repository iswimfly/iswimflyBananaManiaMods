using Flash2;
using UnityEngine;
using Unity.Mathematics;

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
            // If delete is pressed, clear the stored speed.
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                storedSpeed = Vector3.zero;
                goaldone = false;
                Sound.PlayOneShot(sound_id.cue.se_com_cancel);
            }

            // Only run the code below if maingame exists and the player exists
            if (MainGame.Exists != true) return;
            if (FindObjectOfType<Player>() == null) return;

            // Grabs the Main Game controller for later use
            mainGameController = FindObjectOfType<MainGameStage>().gameObject;
            // Grabs the Player for physics control later
            thePlayer = FindObjectOfType<Player>().gameObject;

            // If the player spawns in with stored speed, apply it once.
            if (thePlayer.GetComponent<MainGamePlayerBall>().m_Flags == Flash2.PhysicsBall.Flags.M_BALL_GROUND_ON
                && goaldone == true 
                && storedSpeed != Vector3.zero
                && mainGameController.GetComponent<MainGameStage>().m_State != MainGameStage.State.GOAL)
            {
                thePlayer.GetComponent<MainGamePlayerBall>().m_Velocity = storedSpeed;
                goaldone = false;
            }

            // If the player completes a stage, store their speed and lock out from storing it multiple times
            if (mainGameController.GetComponent<MainGameStage>().m_State == MainGameStage.State.GOAL 
                && goaldone != true)
            {
                goaldone = true;
                storedSpeed = thePlayer.GetComponent<MainGamePlayerBall>().m_Velocity;
            }
        }
    }
}
