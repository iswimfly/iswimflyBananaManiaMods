using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Flash2;

namespace DeadLineReq
{
    public class Main
    {
        // AppInput controller to get controller inputs for P1
        private static AppInput.state_t mgState_t = null;

        public static void OnModUpdate()
        {
            // If the AppInput slot 0 is active, grab it and store it for later.
            if (AppInput.ActiveState(0) != null && mgState_t == null)
            {
                mgState_t = AppInput.ActiveState(0);
                Console.WriteLine("AppInput Found! P1 Grabbed.");
            }

            if (MainGame.Exists && MainGame.gameKind != MainGameDef.eGameKind.Practice)
            {
                // Default keybinds for quick retry.
                if (mgState_t.m_buttonMaskDown == Def.Button.Y || Input.GetKeyDown(KeyCode.X))
                {
                    // If the game is in an unpausable state, do not retry
                    if (MainGame.mainGameStage.check_pausable() == false) return;
                    MainGame.NotifyEvent(MainGame.eEvent.Retry);
                }
            }
        }

    }
}
