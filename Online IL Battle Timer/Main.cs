using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Flash2;
using Framework.UI;
using UnityEngine;
using UnityEngine.UI;
using BMOnline.Mod;
using BMOnline.Mod.Addon;

namespace OnlineILBattleTimer
{

    public class Message
    {
        private float xCoord { get; set; }
        public GameObject message { get; set; }
        public RectTransform rectTransform { get; set; }
        public float TimeElapsed { get; set; } = 0;
        public Message(string charaKind, string playerName, int goalType, int time, bool perfectClear, string stage, bool bonusGoal, bool finalTime, AssetBundle assetBundle, GameObject BingoLog)
        {
            Dictionary<int, string> goalColors = new Dictionary<int, string>()
            { 
                // Blue Goal
                {0, "#0389ff"},

                // Green Goal
                {1, "#00ff40"},

                // Red Goal
                {2, "#ff475a"},

                // Bonus Fallout (Gray)
                {3, "#858585"},

                // Perfect Clear
                {4, "#ffd000"}
            };

            GameObject messagePrefab = assetBundle.LoadAsset<GameObject>("Assets/Message.prefab");
            Transform messageList = BingoLog.transform.GetChild(0).GetChild(0);
            message = UnityEngine.Object.Instantiate(messagePrefab, messageList);
            rectTransform = message.GetComponent<RectTransform>();
            xCoord = rectTransform.localPosition.x;
            // rectTransform.localPosition = new Vector3(rectTransform.localPosition.x - 625, rectTransform.localPosition.y, rectTransform.localPosition.z);
            message.transform.GetChild(0).GetComponent<Image>().sprite = assetBundle.LoadAsset<Sprite>($"Assets/Icons/{charaKind.ToLower().Replace(" ", "")}.png");
            double CorrectedTime = (time * .01);
            // If statement checks if perfect cleared
            if (perfectClear)
            {
                // Make the text golden
                message.transform.GetChild(1).GetComponent<Text>().text = $"{playerName} > <color={goalColors[4]}>{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(stage.ToLower())}</color> > {CorrectedTime.ToString("00.00")}";
                // If this is a bonus stage, don't show the color goal icon.
                // If we got this far, show the color goal icon.
                switch (goalType)
                {
                    case 0:
                        if (bonusGoal) break;
                        message.transform.GetChild(2).gameObject.SetActive(true);
                        break;
                    case 1:
                        if (bonusGoal) break;
                        message.transform.GetChild(3).gameObject.SetActive(true);
                        break;
                    case 2:
                        if (bonusGoal) break;
                        message.transform.GetChild(4).gameObject.SetActive(true);
                        break;
                }
            }
            else
            {
                // If this isn't a perfect clear, just color the text of the player.
                message.transform.GetChild(1).GetComponent<Text>().text = $"{playerName} > <color={goalColors[goalType]}>{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(stage.ToLower())}</color> > {CorrectedTime.ToString("00.00")}";
            }

            // I'm too lazy to make a new prefab so we're disabling everything extra in the Bingo message prefab
            message.transform.GetChild(5).gameObject.SetActive(finalTime);
            message.transform.GetChild(6).gameObject.SetActive(false);
            message.transform.GetChild(7).gameObject.SetActive(false);
            message.transform.GetChild(8).gameObject.SetActive(false);
            message.transform.GetChild(9).gameObject.SetActive(false);
            message.transform.GetChild(10).gameObject.SetActive(false);
        }

        public void Update()
        {
            if (TimeElapsed < 1)
            {
                rectTransform.localPosition = new Vector3(Mathf.Lerp(xCoord - 625, xCoord + 10, -Mathf.Cos((((Mathf.Clamp(TimeElapsed, 0, 0.2f) + 1) * (float)Math.PI) / 0.2f) / 2)), rectTransform.localPosition.y, rectTransform.localPosition.z);
                if (TimeElapsed > 0) message.GetComponent<CanvasGroup>().alpha = 1;
            }
            TimeElapsed += Time.unscaledDeltaTime;
        }
    }

    public class PacketData : IAddonRequestPacket
    {
        public uint time { get; set; }
        public uint stageID { get; set; }
        public byte goaltype { get; set; }
        public byte perfectClear { get; set; }
        public byte bonusGoal { get; set; }

        public byte finalTime { get; set; }
        public void Decode(byte[] data)
        {
            time = BitConverter.ToUInt32(data, 0);
            stageID = BitConverter.ToUInt32(data, 4);
            goaltype = data[8];
            perfectClear = data[9];
            bonusGoal = data[10];
            finalTime = data[11];
        }

        public byte[] Encode()
        {
            byte[] data = new byte[12];
            BitConverter.GetBytes(time).CopyTo(data, 0);
            BitConverter.GetBytes(stageID).CopyTo(data, 4);
            data[8] = goaltype;
            data[9] = perfectClear;
            data[10] = bonusGoal;
            data[11] = finalTime;
            return data;
        }

        public void Reset()
        {
            stageID = 0;
            goaltype = 0;
            bonusGoal = 0;
            perfectClear = 0;
            finalTime = 0;
        }
    }

    public class Main
    {
        private static bool PerfectBattle = false;
        private static bool ILBattleActive = false;
        // 3600 is 1 min, so the default is 5
        private static float ILBattleTotalTime = 18000;
        private static float bestTime;
        private static float frameCount = 0;
        private static float minutes = 0;
        private static bool sendFinalTime = false;

        // New stuff

        private static IBMOnlineApi api;
        private static IAddonRequestType requestType;
        private static bool sentInfo = false;
        private static bool externalChecks = false;
        public static PacketData playerPacketData = new PacketData();
        private static PacketData dataToSend;
        private static bool perfectStage;
        private static bool showLog;
        private static bool LogVisible = false;
        public static AssetBundle assetBundle;
        public static GameObject TimeLog;
        public static Transform messageList;
        public static MainGameUI.eBananaCounterKind eBananaCounter;
        private static List<Message> messages = new List<Message>();

        public static void OnModLoad(Dictionary<string, object> settings)
        {
            // BMM Config
            showLog = (bool)settings["ShowLog"];

            // Set up API for BMOnline
            api = BMOnline.Mod.Main.Api;
            requestType = api.RegisterRelayRequestType(71, 0, () => new PacketData());
            requestType.OnPlayerUpdated += (s, e) =>
            {
                BMOnline.Mod.Players.IOnlinePlayer player = api.PlayerManager.GetPlayer(e.PlayerId);
                PacketData packet = (PacketData)e.Data;
                string stageName = Framework.Text.TextManager.GetText(GameParam.language, $"stagename_st{packet.stageID}", new UnhollowerBaseLib.Il2CppReferenceArray<Il2CppSystem.Object>(Array.Empty<Il2CppSystem.Object>()));
                stageName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(stageName.ToLower());
                if (!showLog) return;
                if (player.SelectedCharacter.m_CharaKind == Chara.eKind.Baby && player.SelectedCharacter.m_SkinIndex == 2)
                {
                    messages.Add(new Message("babyrobo", player.Name, packet.goaltype, (int)packet.time, Convert.ToBoolean(packet.perfectClear), stageName, Convert.ToBoolean(packet.bonusGoal), Convert.ToBoolean(packet.finalTime), assetBundle, TimeLog));
                }
                else
                {
                    messages.Add(new Message(player.SelectedCharacter.m_CharaKind.ToString(), player.Name, packet.goaltype, (int)packet.time, Convert.ToBoolean(packet.perfectClear), stageName, Convert.ToBoolean(packet.bonusGoal), Convert.ToBoolean(packet.finalTime), assetBundle, TimeLog));
                }
            };
            api.DoStateUpdate += (s, e) =>
            {
                if (dataToSend != null)
                {
                    requestType.SendData(dataToSend);
                    dataToSend = null;
                }
            };
        }

        public static void OnModUpdate()
        {
            if (assetBundle == null)
            {
                string DLLFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                assetBundle = AssetBundle.LoadFromFile(Path.Combine(DLLFolder, "ilbattletimer"));
            }
            if (TimeLog == null)
            {
                GameObject prefabLog = assetBundle.LoadAsset<GameObject>("Assets/IlBattleTimerLog.prefab");
                TimeLog = UnityEngine.Object.Instantiate(prefabLog, AppSystemUI.Instance.transform.Find("UIList_GUI_Front").Find("c_system_0").Find("safe_area"));
                TimeLog.SetActive(showLog);
                LogVisible = showLog;
                messageList = TimeLog.transform.GetChild(0).GetChild(0);
            }

            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    LogVisible = !LogVisible;
                    TimeLog.SetActive(LogVisible);
                }
                if (Input.GetKeyDown(KeyCode.Backspace))
                {
                    foreach (Message message in messages)
                    {
                        UnityEngine.Object.Destroy(message.message);
                    }
                }
            }
            foreach (Message message in messages)
            {
                message.Update();
            }
            if (MainGame.Exists == true)
            {
                // Left Shift starts the Battle
                if (Input.GetKeyDown(KeyCode.LeftShift) && ILBattleActive == false)
                {
                    minutes = 0;
                    frameCount = 0;
                    bestTime = 0;
                    ILBattleActive = true;
                }
                // Backspace makes it a Perfect or non Perfect IL Battle (Imperfects will just need to not be in Perfect Mode)
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
                    if (ILBattleTotalTime <= 0)
                    {
                        ILBattleTotalTime = 3600;
                    }
                    GameObject.Find("Text_world").GetComponent<RubyTextMeshProUGUI>().text = "Battle Time: " + (ILBattleTotalTime / 3600).ToString() + ":00";
                }

                if (ILBattleActive == true)
                {
                    // IL Battle Stuff Never Disappears
                    GameObject.Find("c_main_game_0").SetActive(true);
                    GameObject.Find("c_main_game_0").transform.GetChild(0).gameObject.SetActive(true);
                    GameObject.Find("c_main_game_0").transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);

                    // Find the MainGameStage for usage later
                    MainGameStage mainGameStage = UnityEngine.Object.FindObjectOfType<MainGameStage>();
                    Player player = null;
                    if (mainGameStage.state == MainGameStage.State.READY)
                    {
                        sentInfo = false;
                        externalChecks = false;
                        perfectStage = false;
                        eBananaCounter = mainGameStage.m_BananaCounterKind;
                        playerPacketData.Reset();
                    }

                    // Fix the rounding up issue for bestTime
                    double NoRounding = Math.Truncate((bestTime / 60) * 100) / 100;

                    // Internal Timer
                    frameCount++;
                    if (frameCount > 3600)
                    {
                        minutes++;
                        frameCount = 0;
                    }

                    // Check how long the current attempt has been going
                    var CurrentStateFrame = mainGameStage.m_StateFrame;

                    // Hijack the Practice Mode/Stage Name
                    var ImLazy1 = GameObject.Find("Text_world").gameObject;
                    var ImLazy2 = GameObject.Find("Text_stage").gameObject;

                    ImLazy1.GetComponent<RubyTextMeshProUGUI>().text = "Best Time: " + NoRounding.ToString("00.00");
                    ImLazy2.GetComponent<RubyTextMeshProUGUI>().text = minutes.ToString() + ":" + (frameCount / 60).ToString("00.00") + " Elapsed";

                    // Get the Goal Time
                    if (mainGameStage.m_State == MainGameStage.State.GOAL || mainGameStage.state == MainGameStage.State.FALLOUT && mainGameStage.m_BananaCounterKind == MainGameUI.eBananaCounterKind.Bonus)
                    {
                        // Check if it's a Perfect IL Battle
                        if (PerfectBattle == true)
                        {
                            if (mainGameStage.isPerfect == true)
                            {
                                if (bestTime < mainGameStage.m_GameTimer)
                                {
                                    bestTime = mainGameStage.m_GameTimer;
                                }
                            }
                        }
                        // If it's not a Perfect IL Battle just get the time
                        else
                        {
                            if (bestTime < mainGameStage.m_GameTimer)
                            {
                                bestTime = mainGameStage.m_GameTimer;
                            }
                        }
                    }

                    // Emergency Stop
                    if (Input.GetKeyDown(KeyCode.Tab))
                    {
                        ILBattleActive = false;
                    }
                    // End the battle when time is up, accounting for Buzzer Beater
                    if (minutes * 3600 + frameCount > ILBattleTotalTime && (minutes * 3600 + frameCount) - CurrentStateFrame > ILBattleTotalTime)
                    {
                        sendFinalTime = true;
                        playerPacketData.finalTime = 1;
                        NoRounding = Math.Truncate((bestTime / 60) * 100) / 100;
                        ILBattleActive = false;
                        ImLazy1.GetComponent<RubyTextMeshProUGUI>().text = "Final Time:";
                        ImLazy2.GetComponent<RubyTextMeshProUGUI>().text = NoRounding.ToString("00.00");
                    }
                    if (mainGameStage.GetPlayer() != null)
                    {
                        player = mainGameStage.GetPlayer();

                        if (sentInfo && externalChecks || sendFinalTime) return;

                        // Start Making Checks once the state changes to goal
                        if (!sentInfo && mainGameStage.state == MainGameStage.State.GOAL || mainGameStage.state == MainGameStage.State.FALLOUT && eBananaCounter == MainGameUI.eBananaCounterKind.Bonus)
                        {
                            // If the player fell out on a bonus stage
                            if (mainGameStage.state == MainGameStage.State.FALLOUT && eBananaCounter == MainGameUI.eBananaCounterKind.Bonus)
                            {
                                playerPacketData.goaltype = 3;
                                sentInfo = true;
                            }
                            // If the player went in a goal
                            switch (player.goalKind)
                            {
                                // Blue Goal
                                case MainGameDef.eGoalKind.Blue:
                                    playerPacketData.goaltype = 0;
                                    sentInfo = true;
                                    break;
                                // Green Goal
                                case MainGameDef.eGoalKind.Green:
                                    playerPacketData.goaltype = 1;
                                    sentInfo = true;
                                    break;
                                // Red Goal
                                case MainGameDef.eGoalKind.Red:
                                    playerPacketData.goaltype = 2;
                                    sentInfo = true;
                                    break;
                                // Catch for invalids (fallouts)
                                case MainGameDef.eGoalKind.Invalid:
                                    playerPacketData.goaltype = 3;
                                    sentInfo = true;
                                    break;
                            }
                        }
                        // Once we reach the results menu, run checks for every possible goal
                        if (mainGameStage.state == MainGameStage.State.GOAL && mainGameStage.m_IsOpenResultScore == true && !externalChecks)
                        {
                            externalChecks = true;
                            if (mainGameStage.isPerfect) perfectStage = true;
                            // Perfect Clear
                            if (perfectStage)
                            {
                                playerPacketData.perfectClear = 1;
                            }
                            // Bonus Goal
                            if (eBananaCounter == MainGameUI.eBananaCounterKind.Bonus)
                            {
                                if (player.goalKind != MainGameDef.eGoalKind.Invalid || player.goalKind == MainGameDef.eGoalKind.Invalid && mainGameStage.isPerfect)
                                {
                                    playerPacketData.bonusGoal = 1;
                                }
                            }
                            string stageName = Framework.Text.TextManager.GetText(GameParam.language, $"stagename_st{mainGameStage.stageIndex}", new UnhollowerBaseLib.Il2CppReferenceArray<Il2CppSystem.Object>(Array.Empty<Il2CppSystem.Object>()));
                            stageName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(stageName.ToLower());
                            dataToSend = new PacketData();
                            dataToSend.stageID = (uint)mainGameStage.stageIndex;
                            dataToSend.time = (uint)Convert.ToInt32(NoRounding * 100);
                            dataToSend.goaltype = playerPacketData.goaltype; ;
                            dataToSend.bonusGoal = playerPacketData.bonusGoal;
                            dataToSend.perfectClear = playerPacketData.perfectClear;
                            dataToSend.finalTime = playerPacketData.finalTime;
                            if (!showLog) return;
                            if (bestTime > mainGameStage.m_GameTimer && PerfectBattle && !mainGameStage.isPerfect) return;
                            if  (bestTime > mainGameStage.m_GameTimer) return;
                            if (player.charaKind == Chara.eKind.Baby && player.charaSkinIndex == 2)
                            {
                                messages.Add(new Message("babyrobo", SteamManager.GetFriendsHandler().GetPersonaName(), playerPacketData.goaltype, MakeDoubleInt(NoRounding * 100), Convert.ToBoolean(playerPacketData.perfectClear), stageName, Convert.ToBoolean(playerPacketData.bonusGoal), Convert.ToBoolean(playerPacketData.finalTime), assetBundle, TimeLog));
                            }
                            else
                            {
                                messages.Add(new Message(player.charaKind.ToString(), SteamManager.GetFriendsHandler().GetPersonaName(), playerPacketData.goaltype, MakeDoubleInt(NoRounding * 100), Convert.ToBoolean(playerPacketData.perfectClear), stageName, Convert.ToBoolean(playerPacketData.bonusGoal), Convert.ToBoolean(playerPacketData.finalTime), assetBundle, TimeLog));
                            }
                        }
                    }

                }
            }
            else
            {
                if (MainGame.Exists)
                {
                    GameObject.Find("Text_world").GetComponent<RubyTextMeshProUGUI>().text = "Best Time: 00.00";
                }
                minutes = 0;
                frameCount = 0;
                bestTime = 0;
                ILBattleActive = false;
                sendFinalTime = false;
                sentInfo = false;
                externalChecks = false;
            }
        }
        static int MakeDoubleInt(double x)
        {
            return int.Parse(x.ToString().Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, ""));
        }

    }

}
