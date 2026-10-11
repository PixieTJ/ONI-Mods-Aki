using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace MeowMeowFan
{
    [JsonObject(MemberSerialization.OptIn)]
    [RestartRequired]
    public sealed class MeowMeowFanOptions : SingletonOptions<MeowMeowFanOptions>
    {
        [Option("Require Power", "If enabled, the fan needs electrical power to run.", "Fan")]
        [JsonProperty]
        public bool RequirePower { get; set; }

        [Option("Power Consumption (W)", "Electrical power used while the fan is active.", "Fan")]
        [Limit(0.0, 480.0)]
        [JsonProperty]
        public float PowerWatts { get; set; }

        [Option("Pumping Rate (g/s)", "Maximum gas mass pumped per second. Gas pipes themselves cap at 1000 g/s.", "Fan")]
        [Limit(25.0, 1000.0)]
        [JsonProperty]
        public float PumpRateGramsPerSecond { get; set; }

        [Option("Intake Radius", "1 checks the fan's own cell; larger values reach farther around it. Keeping this small is better for performance.", "Fan")]
        [Limit(1, 3)]
        [JsonProperty]
        public int IntakeRadius { get; set; }

        [Option("Only Show Discovered Gases", "If enabled, the selector hides gases your colony has not discovered yet. The currently selected gas remains visible.", "Selector")]
        [JsonProperty]
        public bool OnlyDiscoveredGases { get; set; }

        public MeowMeowFanOptions()
        {
            RequirePower = true;
            PowerWatts = 60f;
            PumpRateGramsPerSecond = 250f;
            IntakeRadius = 1;
            OnlyDiscoveredGases = false;
        }
    }
}
