using HarmonyLib;
using StacklandsUsabilityMod.AnimalPenHolds5Animals;
using StacklandsUsabilityMod.PacksPriceIncreasesTemporarily;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Doorstop
{
    class Entrypoint
    {
        public static void Start()
        {
            SceneManager.sceneLoaded += SceneLoaded;
            new Thread(() =>
            {
                Harmony.DEBUG = true;
                var harmony = new Harmony("net.alienav.StacklandsUsabilityMod");
                harmony.PatchAll();
            }).Start();
        }

        private static void SceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)
        {
            ReplaceBuyBoosterBoxComponentWithExtended();
        }

        public static void ReplaceBuyBoosterBoxComponentWithExtended()
        {
            var oldComponents = GameObject.FindObjectsOfType<BuyBoosterBox>();
            FileLog.Debug("Replacing BuyBoosterBoxes:");
            foreach (var oldComponent in oldComponents)
            {
                FileLog.Debug("\t" + oldComponent.ToString());
                var newComponent = oldComponent.gameObject.AddComponent<BuyBoosterBoxWithPriceIncrease>();
                Utils.CopyPublicFieldsAndProperties(oldComponent, newComponent);
                Object.Destroy(oldComponent);
            }
        }
    }
}