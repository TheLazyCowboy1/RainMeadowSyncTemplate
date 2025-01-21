using Menu.Remix.MixedUI;
using UnityEngine;

namespace RainMeadowSyncTemplate;

public class TemplateModOptions : OptionInterface
{
    public TemplateModOptions()
    {
        PlayerSpeed = this.config.Bind<float>("PlayerSpeed", 1f, new ConfigAcceptableRange<float>(0f, 100f));

        RainMeadowCompat.EasyConfigSync.RegisterConfigs(PlayerSpeed);
    }

    //configs
    public readonly Configurable<float> PlayerSpeed;


    OpUpdown playerSpeedConfig;
    OpLabel speedShameLabel;

    public override void Initialize()
    {
        var opTab = new OpTab(this, "Options");
        this.Tabs = new[]
        {
            opTab
        };

        const float l = 10f, //left margin
            w = 100f, //config width
            t = l+l+w, //text start
            s = 30; //vertical spacing (I find a vertical spacing of 30 to be pleasant for configs; 25 is probably the minimum
        float y = 550f; //current height

        opTab.AddItems(
            new OpLabel(l, y, "Options", true),
            new OpLabel(t, y-=s+s, "Other option here"), //s+s == double spacing. You could also do s * 2f, s * 1.5f, etc.
            //new OpUpdown(BogusConfig, new Vector2(l, y), w, 1),
            new OpLabel(t, y-=s, "Player run speed factor")
        );

        playerSpeedConfig = new OpUpdown(PlayerSpeed, new Vector2(l, y), w, 1);
        speedShameLabel = new OpLabel(l, y -= s + s, "Gotta go fast!", false) { color = new Color(0.2f, 0.5f, 0.8f) };

        opTab.AddItems(
            playerSpeedConfig,
            speedShameLabel
        );
    }

    /**<summary>
     * Calls every frame, approximately.
     * </summary>
     */
    public override void Update()
    {
        if (playerSpeedConfig?.GetValueFloat() > 10)
        {
            speedShameLabel?.Show();
        }
        else
        {
            speedShameLabel?.Hide();
        }
    }

}