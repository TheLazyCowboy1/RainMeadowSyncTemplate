# RainMeadowSync Template

After many hours of struggling to figure out how to make my mods compatible with Rain Meadow, I wanted to make the process easier for myself and others.
I do not claim to have much skill or knowledge regarding Rain Meadow mods; but I think that is in my favor: You can trust that this template is fairly idiot-proof because I'm able to use it efficiently.

## How do I make my mod compatible with Rain Meadow?

The first question is: **What information needs to be synced?**\
Short answer: Anything that can differ from player-to-player in a way that affects gameplay.\
Examples of data that should be synced:\
* A slugcat/creature shooting a giant laser across the screen. (This affect should be visible to all players.)
* Room Randomizer data indicating where each room should connect (or at least a seed to generate the same data).
* Custom items/creatures.

You might be quite surprised by just how much information doesn't need to be synced.
Try to find shortcuts; although I can't systematically tell you how to do that.

## How do I sync this information?

There are two methods of syncing information: **RPCs**, and **States**.\
One dev's rule of thumb is: "if a new player joins will they be desynced?"\
**General overview:**
* RPCs are more useful as a one-time signal for an event or change.
* States store information (for as long as is needed) and share it with other players, and update only the data that is altered.

In my personal opinion, RPCs are much easier to implement and understand. However, States are likely more useful and should be generally preferred for smooth syncing.

# RPCs:
Developer Description: An RPC
* is sent in full every tick until acknowledged. The client \*will\* receive it.
* can be sent directly to a client (but be mindful of the data-path that States take if your logic depends on other state)
* doesn't have any compression fancies

In short: RPCs are an easy, guarenteed way to send a block of information once and quickly.
However, they aren't very efficient at reducing bandwidth or dealing with change.
They simply send _all_ of their data continually until the client receives it and acknowledges it.
