using MonoMod.RuntimeDetour;
using RainMeadowSyncTemplate;
using System;
using RainMeadow;
using System.Reflection;
using System.Collections.Generic;
using BepInEx.Logging;

namespace RainMeadowCompat;

/**<summary>
 * This class is designed to make setting up Rain Meadow compatibility as easy as possible.
 * All you really need to concern yourself with within this file is:
 * AddLobbyData() - here is where you'll initialize your ResourceDatas.
 * LogSomething() - for convenient logging.
 * 
 * Make sure to EITHER
 * call SafeMeadowInterface.InitializeMeadowCompatibility() on OnEnable()
 * OR call SafeMeadowInterface.ModsInitialized() when/after mods are initialized, and before the lobby is joined.
 * </summary>
 */
public class MeadowCompatSetup
{
    //the mod id of Rain Meadow
    public const string RAIN_MEADOW_BEPINEX_ID = "henpemaz.rainmeadow";
    public const string RAIN_MEADOW_MOD_ID = "henpemaz_rainmeadow";

    //whether Rain Meadow is currently enabled. Set by ModsInitialized()
    public static bool MeadowEnabled = false;

    //keeps track of whether the OnModsInit hook was added
    private static bool AddedOnModsInit = false;

    public static ManualLogSource? Logger;

    /**<summary>
     * Use SafeMeadowInterface.InitializeMeadowCompatibility() instead.
     * 
     * The easiest way to set up Meadow compatibility, since everything is managed here.
     * Should be called by OnEnable().
     * MUST be called before mods are initialized.
     * If mods are already initialized, use ModsInitialized() instead.
     * </summary>
     */
    public static void InitializeMeadowCompatibility(ManualLogSource? logger = null)
    {
        On.RainWorld.OnModsInit += RainWorld_OnModsInit;

        AddedOnModsInit = true;

        if (logger != null)
            Logger = logger;
    }

    
    private static void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);

        ModsInitialized();
    }

    /**<summary>
     * Use SafeMeadowInterface.ModsInitialized() instead.
     * 
     * Should be called when or after mods are initialized.
     * Automatically called if InitializeMeadowCompatibility() was called at OnEnable().
     * Checks if Rain Meadow is installed.
     * </summary>
     */
    public static void ModsInitialized(ManualLogSource? logger = null)
    {
        if (logger != null)
            Logger = logger;

        if (ModManager.ActiveMods.Exists(mod => mod.id == RAIN_MEADOW_MOD_ID))
        {
            MeadowEnabled = true;
            AddLobbyHook();
        }

        LogSomething("Rain Meadow enabled: " + MeadowEnabled);
    }

    private static bool LobbyHookAdded = false;
    private static void AddLobbyHook()
    {
        try
        {
            if (!LobbyHookAdded)
            {
                Lobby.OnAvailable += AddLobbyData;
                LobbyHookAdded = true;
                ExtraDebug("Added lobby available event");
            }
        }
        catch (Exception ex) { LogSomething(ex); }
    }

    /**<summary>
     * Use SafeMeadowInterface.RemoveHooks() instead.
     * 
     * Should be called by OnDisable().
     * Removes any hooks added by this file.
     * </summary>
     */
    public static void RemoveHooks()
    {
        try
        {
            if (AddedOnModsInit) On.RainWorld.OnModsInit -= RainWorld_OnModsInit;
            if (LobbyHookAdded) Lobby.OnAvailable -= AddLobbyData;
        }
        catch (Exception ex) { LogSomething(ex); }
    }

    public static List<Type> DataToAdd = new();
    /**
     * This is the place to add all your initial data.
     * This function is called as soon as a lobby is available.
     * This is the best place to add static data,
     *  such as config (Remix) options, global variables, randomizer files, etc.
     * 
     * Other data (like for items) should likely be added elsewhere.
     * ...I'm not sure where, yet.
     */
    private static void AddLobbyData(OnlineResource lobby)
    {
        if (!lobby.isOwner)
            return; //don't add data to something I don't own
        
        ExtraDebug("AddLobbyData");
        foreach (Type dataType in DataToAdd)
        {
            if (!typeof(EasyData).IsAssignableFrom(dataType))
                continue; //it's not EasyData, so discard it

            if (lobby.TryGetData(dataType, out var _))
                continue; //don't add/create data if we already have it

            EasyData data = (EasyData) Activator.CreateInstance(dataType);
            data.AddToResource(lobby);
            
            ExtraDebug("Added data");
        }
        
        //lobby.AddData<ExampleData>(new ExampleData());
    }


    /**<summary>
     * Add your preferred logging method here.
     * If you don't want any logs... just let this function be empty, I guess.
     * </summary>
     */
    public static void LogSomething(object obj)
    {
        Logger?.LogInfo(obj);
    }

    /**<summary>
     * If you want EXTRA debug info, you can use this.
     * Splitting logging into two separate methods makes it easier to
     * disable unneeded logging messages without fully deleting them.
     * </summary>
     */
    public static void ExtraDebug(object obj)
    {
        Logger?.LogDebug(obj);
    }
}
