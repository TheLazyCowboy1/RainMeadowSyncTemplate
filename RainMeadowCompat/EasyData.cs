using RainMeadow;
using System;

namespace RainMeadowCompat;

/// <summary>
/// An class that intends to make using states/ResourceData slightly easier.
/// This data will likely only be useful to add to lobbies.
/// Will not work for various types of EntityData; you'll have to do that manually.
/// </summary>
public abstract class EasyData : OnlineResource.ResourceData
{
    /// <summary>
    /// An easy way to add the data to a lobby.
    /// Only works for the host of the lobby.
    /// </summary>
    /// <param name="resource">The resource that this data should be added to, such as a lobby</param>
    public void AddToResource(OnlineResource resource)
    {
        bool isHost = OnlineManager.lobby is not null && OnlineManager.lobby.isOwner;
        if (!isHost)
            return; //only the host should date this data; I'm a client

        if (!resource.TryGetData(GetType(), out var _))
            resource.AddData(this); //only add if it doesn't already exist
    }
   
}
