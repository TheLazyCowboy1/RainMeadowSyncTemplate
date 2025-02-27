using RainMeadow;

namespace RainMeadowCompat;

/**<summary>
 * Imagine that this State is what gets sent across the internet.
 * More specifically, any fields marked [OnlineField] will be sent.
 * 
 * This class, then, defines 2 things:
 * 1. What data is synced (fields)
 * 3. How this data is processed when received (ReadData())
 * </summary>
 */
public abstract class ManuallyUpdatedState<T> : EasyState<T> where T : ManuallyUpdatedData<T>
{
    //Tracks when the state was last updated
    [OnlineField]
    ulong LastUpdateTime;

    public ManuallyUpdatedState() : base() { }
    /**<summary>
     * You should have a constructor like:
     * public RandomizerState(RandomizerData data) : base(data) { }
     * </summary>
     */
    public ManuallyUpdatedState(T data) {
        if (data != null)
            LastUpdateTime = data.ResetUpdateTime();
    }

    /**<summary>
     * Do NOT override this function in your own implementation.
     * Instead, use ReceivedUpdate().
     * </summary>
     */
    public override void ReadTo(OnlineResource.ResourceData data, OnlineResource resource)
    {
        if (data is not T Tdata)
            return;

        if (Tdata.LastUpdateTime == LastUpdateTime)
            return; //if there's been no change, then there's nothing to change!
        Tdata.LastUpdateTime = LastUpdateTime;

        bool isHost = OnlineManager.lobby is not null && OnlineManager.lobby.isOwner;
        if (isHost)
            return; //only clients should receive this; I'm the host

        ReadData(Tdata, resource);
    }

}
