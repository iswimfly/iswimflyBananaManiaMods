using UnityEngine;
using Framework.UI;
using Flash2;
using System;

namespace ILBattleTimer
{

    public class Main : MonoBehaviour
    {
        private static bool PerfectBattle = false;
        private static bool ILBattleActive = false;
        private static float ILBattleTotalTime = 18000;
        private static float bestTime;
        private static float frameCount = 0;
        private static GameObject mainGameController;
        private static float minutes = 0;

        // 3600 is 1 min

        public static void OnModUpdate()
        {
            if (MainGame.gameKind != MainGameDef.eGameKind.Practice) return;

            GameObject.Find("c_main_game_0").SetActive(true);
            GameObject.Find("c_main_game_0").transform.GetChild(0).gameObject.SetActive(true);
            GameObject.Find("c_main_game_0").transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);

            mainGameController = FindObjectOfType<MainGameStage>().gameObject;
            
            if (Input.GetKeyDown(KeyCode.LeftShift) && ILBattleActive == false)
            {
                minutes = 0;
                frameCount = 0;
                bestTime = 0;
                ILBattleActive = true;
            }
            if (Input.GetKeyDown(KeyCode.Backspace) && ILBattleActive == false)
            {
                PerfectBattle = !PerfectBattle;
                GameObject.Find("Text_world").GetComponent<RubyTextMeshProUGUI>().text = "Perfect Mode: " + PerfectBattle.ToString();

            }
            // Add a minute to the timer
            if (Input.GetKeyDown(KeyCode.Equals) && ILBattleActive == false)
            {
                ILBattleTotalTime += 3600;
                GameObject.Find("Text_world").GetComponent<RubyTextMeshProUGUI>().text = "Battle Time: " + (ILBattleTotalTime / 3600).ToString() + ":00";
            }
            // Remove a minute from the timer
            if (Input.GetKeyDown(KeyCode.Minus) && ILBattleActive == false)
            {
                ILBattleTotalTime -= 3600;
                if(ILBattleTotalTime <= 0)
                {
                    ILBattleTotalTime = 3600;
                }
                GameObject.Find("Text_world").GetComponent<RubyTextMeshProUGUI>().text = "Battle Time: " + (ILBattleTotalTime / 3600).ToString() + ":00";
            }

            if (ILBattleActive == true)
            {
                double NoRounding = Math.Truncate( (bestTime/60) * 100) / 100;
                // Timer
                frameCount++;
                if (frameCount > 3600)
                {
                    minutes++; 
                    frameCount = 0;
                }

                // Check how long the current attempt has been going
                var CurrentStateFrame = mainGameController.GetComponent<MainGameStage>().m_StateFrame;
                
                // Hijack the Practice Mode/Stage Name
                var ImLazy1 = GameObject.Find("Text_world").gameObject;
                var ImLazy2 = GameObject.Find("Text_stage").gameObject;

                ImLazy1.GetComponent<RubyTextMeshProUGUI>().text = "Best Time: " + NoRounding.ToString("00.00");
                ImLazy2.GetComponent<RubyTextMeshProUGUI>().text = minutes.ToString() + ":" + (frameCount / 60).ToString("00.00") + " Elapsed";

                // Get the Goal Time
                if (mainGameController.GetComponent<MainGameStage>().m_State == MainGameStage.State.GOAL)
                {
                    if (PerfectBattle == true)
                    {
                        if (GameObject.FindObjectOfType<MainGameStage>().isPerfect == true)
                        {
                            bestTime = mainGameController.GetComponent<MainGameStage>().m_GameTimer;
                            
                        }
                    }
                    else
                    {
                        bestTime = mainGameController.GetComponent<MainGameStage>().m_GameTimer;
                    }
                    
                }

                // Emergency Brakes
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    ILBattleActive = false;
                }
                // End the battle when time is up, accounting for Buzzer Beater
                if (minutes * 3600 + frameCount > ILBattleTotalTime && (minutes * 3600 + frameCount) - CurrentStateFrame > ILBattleTotalTime)
                {
                    ILBattleActive = false;
                    ImLazy1.GetComponent<RubyTextMeshProUGUI>().text = "Final Time:";
                    ImLazy2.GetComponent<RubyTextMeshProUGUI>().text = NoRounding.ToString("00.00");
                }
            }

        }
    }
}

