using Modding;
using JetBrains.Annotations;
using UnityEngine;
using UObject = UnityEngine.Object;
using System.Collections.Generic;

namespace PVPoP
{
    [UsedImplicitly]
    public class PVPoP : Mod, ITogglableMod
    {
        public static Dictionary<string, GameObject> preloadedGO = new Dictionary<string, GameObject>();

        public static PVPoP Instance;

        public override string GetVersion()
        {
            return "Prepare to Die(1.5)";
        }

        public override List<(string, string)> GetPreloadNames()
        {
            return new List<(string, string)>
            {
                ("GG_Hollow_Knight","Battle Scene/HK Prime"),
                ("GG_Hollow_Knight","Battle Scene/Focus Blasts"),
            };
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Storing GOs");
            preloadedGO["pv"] = preloadedObjects["GG_Hollow_Knight"]["Battle Scene/HK Prime"];
            preloadedGO["blasts"] = preloadedObjects["GG_Hollow_Knight"]["Battle Scene/Focus Blasts"];
            Instance = this;
            Log("Initalizing.");
            Unload();
            On.HeroController.Start += AddCP;
        }

        private void AddCP(On.HeroController.orig_Start orig, HeroController self)
        {
            orig(self);
            if(GameManager.instance.gameObject.GetComponent<FindPain>()==null)
            {
                GameManager.instance.gameObject.AddComponent<FindPain>();
            }
        }

        public void Unload()
        {
            AudioListener.volume = 1f;
            AudioListener.pause = false;
            On.HeroController.Start -= AddCP;

            // ReSharper disable once Unity.NoNullPropogation
            var x = GameManager.instance?.gameObject.GetComponent<FindPain>();
            if (x == null) return;
            UObject.Destroy(x);
        }
    }
}