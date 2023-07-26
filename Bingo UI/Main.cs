using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Flash2;
using UnityEngine;
using UnityEngine.UI;
using BMOnline.Mod;
using BMOnline.Mod.Addon;

namespace BingoUI
{
    public class Message
    {
        private float xCoord { get; set; }
        private GameObject message { get; set; } 
        public RectTransform rectTransform { get; set; }
        public float TimeElapsed { get; set; } = 0;
        public Message(string charaKind, string playerName, int goalType, string stage, bool switchStorage, bool fastForward, bool bonusGoal, bool banana50, bool banana100, bool banana120, bool perfectClear, AssetBundle assetBundle, GameObject BingoLog)
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
            // If statement checks if perfect cleared
            if (perfectClear)
            {
                // Make the text golden
                message.transform.GetChild(1).GetComponent<Text>().text = $"{playerName} > <color={goalColors[4]}>{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(stage.ToLower())}</color>";
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
                // If this isn't a perfect clear, just show the color goal icon.
                message.transform.GetChild(1).GetComponent<Text>().text = $"{playerName} > <color={goalColors[goalType]}>{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(stage.ToLower())}</color>";
            }

            // If the bool is true, show the corresponding icon in the message
            message.transform.GetChild(5).gameObject.SetActive(switchStorage);
            message.transform.GetChild(6).gameObject.SetActive(fastForward);
            message.transform.GetChild(7).gameObject.SetActive(bonusGoal);
            message.transform.GetChild(8).gameObject.SetActive(banana50);
            message.transform.GetChild(9).gameObject.SetActive(banana100);
            message.transform.GetChild(10).gameObject.SetActive(banana120);
        }

        public void Update()
        {
            if (TimeElapsed < 1)
            {
                rectTransform.localPosition = new Vector3(Mathf.Lerp(xCoord - 625, xCoord + 10, -Mathf.Cos((((Mathf.Clamp(TimeElapsed, 0, 0.2f)+ 1 )*(float)Math.PI)/0.2f)/2)), rectTransform.localPosition.y, rectTransform.localPosition.z);
                if (TimeElapsed > 0) message.GetComponent<CanvasGroup>().alpha = 1;
            }
            TimeElapsed += Time.unscaledDeltaTime;
        }
    }
    
    public class Counters
    {
        public int perfectClears = 0;
        public int switchStorages = 0;
        public int fastForwards = 0;
        public int greenGoals = 0;
        public int redGoals = 0;
        public int bonusGoals = 0;
        public int bonusPerfects = 0;
        public int banana50 = 0;
        public int banana100 = 0;
        public int banana120 = 0;
    }

    public class PacketData : IAddonRequestPacket
    {
        // Stage to Enter into message
        public uint stageID { get; set; }

        // Color to make goal text
        public byte goaltype { get; set; }

        // Switch Storage
        public byte switchStorage { get; set; }

        // Fast Forward
        public byte fastForward { get; set; } 

        // Bonus Goal (goaltype should be 3 if this is 1)
        public byte bonusGoal { get; set; }

        // 50 Bananas
        public byte banana50 { get; set; }

        // 100 Bananas
        public byte banana100 { get; set; }

        // 120 Bananas
        public byte banana120 { get; set; }

        // Perfect Clear
        public byte perfectClear { get; set; }



        public void Decode(byte[] data)
        {
            stageID = BitConverter.ToUInt32(data, 0);
            goaltype = data[4];
            switchStorage = data[5];
            fastForward = data[6];
            bonusGoal = data[7];
            banana50 = data[8];
            banana100 = data[9];
            banana120 = data[10];
            perfectClear = data[11];
        }

        public byte[] Encode()
        {
            byte[] data = new byte[12];
            BitConverter.GetBytes(stageID).CopyTo(data, 0);
            data[4] = goaltype;
            data[5] = switchStorage;
            data[6] = fastForward;
            data[7] = bonusGoal;
            data[8] = banana50;
            data[9] = banana100;
            data[10] = banana120;
            data[11] = perfectClear;
            return data;
        }

        public void Reset()
        {
            stageID = 0;
            goaltype = 0;
            switchStorage = 0;
            fastForward = 0;
            bonusGoal = 0;
            banana50 = 0;
            banana100 = 0;
            banana120 = 0;
            perfectClear = 0;
        }
    }

    public class Main
    {

        private static IBMOnlineApi api;
        private static IAddonRequestType requestType;
        private static PacketData dataToSend;
        private static bool showCounters = true;
        private static bool showLog = true;
        private static List<Message> messages = new List<Message>();
        public static void OnModLoad(Dictionary<string, object> settings)
        {
            // BMM Config
            showCounters = (bool)settings["ShowCounters"];
            showLog = (bool)settings["ShowCounters"];

            // Set up API for BMOnline
            api = BMOnline.Mod.Main.Api;
            requestType = api.RegisterRelayRequestType(69, 0, () => new PacketData());
            requestType.OnPlayerUpdated += (s, e) => 
            {
                BMOnline.Mod.Players.IOnlinePlayer player = api.PlayerManager.GetPlayer(e.PlayerId);
                PacketData packet = (PacketData)e.Data;
                string stageName = Framework.Text.TextManager.GetText(GameParam.language, $"stagename_st{packet.stageID}", new UnhollowerBaseLib.Il2CppReferenceArray<Il2CppSystem.Object>(Array.Empty<Il2CppSystem.Object>()));
                stageName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(stageName.ToLower());
                if(!showLog) return;
                messages.Add(new Message(player.SelectedCharacter.m_CharaKind.ToString(), player.Name, packet.goaltype, stageName, Convert.ToBoolean(packet.switchStorage), Convert.ToBoolean(packet.fastForward), Convert.ToBoolean(packet.bonusGoal), Convert.ToBoolean(packet.banana50), Convert.ToBoolean(packet.banana100), Convert.ToBoolean(packet.banana120), Convert.ToBoolean(packet.perfectClear), assetBundle, BingoLog));
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

        public static bool playingBingo = false;
        public static MainGameDef.eGoalKind goalKind;
        public static bool perfectStage = false;
        public static bool switchStorage = false;
        public static Counters bingoCounters = new Counters();
        public static bool sentInfo = false;
        public static bool externalChecks = false;
        public static PacketData playerPacketData = new PacketData();
        public static GameObject Counters;
        public static GameObject BingoLog;
        public static MainGameUI.eBananaCounterKind eBananaCounter = MainGameUI.eBananaCounterKind.None;
        public static AssetBundle assetBundle;
        public static Transform messageList;
        // Have perfect count separate
        // If mainGameStage.m_IsOpenResultMenu == true && mainGameStage.isPerfect

        // Console Colors:
        // Blue - Blue Goal
        // Green - Green Goal
        // Red - Red Goal
        // Yellow - Perfect
        // Gray - Bonus Fallout
        // Purple - Bonus Goal

        // Don't have entering modes - adds game knowledge

        // Bonus Logic - FALLOUT - GOAL - GOALKIND.INVALID
        // If MainGameStage.state = eState.FALLOUT && MainGameStage.BananaCounterKind = eBananaCounterKind.Bonus

        public static void OnModStart()
        {
            
        }

        public static void OnModUpdate()
        {
            if (assetBundle == null)
            {
                string DLLFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                assetBundle = AssetBundle.LoadFromFile(Path.Combine(DLLFolder, "bingo"));
            }
            if (BingoLog == null)
            {
                GameObject prefabLog = assetBundle.LoadAsset<GameObject>("Assets/BingoLog.prefab");
                BingoLog = UnityEngine.Object.Instantiate(prefabLog, AppSystemUI.Instance.transform.Find("UIList_GUI_Front").Find("c_system_0").Find("safe_area"));
                BingoLog.SetActive(false);
                messageList = BingoLog.transform.GetChild(0).GetChild(0);
            }
            if (Counters == null)
            {
                GameObject prefabCounters = assetBundle.LoadAsset<GameObject>("Assets/BingoCounters.prefab");
                Counters = UnityEngine.Object.Instantiate(prefabCounters, AppSystemUI.Instance.transform.Find("UIList_GUI_Front").Find("c_system_0").Find("safe_area"));
                Counters.SetActive(false);
            }
            if (!MainGame.Exists || UnityEngine.Object.FindObjectOfType<MainGameStage>() == null)
            {
                sentInfo = false;
                externalChecks = false;
            }
            if (messageList.childCount > 5)
            {
                UnityEngine.Object.Destroy(messageList.GetChild(0).gameObject);
                messages.RemoveAt(0);
            }
            foreach(Message message in messages)
            {
                message.Update();
            }
            // KeyPress to toggle showing the UI
            if (Input.GetKeyDown(KeyCode.F9))
            {
                // Reset Counters
                Console.WriteLine("Reset Counters!");
                bingoCounters.greenGoals = 0;
                bingoCounters.redGoals = 0;
                bingoCounters.perfectClears = 0;
                bingoCounters.fastForwards = 0;
                bingoCounters.switchStorages = 0;
                bingoCounters.bonusGoals = 0;
                bingoCounters.bonusPerfects = 0;
                bingoCounters.banana50 = 0;
                bingoCounters.banana100 = 0;
                bingoCounters.banana120 = 0;
                UpdateCounters();
                playerPacketData.Reset();
                int childCount = messageList.childCount;
                while (childCount > 0)
                {
                    UnityEngine.GameObject.Destroy(messageList.GetChild(0).gameObject);
                    childCount--;
                }
            }
            if (Input.GetKeyDown(KeyCode.F10))
            {
                Console.WriteLine("Game On!");
                playingBingo = !playingBingo;
                if (showCounters)
                {
                    Counters.SetActive(playingBingo);
                }
                if (showLog)
                {
                    BingoLog.SetActive(playingBingo);
                }
            }
            if (!playingBingo) return;
            // If MainGameStage exists and we haven't yet gotten it, get the objects for this stage
            if (UnityEngine.Object.FindObjectOfType<MainGameStage>() != null)
            {
                MainGameStage mainGameStage = UnityEngine.Object.FindObjectOfType<MainGameStage>();
                // Reset the booleans every time the player gets ready (prevents spam)
                if (mainGameStage.state == MainGameStage.State.READY)
                {
                    sentInfo = false;
                    externalChecks = false;
                    switchStorage = false;
                    perfectStage = false;
                    eBananaCounter = mainGameStage.m_BananaCounterKind;
                    playerPacketData.Reset();
                }
                if (mainGameStage.GetPlayer() != null)
                {
                    Player player = mainGameStage.GetPlayer();
                    
                    // Perfect Stage
                    if (mainGameStage.isPerfect && perfectStage != true)
                    {
                        perfectStage = true;
                        playerPacketData.perfectClear = 1;
                    }
                    // Switch Storage
                    if (mainGameStage.gameObject.GetComponent<MgStageSwitchManager>() != null)
                    {
                        if (mainGameStage.gameObject.GetComponent<MgStageSwitchManager>()._isPushedButtonEvenOnce_k__BackingField == true && mainGameStage.state == MainGameStage.State.READY) switchStorage = true;
                    }
                    else
                    {
                        switchStorage = false;
                    }

                    // Don't bother checking things that don't matter until the player completes the level
                    if (sentInfo && externalChecks) return;

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
                                bingoCounters.greenGoals++;
                                playerPacketData.goaltype = 1;
                                sentInfo = true;
                                break;
                            // Red Goal
                            case MainGameDef.eGoalKind.Red:
                                bingoCounters.redGoals++;
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
                        // Fast Forward
                        if (mainGameStage.gameObject.GetComponent<MgStageSwitchManager>() != null)
                        {
                            if (mainGameStage.gameObject.GetComponent<MgStageSwitchManager>().currentState == MainGameDef.StageAnimation.eState.FastForward || mainGameStage.gameObject.GetComponent<MgStageSwitchManager>().currentState == MainGameDef.StageAnimation.eState.FastRewind)
                            {
                                playerPacketData.fastForward = 1;
                                bingoCounters.fastForwards++;
                            }
                        }
                        // Switch Storage
                        if (switchStorage)
                        {
                            playerPacketData.switchStorage = 1;
                            bingoCounters.switchStorages++;
                        }
                        // Perfect Clear
                        if (perfectStage)
                        {
                            playerPacketData.perfectClear = 1;
                            bingoCounters.perfectClears++;
                        }
                        // 50 Count
                        if (mainGameStage.m_HarvestedBananaCount >= 50)
                        {
                            playerPacketData.banana50 = 1;
                            bingoCounters.banana50++;
                        }
                        // 100 Count, Reset 50
                        if (mainGameStage.m_HarvestedBananaCount >= 100)
                        {
                            playerPacketData.banana50 = 0;
                            playerPacketData.banana100 = 1;
                            bingoCounters.banana100++;
                        }
                        // 120 Count, Reset 50 & 100
                        if (mainGameStage.m_HarvestedBananaCount >= 120)
                        {
                            playerPacketData.banana50 = 0;
                            playerPacketData.banana100 = 0;
                            playerPacketData.banana120 = 1;
                            bingoCounters.banana120++;
                        }
                        // Bonus Goal
                        if (eBananaCounter == MainGameUI.eBananaCounterKind.Bonus)
                        {
                            if (player.goalKind != MainGameDef.eGoalKind.Invalid || player.goalKind == MainGameDef.eGoalKind.Invalid && mainGameStage.isPerfect)
                            {
                                playerPacketData.bonusGoal = 1;
                                bingoCounters.bonusGoals++;
                            }
                        }
                        // Bonus Perfect
                        if (eBananaCounter == MainGameUI.eBananaCounterKind.Bonus && perfectStage)
                        {
                            bingoCounters.bonusPerfects++;
                        }
                        UpdateCounters();
                        string stageName = Framework.Text.TextManager.GetText(GameParam.language, $"stagename_st{mainGameStage.stageIndex}", new UnhollowerBaseLib.Il2CppReferenceArray<Il2CppSystem.Object>(Array.Empty<Il2CppSystem.Object>()));
                        stageName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(stageName.ToLower());
                        dataToSend = new PacketData();
                        dataToSend.stageID = (uint)mainGameStage.stageIndex;
                        dataToSend.goaltype = playerPacketData.goaltype;
                        dataToSend.switchStorage = playerPacketData.switchStorage;
                        dataToSend.fastForward = playerPacketData.fastForward;
                        dataToSend.bonusGoal = playerPacketData.bonusGoal;
                        dataToSend.banana50 = playerPacketData.banana50;
                        dataToSend.banana100 = playerPacketData.banana100;
                        dataToSend.banana120 = playerPacketData.banana120;
                        dataToSend.perfectClear = playerPacketData.perfectClear;
                        if (!showLog) return;
                        messages.Add(new Message(player.charaKind.ToString(), SteamManager.GetFriendsHandler().GetPersonaName(), playerPacketData.goaltype, stageName, Convert.ToBoolean(playerPacketData.switchStorage), Convert.ToBoolean(playerPacketData.fastForward), Convert.ToBoolean(playerPacketData.bonusGoal), Convert.ToBoolean(playerPacketData.banana50), Convert.ToBoolean(playerPacketData.banana100), Convert.ToBoolean(playerPacketData.banana120), Convert.ToBoolean(playerPacketData.perfectClear), assetBundle, BingoLog));
                    }
                }
            }
        }

        public static void UpdateCounters()
        {
            GameObject List = Counters.transform.GetChild(0).gameObject;
            List.transform.Find("GreenGoals").GetChild(1).GetComponent<Text>().text = bingoCounters.greenGoals.ToString();
            List.transform.Find("RedGoals").GetChild(1).GetComponent<Text>().text = bingoCounters.redGoals.ToString();
            List.transform.Find("PerfectClears").GetChild(1).GetComponent<Text>().text = bingoCounters.perfectClears.ToString();
            List.transform.Find("FastForwards").GetChild(1).GetComponent<Text>().text = bingoCounters.fastForwards.ToString();
            List.transform.Find("SwitchStorages").GetChild(1).GetComponent<Text>().text = bingoCounters.switchStorages.ToString();
            List.transform.Find("BonusGoals").GetChild(1).GetComponent<Text>().text = bingoCounters.bonusGoals.ToString();
            List.transform.Find("BonusPerfects").GetChild(1).GetComponent<Text>().text = bingoCounters.bonusPerfects.ToString();
            List.transform.Find("Banana50s").GetChild(1).GetComponent<Text>().text = bingoCounters.banana50.ToString();
            List.transform.Find("Banana100s").GetChild(1).GetComponent<Text>().text = bingoCounters.banana100.ToString();
            List.transform.Find("Banana120s").GetChild(1).GetComponent<Text>().text = bingoCounters.banana120.ToString();
        }


    }
}
