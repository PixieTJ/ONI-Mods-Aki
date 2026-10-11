using TUNING;
using UnityEngine;

namespace MeowMeowFan
{
    public sealed class SelectiveVentilationFanConfig : IBuildingConfig
    {
        public const string ID = "MeowMeowSelectiveGasFan";
        public const string ANIM = "mmpaifengshan_kanim";

        public override BuildingDef CreateBuildingDef()
        {
            return CreateBuildingDefFor(ID, false);
        }

        internal static BuildingDef CreateBuildingDefFor(string id, bool deprecated)
        {
            var options = MeowMeowFanOptions.Instance;
            var def = BuildingTemplates.CreateBuildingDef(
                id,
                1,
                1,
                ANIM,
                30,
                10f,
                BUILDINGS.CONSTRUCTION_MASS_KG.TIER1,
                MATERIALS.ALL_METALS,
                1600f,
                BuildLocationRule.Anywhere,
                decor: BUILDINGS.DECOR.NONE,
                noise: NOISE_POLLUTION.NOISY.TIER1);

            def.RequiresPowerInput = options.RequirePower;
            def.EnergyConsumptionWhenActive = options.RequirePower ? Mathf.Max(0f, options.PowerWatts) : 0f;
            def.ExhaustKilowattsWhenActive = 0f;
            def.SelfHeatKilowattsWhenActive = 0f;
            def.OutputConduitType = ConduitType.Gas;
            def.UtilityOutputOffset = new CellOffset(0, 0);
            def.PowerInputOffset = new CellOffset(0, 0);
            def.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
            def.Floodable = true;
            def.ViewMode = OverlayModes.GasConduits.ID;
            def.AudioCategory = "Metal";
            def.SceneLayer = Grid.SceneLayer.BuildingBack;
            def.ForegroundLayer = Grid.SceneLayer.BuildingFront;
            def.Deprecated = deprecated;
            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, id);
            return def;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefabTag)
        {
            base.ConfigureBuildingTemplate(go, prefabTag);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            ConfigureFan(go);
        }

        internal static void ConfigureFan(GameObject go)
        {
            var options = MeowMeowFanOptions.Instance;
            int radius = Mathf.Clamp(options.IntakeRadius, 1, 3);
            float rateKgPerSecond = Mathf.Clamp(options.PumpRateGramsPerSecond / 1000f, 0.025f, 1f);

            go.AddOrGet<LogicOperationalController>();
            go.AddOrGet<LoopingSounds>();
            if (options.RequirePower)
                go.AddOrGet<EnergyConsumer>();

            var filterable = go.AddOrGet<Filterable>();
            filterable.filterElementState = Filterable.ElementState.Gas;

            var storage = go.AddOrGet<Storage>();
            storage.capacityKg = 1f;

            var consumer = go.AddOrGet<ElementConsumer>();
            consumer.configuration = ElementConsumer.Configuration.Element;
            consumer.elementToConsume = SimHashes.Vacuum;
            consumer.consumptionRate = rateKgPerSecond;
            consumer.storeOnConsume = true;
            consumer.showInStatusPanel = false;
            consumer.consumptionRadius = (byte)radius;
            consumer.capacityKG = 1f;
            consumer.EnableConsumption(false);

            var dispenser = go.AddOrGet<ConduitDispenser>();
            dispenser.conduitType = ConduitType.Gas;
            dispenser.alwaysDispense = true;
            dispenser.elementFilter = null;

            go.AddOrGetDef<OperationalController.Def>();
            var fan = go.AddOrGet<SelectiveVentilationFan>();
            fan.detectRadius = radius;

            go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits, false);
        }
    }
}
