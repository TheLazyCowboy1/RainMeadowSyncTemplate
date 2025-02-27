using RainMeadow;
using static RainMeadowCompat.EasyConfigSync;

namespace RainMeadowCompat;
 
public class ConfigData : ManuallyUpdatedData<ConfigData>
{
    public bool[] Bools;
    public int[] Ints;
    public float[] Floats;
    public string[] Strings;

    public ConfigData()
    {
        UpdateData(); //hopefully not very necessary, but I added this line just in case
    }

    public override void UpdateData()
    {
        Bools = new bool[BoolConfigs.Count];
        for (int i = 0; i < Bools.Length; i++)
            Bools[i] = BoolConfigs[i].Value;

        Ints = new int[IntConfigs.Count];
        for (int i = 0; i < Ints.Length; i++)
            Ints[i] = IntConfigs[i].Value;

        Floats = new float[FloatConfigs.Count];
        for (int i = 0; i < Floats.Length; i++)
            Floats[i] = FloatConfigs[i].Value;

        Strings = new string[StringConfigs.Count];
        for (int i = 0; i < Strings.Length; i++)
            Strings[i] = StringConfigs[i].Value;

        base.UpdateData();
    }

    public override ManuallyUpdatedState<ConfigData> CreateState()
    {
        return new ConfigState(this);
    }

    private class ConfigState : ManuallyUpdatedState<ConfigData>
    {
        [OnlineField]
        bool[] Bools;
        [OnlineField]
        int[] Ints;
        [OnlineField]
        float[] Floats;
        [OnlineField]
        string[] Strings;

        public ConfigState() : base() { }
        public ConfigState(ConfigData data) : base(data)
        {
            Bools = data.Bools;
            Ints = data.Ints;
            Floats = data.Floats;
            Strings = data.Strings;
        }

        public override void ReadData(ConfigData data, OnlineResource resource)
        {
            //update options
            for (int i = 0; i < Bools.Length; i++)
                BoolConfigs[i].Value = Bools[i];

            for (int i = 0; i < Ints.Length; i++)
                IntConfigs[i].Value = Ints[i];

            for (int i = 0; i < Floats.Length; i++)
                FloatConfigs[i].Value = Floats[i];

            for (int i = 0; i < Strings.Length; i++)
                StringConfigs[i].Value = Strings[i];

            MeadowCompatSetup.LogSomething("Updated config values.");
        }
    }
}

    
