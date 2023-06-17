using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flash2;
using UnityEngine;

namespace TrueBallCustomizer
{
    public class Main : MonoBehaviour
	{
        // List to convert i loops to settings entries
        private static readonly Dictionary<int, string> convertFromSettings = new Dictionary<int, string>()
        {
            {0, "Sonic"},
            {1, "Tails"},
            {2, "Other"}
        };
        // List to convert i loops to charaKinds
        private static readonly Dictionary<string, Chara.eKind> charaKinds = new Dictionary<string, Chara.eKind>()
		{
			{"Sonic", Chara.eKind.Sonic},
			{"Tails", Chara.eKind.Tails},
			{"Other", Chara.eKind.Dlc02}
		};
        public static List<int> balls = new List<int>();

		public static CharaCustomizeManager manager;
        public static Chara.eKind charaKind { get; set; } = Chara.eKind.Aiai;
        public static int ballNumber { get; set; } = 1;

        public static void OnModLoad(Dictionary<string, object> settings)
        {
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine((string)settings[convertFromSettings[i]]);
                balls.Insert(i, Convert.ToInt32((string)settings[convertFromSettings[i]]));
            }
        }

        public static void OnModStart()
        {
            manager = FindObjectOfType<CharaCustomizeManager>();
            for (int i = 0; i < balls.Count; i++)
            {
                // Store information
                charaKind = charaKinds[(convertFromSettings[i])];
                ballNumber = balls[i];

                // Find the type that stores a character's ball number
                CharaCustomize.PartsSet partSet = manager.getPartsSet(charaKind, 0);
                CharaCustomize.PartsKey ballKey = partSet.m_PartsKeyDict[CharaCustomize.eAssignPos.Ball];

                // Change the number
                ballKey.m_Number = ballNumber;
            } 
			Console.WriteLine("Ball changes applied!");
        }
    }
}
