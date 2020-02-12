using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Vehicle Seat Info Container", menuName = "Game Data/Containers/Vehicle Seat Info", order = 1)]
    internal class VehicleSeatInfoContainer : ScriptableUniqueInfoContainer<VehicleSeatInfo>
    {
        [SerializeField, UsedImplicitly] private List<VehicleSeatInfo> vehicleSeatInfos;

        protected override List<VehicleSeatInfo> Items => vehicleSeatInfos;

        private readonly Dictionary<int, VehicleSeatInfo> vehicleSeatInfoById = new Dictionary<int, VehicleSeatInfo>();

        public IReadOnlyDictionary<int, VehicleSeatInfo> VehicleSeatInfoById => vehicleSeatInfoById;

        public override void Register()
        {
            base.Register();

            vehicleSeatInfos.ForEach(vehicle => vehicleSeatInfoById.Add(vehicle.Id, vehicle));
        }

        public override void Unregister()
        {
            vehicleSeatInfoById.Clear();

            base.Unregister();
        }
    }
}