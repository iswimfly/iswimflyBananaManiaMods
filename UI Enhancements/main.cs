using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Globalization;
using Flash2;
using Framework.UI;
using Il2CppSystem;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using Object = UnityEngine.Object;


namespace MoveRankingTimer
{
    public class Main
    {
        public static bool RankingChallengeMoveTimer { get; set; } = true;
        public static float RankingChallengePositionX { get; set; } = 365;
        public static float RankingChallengePositionY { get; set; } = 860;
        public static bool PauseMenuChanges { get; set; } = true;
        public static bool RunInBackground { get; set; } = true;
        public static bool CharaIconOverArrow { get; set; } = true;
        public static void OnModLoad(Dictionary<string, object> settings)

        {
            RankingChallengeMoveTimer = (bool)settings["RankingChallengeMoveTimer"];
            RankingChallengePositionX = (float)settings["RankingChallengePositionX"];
            RankingChallengePositionY = (float)settings["RankingChallengePositionY"];
            PauseMenuChanges = (bool)settings["PauseMenuChanges"];
            RunInBackground = (bool)settings["RunInBackground"];
            CharaIconOverArrow = (bool)settings["CharacterIconOverArrow"];
        }
        private static GameObject timer = null;
        private static bool timerChanged = false;
        private static GameObject pausebg = null;
        private static GameObject pausechara = null;
        private static bool pauseMenuChanged = false;
        private static GameObject blackbg = null;
        private static GameObject buttonparent = null;
        private static Image arrow = null;
        private static GameObject SoundController = null;
        private static AssetBundle assetBundle = null;

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
            if (UnityEngine.Object.FindObjectOfType<MainGameStage>() == null) return;
            MainGameStage mainGameStage = UnityEngine.Object.FindObjectOfType<MainGameStage>();
            // change the position
            if (timer != null && RankingChallengeMoveTimer == true && !timerChanged)
            {
                timer.GetComponent<TimeAttackView>().m_TimerObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(RankingChallengePositionX, RankingChallengePositionY);
            }
            // look for the timer
            else if (timer == null)
            {
                timer = GameObject.Find("c_time_attack_0");
            }
            // modify pause menu
            if (pausebg != null && PauseMenuChanges == true && !pauseMenuChanged)
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
            else if (pausebg == null)
            {
                pausebg = GameObject.Find("pause_bg");
                blackbg = GameObject.Find("pt_button_info_base");
                pausechara = GameObject.Find("pos_pause_chara");
                buttonparent = GameObject.Find("pos_sel");
            }
            // Arrow logic

            if (!CharaIconOverArrow) return;
            // Keep AssetBundle loaded
            if (assetBundle == null)
            {
                string DLLFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                assetBundle = AssetBundle.LoadFromFile(Path.Combine(DLLFolder, "icons"));
            }

            // Don't bother checking the rest of the code if the Main Game isn't loaded

            Player player;
            Chara.eKind charaKind;

            // If Player is null, don't bother getting info
            if (mainGameStage.GetPlayer() != null)
            {
                player = mainGameStage.GetPlayer();
                charaKind = player.charaKind;
                if (arrow != null && charaKind != Chara.eKind.Invalid)
                {
                    // If it's Baby Robo, use the special icon
                    if (charaKind == Chara.eKind.Baby && player.charaSkinIndex == 2)
                    {
                        arrow.sprite = assetBundle.LoadAsset<Sprite>($"Assets/Icons/babyrobo.png");
                        arrow.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(75, 75);
                        
                    }
                    // If it's literally anyone else, just use their charaKind
                    else
                    {
                        arrow.sprite = assetBundle.LoadAsset<Sprite>($"Assets/Icons/{charaKind.ToString().ToLower().Replace(" ", "")}.png");
                        arrow.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(75, 75);
                    }
                }
                else if (GameObject.Find("pt_main_game_map_arrow_p_Variant(Clone)") != null)
                {
                    arrow = GameObject.Find("pt_main_game_map_arrow_p_Variant(Clone)").transform.GetChild(0).GetComponent<Image>();
                }
            }
        }
    }
}   