using UnityEngine;

namespace MeowMeowFan
{
    internal abstract class LegacyFanConfigBase : IBuildingConfig
    {
        protected abstract string LegacyId { get; }
        protected abstract SimHashes DefaultGas { get; }

        public override BuildingDef CreateBuildingDef()
        {
            return SelectiveVentilationFanConfig.CreateBuildingDefFor(LegacyId, true);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            SelectiveVentilationFanConfig.ConfigureFan(go);
            var migration = go.AddOrGet<LegacyFanDefaultGas>();
            migration.defaultGas = DefaultGas;
        }
    }

    internal sealed class LegacyOxygenFanConfig : LegacyFanConfigBase
    {
        protected override string LegacyId => "O2GasPump";
        protected override SimHashes DefaultGas => SimHashes.Oxygen;
    }

    internal sealed class LegacyPollutedOxygenFanConfig : LegacyFanConfigBase
    {
        protected override string LegacyId => "O22GasPump";
        protected override SimHashes DefaultGas => SimHashes.ContaminatedOxygen;
    }

    internal sealed class LegacyCarbonDioxideFanConfig : LegacyFanConfigBase
    {
        protected override string LegacyId => "CO2GasPump";
        protected override SimHashes DefaultGas => SimHashes.CarbonDioxide;
    }

    internal sealed class LegacyChlorineFanConfig : LegacyFanConfigBase
    {
        protected override string LegacyId => "Cl2GasPump";
        protected override SimHashes DefaultGas => SimHashes.ChlorineGas;
    }

    internal sealed class LegacyHydrogenFanConfig : LegacyFanConfigBase
    {
        protected override string LegacyId => "H2GasPump";
        protected override SimHashes DefaultGas => SimHashes.Hydrogen;
    }

    internal sealed class LegacyNaturalGasFanConfig : LegacyFanConfigBase
    {
        protected override string LegacyId => "CH4GasPump";
        protected override SimHashes DefaultGas => SimHashes.Methane;
    }

    /// <summary>
    /// Gives an old fixed-gas fan its historical gas selection the first time it is loaded
    /// by Meow Meow Fan. Once migrated, the normal Filterable component saves any selection
    /// the player makes, so the old building can be retargeted just like a new fan.
    /// </summary>
    internal sealed class LegacyFanDefaultGas : KMonoBehaviour
    {
        [MyCmpReq]
        private Filterable filterable;

        [SerializeField]
        public SimHashes defaultGas = SimHashes.Void;

        public override void OnSpawn()
        {
            base.OnSpawn();

            if (defaultGas == SimHashes.Void)
                return;

            Tag selected = filterable.SelectedTag;
            if (!selected.IsValid || selected == GameTags.Void)
            {
                Element element = ElementLoader.FindElementByHash(defaultGas);
                if (element != null)
                    filterable.SelectedTag = element.tag;
            }
        }
    }
}
