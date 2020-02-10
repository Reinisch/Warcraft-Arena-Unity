using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Vehicle Info Container", menuName = "Game Data/Containers/Vehicle Info", order = 1)]
    internal class VehicleInfoContainer : ScriptableUniqueInfoContainer<VehicleInfo>
    {
        [SerializeField, UsedImplicitly] private List<VehicleInfo> vehicleInfos;

        protected override List<VehicleInfo> Items => vehicleInfos;

        private readonly Dictionary<int, VehicleInfo> vehicleInfoById = new Dictionary<int, VehicleInfo>();

        public IReadOnlyDictionary<int, VehicleInfo> VehicleInfoById => vehicleInfoById;

        public override void Register()
        {
            base.Register();

            vehicleInfos.ForEach(vehicle => vehicleInfoById.Add(vehicle.Id, vehicle));
        }

        public override void Unregister()
        {
            vehicleInfoById.Clear();

            base.Unregister();
        }
    }
}