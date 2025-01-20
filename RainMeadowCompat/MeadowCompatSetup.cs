using MonoMod.RuntimeDetour;
using RainMeadowSyncTemplate;
using System;
using RainMeadow;
using System.Reflection;

namespace RainMeadowCompat
{
    public class MeadowCompatSetup
    {
        //the mod id of Rain Meadow
        public const string RAIN_MEADOW_ID = "henpemaz_rainmeadow";

        //whether Rain Meadow is currently enabled. Set by ModsInitialized()
        public static bool MeadowEnabled = false;

        //keeps track of whether the OnModsInit hook was added
        private static bool AddedOnModsInit = false;

        /**<summary>
         * The easiest way to set up Meadow compatibility, since everything is managed here.
         * Should be called by OnEnable().
         * MUST be called before mods are initialized.
         * If mods are already initialized, use ModsInitialized() instead.
         * </summary>
         */
        public static void InitializeMeadowCompatibility()
        {
            On.RainWorld.OnModsInit += RainWorld_OnModsInit;

            AddedOnModsInit = true;
        }

        
        private static void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);

            ModsInitialized();
        }

        /**<summary>
         * Should be called when or after mods are initialized.
         * Automatically called if InitializeMeadowCompatibility() was called at OnEnable().
         * Checks if Rain Meadow is installed.
         * </summary>
         */
        public static void ModsInitialized()
        {
            foreach (ModManager.Mod mod in ModManager.ActiveMods)
            {
                if (mod.id == RAIN_MEADOW_ID)
                {
                    MeadowEnabled = true;
                    AddLobbyHook();
                    break;
                }
            }
        }

        private static Hook LobbyHook = null;
        private static void AddLobbyHook()
        {
            try
            {
                LobbyHook = new Hook(
                    typeof(OnlineResource).GetMethod("Available", BindingFlags.Instance | BindingFlags.NonPublic),
                    typeof(MeadowCompatSetup).GetMethod(nameof(OnLobbyAvailable), BindingFlags.Static | BindingFlags.NonPublic)
                );
            }
            catch (Exception ex) { TemplateMod.LogSomething(ex); }
        }

        /**<summary>
         * Should be called by OnDisable().
         * Removes any hooks added by this file.
         * </summary>
         */
        public static void RemoveHooks()
        {
            try
            {
                if (AddedOnModsInit) On.RainWorld.OnModsInit -= RainWorld_OnModsInit;

                //destroy the OnLobbyAvailable hook, if it exists
                if (LobbyHook != null) LobbyHook.Dispose();
            }
            catch (Exception ex) { TemplateMod.LogSomething(ex); }
        }


        private delegate void orig_OnLobbyAvailable(OnlineResource self);
        private static void OnLobbyAvailable(orig_OnLobbyAvailable orig, OnlineResource self)
        {
            orig(self);


        }
    }
}
