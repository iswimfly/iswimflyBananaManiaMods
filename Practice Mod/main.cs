using UnityEngine;
using Framework.UI;
using Flash2;
using System;

namespace PracticeMod
{

    public class Main : MonoBehaviour
    {
        private static AppInput.state_t mgState_t = null;

        public static void OnModUpdate()
        {
            // If the AppInput slot 0 is active, grab it and store it for later.
            if (AppInput.ActiveState(0) != null && mgState_t == null)
            {
                mgState_t = AppInput.ActiveState(0);
                Console.WriteLine("AppInput Found! P1 Grabbed.");
            }

            if (MainGame.Exists)
            {
                // Default keybinds for quick retry.
                if (mgState_t.m_buttonMaskDown == Def.Button.Y || Input.GetKeyDown(KeyCode.X))
                {
                    // If the game is in an unpausable state, do not retry
                    if (MainGame.mainGameStage.check_pausable() == false || MainGame.gameKind == MainGameDef.eGameKind.Practice) return;
                    MainGame.NotifyEvent(MainGame.eEvent.Retry);
                }
                if (mgState_t.m_buttonMaskDown == Def.Button.X || Input.GetKeyDown(KeyCode.LeftControl))
                {
                    if (MainGame.mainGameStage.check_pausable() == false) return;
                    FindObjectOfType<MainGame>().m_isRequestRecreateStage = true;
                }
            }
        }
    }
}

