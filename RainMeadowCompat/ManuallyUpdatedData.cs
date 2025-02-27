using System;
using RainMeadow;

namespace RainMeadowCompat;

/**<summary>
 * This class is really quite simple:
 * Make a class that extends this one and is typed with itself:
 * E.g: public class RandomizerData : RainMeadowCompat.ManuallyUpdatedData<RandomizerData>
 * 
 * In it, do 4 things:
 * Define the variables that you will want synced
 * Add a constructor that calls CreateState()
 * Make UpdateData() update the relevant variables (technically optional,but recommended)
 * Make a ManuallyUpdatedState of your own (see ManuallyUpdatedState)
 * Example:
 * private class RandomizerState : RainMeadowCompat.ManuallyUpdatedState<RandomizerData>
 * </summary>
 */
public abstract class ManuallyUpdatedData<T> : EasyData where T : ManuallyUpdatedData<T>
{

    public ManuallyUpdatedState<T> CurrentState;
    public ulong LastUpdateTime = (ulong)DateTime.Now.Ticks;

    /**<summary>
     * In your implementation, be sure to initialize CurrentState.
     * Example:
     * public RandomizerData() {
     * CurrentState = new RandomizerData(this);
     * }
     * </summary>
     */
    public ManuallyUpdatedData()
    {
        //CurrentState = CreateState();
    }

    public abstract ManuallyUpdatedState<T> CreateState();

    /**<summary>
     * MAKE SURE you call the base method at the END of your UpdateData() method.
     * So after you do all the logic to update your data, call:
     * base.UpdateData();
     * </summary>
     */
    public virtual void UpdateData()
    {
        CurrentState = CreateState();
        MeadowCompatSetup.LogSomething("Initialized a new ConfigState.");
    }


    /**<summary>
     * Although this function is only supposed to be called when there are changes made to the state,
     * I found that it would be called every tick.
     * Additionally, I like manual control.
     * Therefore, instead of returning a new state, I simply return the old one
     * and update CurrentState whenever I feel like it.
     * </summary>
     */
    public override ResourceDataState MakeState(OnlineResource resource)
    {
        return CurrentState;
    }

    /**<summary>
     * Called when a new state is created.
     * </summary>
     */
    public ulong ResetUpdateTime()
    {
        LastUpdateTime = (ulong)DateTime.Now.Ticks;
        MeadowCompatSetup.ExtraDebug("Updated LastUpdateTime: " + LastUpdateTime);
        return LastUpdateTime;
    }

}
