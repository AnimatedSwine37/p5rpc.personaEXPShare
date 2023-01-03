using p5rpc.personaEXPShare.Template.Configuration;
using System.ComponentModel;

namespace p5rpc.personaEXPShare.Configuration
{
    public class Config : Configurable<Config>
    {
        /*
            User Properties:
                - Please put all of your configurable properties here.

            By default, configuration saves as "Config.json" in mod user config folder.    
            Need more config files/classes? See Configuration.cs

            Available Attributes:
            - Category
            - DisplayName
            - Description
            - DefaultValue

            // Technically Supported but not Useful
            - Browsable
            - Localizable

            The `DefaultValue` attribute is used as part of the `Reset` button in Reloaded-Launcher.
        */

        [DisplayName("EXP Multiplier")]
        [Description("How much exp will be given to inactive Personas.\n1.0 is the same as the active Persona (Growth 3), 0.5 is 50% of the active Persona (Growth 2), 0.25 is 25% of the active Persona (Growth 1), etc.")]
        [DefaultValue(1.0f)]
        public float ExpMultiplier { get; set; } = 1.0f;

        [DisplayName("Debug Mode")]
        [Description("Logs additional information to the console that is useful for debugging.")]
        [DefaultValue(false)]
        public bool DebugEnabled { get; set; } = false;

    }

    /// <summary>
    /// Allows you to override certain aspects of the configuration creation process (e.g. create multiple configurations).
    /// Override elements in <see cref="ConfiguratorMixinBase"/> for finer control.
    /// </summary>
    public class ConfiguratorMixin : ConfiguratorMixinBase
    {
        // 
    }
}