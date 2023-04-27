using UnityEngine;
using Flash2;
using Il2CppSystem;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BrainOff
{
	public class Main : MonoBehaviour
	{
		private static readonly Dictionary<int, Chara.eKind> characterKind = new Dictionary<int, Chara.eKind>()
		{
			{100, Chara.eKind.Aiai},
			{101, Chara.eKind.Aiai},
			{102, Chara.eKind.Aiai},
			{103, Chara.eKind.Aiai},
			{200, Chara.eKind.Meemee},
			{201, Chara.eKind.Meemee},
			{202, Chara.eKind.Baby},
			{300, Chara.eKind.Baby},
			{301, Chara.eKind.Baby},
			{302, Chara.eKind.Baby},
			{303, Chara.eKind.Baby},
			{400, Chara.eKind.Gongon},
			{401, Chara.eKind.Gongon},
			{402, Chara.eKind.Gongon},
			{500, Chara.eKind.Yanyan},
			{501, Chara.eKind.Yanyan},
			{502, Chara.eKind.Yanyan},
			{600, Chara.eKind.Doctor},
			{601, Chara.eKind.Doctor},
			{602, Chara.eKind.Doctor},
			{700, Chara.eKind.Jam},
			{701, Chara.eKind.Jam},
			{800, Chara.eKind.Jet},
			{801, Chara.eKind.Jet},
			{900, Chara.eKind.Sonic},
			{1000, Chara.eKind.Tails},
			{1100, Chara.eKind.Kiryu},
			{1200, Chara.eKind.Beat},
			{1300, Chara.eKind.Dlc01},
			{1400, Chara.eKind.Dlc02},
			{1500, Chara.eKind.Dlc03},
			{1600, Chara.eKind.GameGear},
			{1700, Chara.eKind.SegaSaturn},
			{1800, Chara.eKind.Dreamcast}
		};

		private static readonly Dictionary<int, string> bananaReplacer = new Dictionary<int, string>()
		{
			{900, "Sonic"},
			{1000, "Sonic"},
			{1100, "Kiryu"},
			{1200, "Beat"},
			{1300, "DLC01"},
			{1400, "DLC02"},
			{1500, "DLC03"},
		};

		private static readonly Dictionary<int, string> prefabs = new Dictionary<int, string>()
		{
			{100, "Player/Chara/0100Aiai/PlayerAiai.prefab"},
			{101, "Player/Chara/0101AiaiDstyle/PlayerAiaiDstyle.prefab"},
			{102, "Player/Chara/0102AiaiCstyle/PlayerAiaiCstyle.prefab"},
			{103,  "Player/Chara/0103AiaiGold/PlayerAiaiGold.prefab"},
			{200, "Player/Chara/0200Meemee/PlayerMeemee.prefab"},
			{201, "Player/Chara/0201MeemeeDstyle/PlayerMeemeeDstyle.prefab"},
			{202, "Player/Chara/0202MeemeeCstyle/PlayerMeemeeCstyle.prefab"},
			{300, "Player/Chara/0300Baby/PlayerBaby.prefab"},
			{301, "Player/Chara/0301BabyDstyle/PlayerBabyDstyle.prefab"},
			{302, "Player/Chara/0302BabyRobo/PlayerBabyRobo.prefab"},
			{303, "Player/Chara/0303BabyCstyle/PlayerBabyCstyle.prefab"},
			{400, "Player/Chara/0400Gongon/PlayerGongon.prefab"},
			{401, "Player/Chara/0401GongonDstyle/PlayerGongonDstyle.prefab"},
			{402, "Player/Chara/0402GongonCstyle/PlayerGongonCstyle.prefab"},
			{500, "Player/Chara/0500Yanyan/PlayerYanyan.prefab"},
			{501, "Player/Chara/0501YanyanDstyle/PlayerYanyanDstyle.prefab"},
			{502, "Player/Chara/0502YanyanCstyle/PlayerYanyanCstyle.prefab"},
			{600, "Player/Chara/0600Doctor/PlayerDoctor.prefab"},
			{601, "Player/Chara/0601DoctorDstyle/PlayerDoctorDstyle.prefab"},
			{602, "Player/Chara/0602DoctorCstyle/PlayerDoctorCstyle.prefab"},
			{700, "Player/Chara/0700Jam/PlayerJam.prefab"},
			{701, "Player/Chara/0701Jam_N/PlayerJam_N.prefab"},
			{800, "Player/Chara/0800Jet/PlayerJet.prefab"},
			{801, "Player/Chara/0801Jet_B/PlayerJet_B.prefab"},
			{900, "Player/Chara/0900Sonic/PlayerSonic.prefab"},
			{1000, "Player/Chara/1000Tails/PlayerTails.prefab"},
			{1100, "Player/Chara/1100Kiryu/PlayerKiryu.prefab"},
			{1200, "Player/Chara/1200Beat/PlayerBeat.prefab"},
			{1300, "Player/Chara/1300Dlc01/PlayerDlc01.prefab"},
			{1400, "Player/Chara/1400Dlc02/PlayerDlc02.prefab"},
			{1500, "Player/Chara/1500Dlc03/PlayerDlc03.prefab"},
			{1600, "Player/Chara/1600GameGear/PlayerGameGear.prefab"},
			{1700, "Player/Chara/1700SegaSaturn/PlayerSegaSaturn.prefab"},
			{1800, "Player/Chara/1800Dreamcast/PlayerDreamcast.prefab"}
		};

		// CustomMonkey is the one replacing,
		// monkeyToReplace is the one being replaced.
		public static int customMonkey { get; set; } = 100;
		public static int monkeyToReplace { get; set; } = 0;

		public static void OnModLoad(Dictionary<string, object> settings)
		{
			customMonkey = System.Convert.ToInt32((float)settings["CustomMonkey"]);
			monkeyToReplace = System.Convert.ToInt32((float)settings["MonkeyToReplace"]);
			System.Console.WriteLine($"{characterKind[customMonkey]} to {characterKind[monkeyToReplace]} prepped. Waiting for GameObjects!");
		}

		public static void OnModStart()
		{
			MgCharaManager charaData = FindObjectOfType<MgCharaManager>();
			MgBananaLookObj mgBananaLookObj = FindObjectOfType<MgBananaLookManager>().m_BananaLookObj;
			if (charaData == null) return;
			try
			{
				// Fuck IL2CPP
				foreach (Il2CppSystem.Collections.Generic.KeyValuePair<int, MgCharaDatum> kvp in charaData.m_CharaDataDict)
				{
					if (kvp.Key == monkeyToReplace)
					{
						// Replace the original monkey with new values
						MgCharaDatum oldMonkey = charaData.m_CharaDataDict[kvp.Key];
						oldMonkey.m_CharaKind = characterKind[customMonkey];
						oldMonkey.m_prefabPath = prefabs[customMonkey];
						// Check the last digit of the int for skinId
						oldMonkey.m_SkinId = customMonkey % 10;
						// If they have a custom banana shape, apply it
						if (customMonkey > 802 || customMonkey < 1500)
						{
							mgBananaLookObj.m_CharaToBananaLookArray[System.Convert.ToInt32(monkeyToReplace.ToString().Substring(0, monkeyToReplace.ToString().Length - 2 ))].m_BananaLookStr = bananaReplacer[customMonkey];
						}
						System.Console.WriteLine($"Successfully replaced {characterKind[monkeyToReplace]} with {characterKind[customMonkey]}!");
					}
				}
			}
			catch
			{
				// It shouldn't catch but in case it does, let's just keep it contained, shall we?
			}
		}
	}
}
