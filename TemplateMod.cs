using System;
using System.Security;
using System.Security.Permissions;
using BepInEx;
using RainMeadowCompat;

#pragma warning disable CS0618

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace RainMeadowSyncTemplate; //rename this with ctrl + r

//dependencies:
//Rain Meadow:
[BepInDependency(MeadowCompatSetup.RAIN_MEADOW_ID, BepInDependency.DependencyFlags.SoftDependency)]

[BepInPlugin(MOD_ID, MOD_NAME, MOD_VERSION)]
public class TemplateMod : BaseUnityPlugin
{
    public const string MOD_ID = "AuthorName.ModName";
    public const string MOD_NAME = "Mod Name";
    public const string MOD_VERSION = "0.0.1";


    //mostly just used for easy logging/data access
    public static TemplateMod Instance;

    //made static for easy access. Hopefully this mod should never be initiated twice anyway...
    public static TemplateModOptions Options;

    public TemplateMod()
    {
        try
        {
            Instance = this;
            Options = new TemplateModOptions();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
            throw;
        }
    }
    private void OnEnable()
    {
        On.RainWorld.OnModsInit += RainWorldOnOnModsInit;

        //this is the easiest way to set up the Rain Meadow stuff
        MeadowCompatSetup.InitializeMeadowCompatibility();
        //alternatively, you could call
    }
    private void OnDisable()
    {
        //Remove hooks

        On.RainWorld.OnModsInit -= RainWorldOnOnModsInit;

        if (IsInit)
        {
            On.Player.ctor -= PlayerHooks.PlayerOnctor;
        }

        MeadowCompatSetup.RemoveHooks();
    }

    private bool IsInit;
    private void RainWorldOnOnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);
        try
        {
            if (IsInit) return; //prevents adding hooks twice

            //Your hooks go here
            On.Player.ctor += PlayerHooks.PlayerOnctor;
            
            MachineConnector.SetRegisteredOI(MOD_ID, Options);
            IsInit = true;

            Logger.LogDebug("Hooks added!");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
            throw;
        }
    }

    #region Helper Methods

    /**<summary>
     * This static method allows for easy logging to the LogOutput.log file.
     * I made this method so that I can easily log anything from anywhere.
     * </summary>
     */
    public static void LogSomething(object obj)
    {
        Instance.Logger.LogInfo(obj);
    }

    #endregion
}
