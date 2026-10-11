using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace MeowMeowFan
{
    /// <summary>
    /// Selective gas pump behavior. The overall approach follows Peter Han's MIT-licensed
    /// Smart Pumps implementation, adapted for a compact 1x1 fan and without custom status UI.
    /// </summary>
    public sealed class SelectiveVentilationFan : KMonoBehaviour, ISim1000ms
    {
        private static readonly Operational.Flag PumpableFlag =
            new Operational.Flag("MeowMeowFanPumpable", Operational.Flag.Type.Requirement);

        [MyCmpReq] private ElementConsumer consumer;
        [MyCmpReq] private ConduitDispenser dispenser;
        [MyCmpReq] private Operational operational;
        [MyCmpReq] private KSelectable selectable;
        [MyCmpReq] private Storage storage;
        [MyCmpReq] private Filterable filterable;

        [SerializeField]
        public int detectRadius = 1;

        private MethodInfo simRegister;
        private MethodInfo simUnregister;
        private SimHashes lastElement = SimHashes.Void;
        private Guid noElementGuid;
        private Guid conduitBlockedGuid;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            consumer.EnableConsumption(false);
            simRegister = AccessTools.Method(typeof(SimComponent), "SimRegister");
            simUnregister = AccessTools.Method(typeof(SimComponent), "SimUnregister");
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            filterable.onFilterChanged += OnFilterChanged;
            UpdateAndApplyOperationalState();
            dispenser.GetConduitManager()?.AddConduitUpdater(OnConduitUpdate, ConduitFlowPriority.LastPostUpdate);
        }

        protected override void OnCleanUp()
        {
            filterable.onFilterChanged -= OnFilterChanged;
            dispenser.GetConduitManager()?.RemoveConduitUpdater(OnConduitUpdate);
            base.OnCleanUp();
        }

        private void OnFilterChanged(Tag tag)
        {
            UpdateAndApplyOperationalState();
        }

        private void OnConduitUpdate(float dt)
        {
            conduitBlockedGuid = selectable.ToggleStatusItem(
                Db.Get().BuildingStatusItems.ConduitBlocked,
                conduitBlockedGuid,
                dispenser.blocked);
        }

        public void Sim1000ms(float dt)
        {
            UpdateAndApplyOperationalState();
        }

        private void UpdateAndApplyOperationalState()
        {
            bool hasMedia = UpdateOperationalRequirements();
            operational.SetActive(operational.IsOperational && hasMedia, false);
        }

        private bool UpdateOperationalRequirements()
        {
            SimHashes element = ElementLoader.GetElementID(filterable.SelectedTag);
            bool validElement = element != SimHashes.Void && element != SimHashes.Vacuum;

            if (validElement && element != lastElement)
            {
                consumer.elementToConsume = element;
                RecreateSimHandle();
                consumer.RefreshConsumptionRate();
                lastElement = element;
            }
            else if (!validElement)
            {
                lastElement = SimHashes.Void;
            }

            bool hasMedia = validElement && IsPumpableFixed(gameObject, Element.State.Gas, element, detectRadius);
            bool canPump = validElement && !storage.IsFull() && hasMedia;
            operational.SetFlag(PumpableFlag, canPump);

            noElementGuid = selectable.ToggleStatusItem(
                Db.Get().BuildingStatusItems.NoGasElementToPump,
                noElementGuid,
                validElement && !hasMedia);

            return canPump;
        }

        private void RecreateSimHandle()
        {
            if (simRegister == null || simUnregister == null)
                return;

            simUnregister.Invoke(consumer, null);
            simRegister.Invoke(consumer, null);
        }

        private static bool IsPumpableFixed(GameObject pump, Element.State state, SimHashes element, int radius)
        {
            radius = Math.Max(1, radius);
            int offset = Math.Max(0, radius - 1);
            int diameter = 2 * radius - 1;
            int baseCell = Grid.PosToCell(pump);

            if (offset > 0)
                baseCell = Grid.OffsetCell(baseCell, new CellOffset(-offset, -offset));

            for (int y = 0; y < diameter; y++)
            {
                for (int x = 0; x < diameter; x++)
                {
                    if (Math.Abs(y - offset) + Math.Abs(x - offset) > offset)
                        continue;

                    int cell = baseCell + x + Grid.WidthInCells * y;
                    if (!Grid.IsValidCell(cell))
                        continue;

                    Element cellElement = Grid.Element[cell];
                    if (cellElement.IsState(state) && cellElement.id == element)
                        return true;
                }
            }

            return false;
        }
    }
}
