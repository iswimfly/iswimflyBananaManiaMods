using Flash2;
using UnityEngine;
using Unity.Mathematics;
using Console = System.Console;

namespace LimyReq
{
    public class Main : MonoBehaviour
    {
        // Empty GameObject to be assigned the Player GameObject
        private static GameObject thePlayer = null;

        private static GameObject theGame = null;
        // True/False to check if we've stored speed
        private static bool speedStored = false;
        // Vector3 to hold our stored speed
        private static Vector3 goalVelocity = Vector3.zero;

        public static void OnModUpdate()
        {
            // If delete is pressed, clear the stored speed.
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                goalVelocity = Vector3.zero;
                Sound.PlayOneShot(sound_id.cue.se_com_cancel);
                speedStored = false;
                Console.WriteLine("Velocity deleted!");
            }

            // Only run the code below if maingame exists and the player exists
            if (MainGame.Exists != true) return;
            if (FindObjectOfType<Player>() == null) return;

            // Grabs the Player for physics control later
            thePlayer = FindObjectOfType<Player>().gameObject;
            theGame = FindObjectOfType<MainGameStage>().gameObject;

            // If the player spawns in with stored speed, apply it once.
            // Check if they're on the ground,
            // Check if the timer is counting down (Player has control at this point)
            // Check if speed has been Stored
            // Check if the player isn't in the Goal (to prevent stored speed from instantly being reapplied)
            if (thePlayer.GetComponent<MainGamePlayerBall>().m_Flags == PhysicsBall.Flags.M_BALL_GROUND_ON
                && speedStored == true
                && theGame.GetComponent<MainGameStage>().m_State != MainGameStage.State.GOAL)
            {
                thePlayer.GetComponent<MainGamePlayerBall>().m_Velocity = goalVelocity;
                speedStored = false;
            }

            // If the player completes a stage, store their speed and lock out from storing it multiple times
            // Check if the player has entered a goal
            // Check if we've already stored speed
            if (theGame.GetComponent<MainGameStage>().m_State == MainGameStage.State.GOAL
                && speedStored == false)
            {
                speedStored = true;
                Console.WriteLine(goalVelocity.ToString());
                goalVelocity = thePlayer.GetComponent<MainGamePlayerBall>().m_Velocity;
            }
        }
    }
}
