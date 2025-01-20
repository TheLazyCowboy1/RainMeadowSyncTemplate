using Menu.Remix.MixedUI;
using UnityEngine;

namespace RainMeadowSyncTemplate;

public class TemplateModOptions : OptionInterface
{
    public TemplateModOptions()
    {
        PlayerSpeed = this.config.Bind<float>("PlayerSpeed", 1f, new ConfigAcceptableRange<float>(0f, 100f));
    }

    //configs
    public readonly Configurable<float> PlayerSpeed;

    //tabs
    private UIelement[] UIArr;
    
    
    public override void Initialize()
    {
        var opTab = new OpTab(this, "Options");
        this.Tabs = new[]
        {
            opTab
        };

        UIArr = new UIelement[]
        {
            new OpLabel(10f, 550f, "Options", true),
            new OpLabel(10f, 520f, "Player run speed factor"),
            new OpUpdown(PlayerSpeed, new Vector2(10f,490f), 100f, 1),
            
            new OpLabel(10f, 460f, "Gotta go fast!", false){ color = new Color(0.2f, 0.5f, 0.8f) }
        };
        opTab.AddItems(UIArr);
    }

    public override void Update()
    {
        if (((OpUpdown)UIArr[2]).GetValueFloat() > 10)
        {
            ((OpLabel)UIArr[3]).Show();
        }
        else
        {
            ((OpLabel)UIArr[3]).Hide();
        }
    }

}