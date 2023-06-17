using System;
using System.Collections.Generic;
using Flash2;
using UnhollowerRuntimeLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Guest
{
    // Honestly I don't know C# well enough, this just works and I'm not touching it
    public static class Main
    {
        private static GuestCharacters _dynamicRoll;
        private static CameraController _cameraController;
        private static Player player;
        private static MainGameStage mainGameStage;
        private static bool soundDestroyed = false;

        public static List<Type> OnModLoad(Dictionary<string, object> settings)
        {
            return new List<Type> { typeof(GuestCharacters) };
        }

        public static void OnModUpdate()
        {
            if (!MainGame.Exists && soundDestroyed == false)
            {
                GuestCharacters[] controllers = Object.FindObjectsOfType<GuestCharacters>();
                foreach (GuestCharacters controller in controllers)
                {
                    Object.Destroy(controller);
                }
                _dynamicRoll = null;
                soundDestroyed = true;
            }
            if (MainGame.Exists && soundDestroyed == false && player == null)
            {
                GuestCharacters[] controllers = Object.FindObjectsOfType<GuestCharacters>();
                foreach (GuestCharacters controller in controllers)
                {
                    Object.Destroy(controller);
                }
                _dynamicRoll = null;
                soundDestroyed = true;
            }
            if (Object.FindObjectOfType<MainGameStage>() == null) return;

            mainGameStage = Object.FindObjectOfType<MainGameStage>();
            player = mainGameStage.GetPlayer();

            if (player != null)
            {
                if (_dynamicRoll == null)
                {
                    _cameraController = player.GetCameraController();
                    _dynamicRoll = new GuestCharacters(_cameraController.gameObject.AddComponent(Il2CppType.Of<GuestCharacters>()).Pointer);
                    soundDestroyed = false;
                }

            }
        }
    }
}