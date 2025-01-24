# RainMeadowSync Template

After many hours of struggling to figure out how to make my mods compatible with Rain Meadow, I wanted to make the process easier for myself and others.
I do not claim to have much skill or knowledge regarding Rain Meadow mods; but I think that is in my favor: You can trust that this template is fairly idiot-proof because I'm able to use it efficiently.

## How do I make my mod compatible with Rain Meadow?

The first question is: **What information needs to be synced?**\
Short answer: Anything that can differ from player-to-player in a way that affects gameplay.\
Examples of data that should be synced:
* A slugcat/creature shooting a giant laser across the screen. (This affect should be visible to all players.)
* Room Randomizer data indicating where each room should connect (or at least a seed to generate the same data).
* Custom items/creatures.

You might be quite surprised by just how much information doesn't need to be synced.
Test your mod in Rain Meadow to see how well it works as-is, then determine what should be synced.
Try to find ways to find shortcuts to sync as little as possible; although I can't systematically tell you how to do that.

## How do I sync this information?

There are two methods of syncing information: **RPCs**, and **States**.\
One dev's rule of thumb is: "if a new player joins will they be desynced?"\
**General overview:**
* RPCs are more useful as a one-time signal for an event or change.
* States store information (for as long as is needed) and share it with other players, and update only the data that is altered.

In my personal opinion, RPCs are much easier to implement and understand. However, States are likely more useful and should be generally preferred for smooth syncing.

### RPCs:
**Developer Description:** An RPC
* is sent in full **every tick** until acknowledged. The client \*will\* receive it.
* can be sent **directly** to a client (but be mindful of the data-path that States take if your logic depends on other state)
* doesn't have any compression fancies

**In short:** RPCs are an easy, guarenteed way to send a block of information once and quickly.
However, they aren't very efficient at reducing bandwidth or dealing with change.
They simply send _all_ of their data continually until the client receives it and acknowledges it.

Think of a RPC as a **single function call**.
An RPC defines which function to call, and what the parameters for that function are.
For example, hitting a creature with a rock might send an RPC indicating that Creature.Stun() should be called for that creature.\
In fact, RPCs are effectively and somewhat literally just function calls.

### States:
**Developer Description:** A State
* is sent to all clients that are subscribed to the resource/entity that has customdata,
* always goes from entity-owner to resource-owner ("feed"), then resource-owner to subscribers ("subscription").
* because state stops sending once the resource/entity ceases existing (or client unsubscribes) some state-changes near end-of-life might never been seen (or even long-living ones given enough lag)
* once the client has acknowledged state at least once, delta-encoding kicks in, state that doesn't change doesn't take any bandwidth (does take a bit of processing and allocation tho)
* the delta encoding system ain't the brightest and quite a few things can break it

View a State as a **collection** of variables.
This State is synced among everyone who is **subscribed** to it.
If one of the variables change, the State signals everyone subscribed to it that the variable has been changed.
Thus, all the data is kept in-sync without you having to worry about when to signal that there's been a change.
Additionally, States are very bandwidth-efficient because they only send online information about changes made.

In short: <ins>States are more complicated but more efficient.</ins>

# Setting up Rain Meadow compatibility:

Hopefully you have some mental concept of what needs to be synced and what method (RPCs or States) will work best for it.
Now it's time to describe how to set up the code.

## Establishing the framework:

1. **Simply copy the RainMeadowCompat folder in this repository into your mod's source code.**
2. Find the current verison of the **RainMeadow.dll** file and add it as an assembly reference in your project.
3. Add Rain Meadow as a soft dependency in your mod (see TemplateMod.cs as an example).
4. Call SafeMeadowInterface.InitializeMeadowCompatibility() in the OnEnable() function in your mod.
   - Alternatively, you can call SafeMeadowInterface.ModsInitialized() once mods are initialized.
   - It's also good to call SafeMeadowInterface.RemoveHooks() in your OnDisable() function.

InitializeMeadowCompatibility() will automatically set up initialize code that will detect whether Rain Meadow is enabled, and add hooks accordingly.

## Using RPCs:

### EasyRPC Class:

I highly recommend using my EasyRPC class to set up RPCs. It hopefully makes it easier to control who can send and receive the RPC.\
EasyRPCs will be sent to the players you specify (the Host, the Clients, or everyone).

1. Create a file like MyModRPCs.cs that will contain your **public static** RPCs.
2. For each RPC you want to make, add two things to your MyModRPCs.cs file:\
   i. A [RPCMethod] public static method that takes a RPCEvent as its first parameter.
     ```
     [RPCMethod]
     public static void ExampleRPCMethod(RPCEvent rpcEvent, int myFavoriteNumber, string childhoodNickname) {

     }
     ```
   ii. A public static EasyRPC instance that uses the function you just created and defines who can send/receive the RPC.
     ```
     public static EasyRPC MyExampleRPC = new(ExampleRPCMethod, Recipient.Host, Recipient.Clients);
     ```
3. Add a function in MeadowInterface.cs by which you will invoke the RPC. Simply call MyExampleRPC.Invoke(), passing in all the parameters (except the RPCEvent).
   ```
   public static void InvokeMyExampleRPC(int myFavoriteNumber, string childhoodNickname) {
     MyModRpcs.MyExampleRPC.Invoke(false, myFavoriteNumber, childhoodNickname);
   }
   ```
4. Add a function in SafeMeadowInterface.cs that will call the function you made in MeadowInferface.cs IF Rain Meadow is enabled.
   ```
   public static void InvokeMyExampleRPC(int myFavoriteNumber, string childhoodNickname) {
   if (!MeadowCompatSetup.MeadowEnabled) return;
   try { MeadowInterface.InvokeMyExampleRPC(myFavoriteNumber, childhoodNickname) }
   catch { return; }
   ```
   Make sure you remember to add a try/catch!
5. Call your function in SafeMeadowInferface whenever you want to invoke your RPC.

### Custom RPCs:

Alternatively, you could just invoke RPCs manually.\
RPCs really are quite simple. You'll likely want to go through the list of OnlinePlayers in OnlineManager.Players and invoke RPCs for whoever you want.\
But the EasyRPC class is designed to be as idiot-proof as possible. Even I can use it!

## Using States:

### Syncing config data:

If you want any remix/config data for your mod to be synced, you can simply add the configs you want synced with the EasyConfigSync.RegisterConfigs() function.\
But even if you don't want to sync configs, the ConfigData.cs class should serve as a good example for how to use the ManuallyUpdatedData and ManuallyUpdatedState classes.

### ManuallyUpdatedState:

ManuallyUpdatedStates may have a niche use, but I found them very useful for myself. They are useful for:
* Syncing global and/or static data.
* Syncing information that changes infrequently.
* Syncing literal files.
* Host authoritative information.

Each ManuallyUpdatedState class must be linked to a related ManuallyUpdatedData class that contains it.\
I'm too lazy to describe to you how to set it up. Examine the comments in ManuallyUpdatedData.cs, ManuallyUpdatedState.cs, and ConfigData.cs.

ManuallyUpdatedData will be updated very similarly to how RPCs are invoked.

I made the ManuallyUpdatedData class because I was struggling with States flip-flopping information:
The Host would send some data, the Client would send different data, the Host and the Client would both update to the other's data (thus there was a change), so they would send that data, etc.
Since I only required the OnlineData to be updated once or twice, I therefore implemented measures to make it "Manually Updated."

### ResourceState:

...does anyone else want to put some descriptions here? There's a LOT that needs explaining; but frankly I just don't know how to use States properly.

### OnlineObjectState:

Oftentimes, the data you need synced won't be static. For example, it might be data tied to an object (like an item or a creature).

Unfortunately, I haven't yet implemented any easier methods of tying data to an object. ...I'll... I'll do it eventually...

## Testing your mod:

The optimal way to test your mod would be to use some testing builds.
If you want to get these testing builds or contribute to Rain Meadow in any way, follow the steps in here: [Rain Meadow Contributing.md](https://github.com/henpemaz/Rain-Meadow/blob/main/CONTRIBUTING.md)

Build with one of the following configurations:
* StoryP2P for testing Story mode.
* LocalP2P for testing Meadow mode.
* ArenaP2P for testing Arena mode.

With one of these builds copied into your mod folder, you should be able to open Rain World, create a lobby, open Rain World again, and join the lobby you made.
Thus, you'll have two (or more) players running Rain World on your computer.
This way, you won't have to send your mod to other computers just to test it.

_Alternatively..._

You could beg someone (like me) who has these test builds.
That'll probably save you a few hours of strugging with GitHub repos and dependencies and what-not.
