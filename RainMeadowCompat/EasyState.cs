using RainMeadow;
using System;

namespace RainMeadowCompat;

/// <summary>
/// Makes setting up states just a tad bit easier by:
/// 1. Making the DataType be stored without the potentially-confusing GetDataType() method,
/// 2. Offers a ReadData() method that is slightly more convenient to use.
/// </summary>
/// <typeparam name="T">The EasyData associated with this state.</typeparam>
public abstract class EasyState<T> : OnlineResource.ResourceData.ResourceDataState where T : EasyData
{

    public override Type GetDataType() => typeof(T);

    public EasyState() : base() { }

    public override void ReadTo(OnlineResource.ResourceData data, OnlineResource resource)
    {
        if (data is not T Tdata)
            return;
        
        bool isHost = OnlineManager.lobby is not null && OnlineManager.lobby.isOwner;
        if (isHost)
            return; //only clients should receive this; I'm the host

        ReadData(Tdata, resource);
    }

    /// <summary>
    /// Called ONLY for clients.
    /// Called EVERY tick.
    /// This is function is called when you receive data from the internet and are reading it to your device.
    /// </summary>
    /// <param name="data">The ResourceData associated with this state.</param>
    /// <param name="resource">The OnlineResource (probably a lobby or entity) that the data belongs to.</param>
    public abstract void ReadData(T data, OnlineResource resource);
}
