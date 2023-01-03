using p5rpc.personaEXPShare.Configuration;
using p5rpc.personaEXPShare.Template;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;
using Reloaded.Memory.Sources;
using Reloaded.Mod.Interfaces;
using IReloadedHooks = Reloaded.Hooks.ReloadedII.Interfaces.IReloadedHooks;

namespace p5rpc.personaEXPShare
{
    /// <summary>
    /// Your mod logic goes here.
    /// </summary>
    public class Mod : ModBase // <= Do not Remove.
    {
        /// <summary>
        /// Provides access to the mod loader API.
        /// </summary>
        private readonly IModLoader _modLoader;

        /// <summary>
        /// Provides access to the Reloaded.Hooks API.
        /// </summary>
        /// <remarks>This is null if you remove dependency on Reloaded.SharedLib.Hooks in your mod.</remarks>
        private readonly IReloadedHooks? _hooks;

        /// <summary>
        /// Provides access to the Reloaded logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Entry point into the mod, instance that created this class.
        /// </summary>
        private readonly IMod _owner;

        /// <summary>
        /// Provides access to this mod's configuration.
        /// </summary>
        private Config _configuration;

        /// <summary>
        /// The configuration of the currently executing mod.
        /// </summary>
        private readonly IModConfig _modConfig;

        private IMemory _memory;

        private nuint _expMultiplier;

        private IAsmHook _expGainHook;

        public Mod(ModContext context)
        {
            _modLoader = context.ModLoader;
            _hooks = context.Hooks;
            _logger = context.Logger;
            _owner = context.Owner;
            _configuration = context.Configuration;
            _modConfig = context.ModConfig;

            Utils.Initialise(_logger, _configuration);

            var startupScannerController = _modLoader.GetController<IStartupScanner>();
            if (startupScannerController == null || !startupScannerController.TryGetTarget(out var startupScanner))
            {
                Utils.LogError($"Unable to get controller for Reloaded SigScan Library, aborting initialisation");
                return;
            }

            _memory = new Memory();
            _expMultiplier = _memory.Allocate(4);
            _memory.Write(_expMultiplier, _configuration.ExpMultiplier);
            
            startupScanner.AddMainModuleScan("48 8D 56 ?? 41 B8 08 00 00 00", result =>
            {
                if(!result.Found)
                {
                    Utils.LogError($"Unable to find address for Persona exp gain, aborting initialisation :(");
                    return;
                }
                Utils.LogDebug($"Found Persone exp gain at 0x{Utils.BaseAddress + result.Offset:X}");

                string[] function =
                {
                    "use64",
                    $"mulss xmm0, [qword {_expMultiplier}]",
                    "cvttss2si ebx, xmm0" // Put the exp gained if active (multiplied by our multiplier) into total exp gained instead of 0
                };
                _expGainHook = _hooks.CreateAsmHook(function, result.Offset + Utils.BaseAddress, AsmHookBehaviour.ExecuteFirst).Activate();
            });
        }

        #region Standard Overrides
        public override void ConfigurationUpdated(Config configuration)
        {
            // Apply settings from configuration.
            // ... your code here.
            if (_configuration.ExpMultiplier != configuration.ExpMultiplier)
            {
                _memory.Write(_expMultiplier, configuration.ExpMultiplier);
                Utils.Log($"Updated exp multiplier to {configuration.ExpMultiplier}");
            }
            _configuration = configuration;
            Utils.UpdateConfig(_configuration);
            _logger.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");
        }
        #endregion

        #region For Exports, Serialization etc.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Mod() { }
#pragma warning restore CS8618
        #endregion
    }
}