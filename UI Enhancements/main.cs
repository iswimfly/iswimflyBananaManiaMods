using System.Collections.Generic;
using Flash2;
using Framework.UI;
using Il2CppSystem;
using Il2CppSystem.Xml;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using Object = UnityEngine.Object;


namespace MoveRankingTimer
{
    public static class Main
    {
        public static bool RankingChallengeMoveTimer { get; set; } = true;
        public static float RankingChallengePositionX { get; set; } = 365;
        public static float RankingChallengePositionY { get; set; } = 860;
        public static bool PauseMenuChanges { get; set; } = false;
        public static bool RunInBackground { get; set; } = true;
        public static void OnModLoad(Dictionary<string, object> settings)

        {
            RankingChallengeMoveTimer = (bool)settings["RankingChallengeMoveTimer"];
            RankingChallengePositionX = (float)settings["RankingChallengePositionX"];
            RankingChallengePositionY = (float)settings["RankingChallengePositionY"];
            PauseMenuChanges = (bool)settings["PauseMenuChanges"];
            RunInBackground = (bool)settings["RunInBackground"];
        }
        private static GameObject timer = null;
        private static GameObject pausebg = null;
        private static GameObject pausechara = null;
        private static GameObject blackbg = null;
        private static GameObject buttonparent = null;
        private static GameObject SoundController = null;
        private static GameObject arrow = null;
        private static GameObject player = null;
        private static GameObject playericon = null
        private static Flash2.Chara.eKind character;
        private static Sprite icon;

        public static void OnModLateUpdate()
        {
            if (RunInBackground == true)
            {
                SoundController = GameObject.Find("Sound");
                SoundController.GetComponent<Sound>().OnApplicationFocus(RunInBackground);
            }
        }

        public static void OnModUpdate()
        {
            // change the position
            if (timer != null && RankingChallengeMoveTimer == true)
            {
                timer.GetComponent<TimeAttackView>().m_TimerObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(RankingChallengePositionX, RankingChallengePositionY);
            }
            // look for the timer
            else
            {
                timer = GameObject.Find("c_time_attack_0");
            }
            // modify pause menu
            if (pausebg != null && PauseMenuChanges == true)
            {
                pausechara.SetActive(false);
                pausebg.GetComponent<Image>().GetComponent<Behaviour>().enabled = false;
                blackbg.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Graphic>().color = new Color(0, 0, 0, (float)0.75);
                blackbg.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(1, 40, 0);
                for (int i = 0; i < pausebg.transform.childCount; i++)
                {
                    pausebg.transform.GetChild(i).gameObject.SetActive(false);
                }
                for (int i = 0; i < buttonparent.transform.childCount; i++)
                {
                    buttonparent.transform.GetChild(i).gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(10, -30 + i * -90);
                }
            }
            else
            {
                pausebg = GameObject.Find("pause_bg");
                blackbg = GameObject.Find("pt_button_info_base");
                pausechara = GameObject.Find("pos_pause_chara");
                buttonparent = GameObject.Find("pos_sel");
            }
        }
    }
}   