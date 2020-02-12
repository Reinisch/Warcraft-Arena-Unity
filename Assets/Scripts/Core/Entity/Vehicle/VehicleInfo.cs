using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Vehicle Info", menuName = "Game Data/Entities/Vehicle Info", order = 1)]
    public sealed class VehicleInfo : ScriptableUniqueInfo<VehicleInfo>
    {
        [SerializeField, UsedImplicitly] private VehicleInfoContainer container;
        [SerializeField, UsedImplicitly] private List<VehicleSeatInfo> vehicleSeats;

        protected override VehicleInfo Data => this;
        protected override ScriptableUniqueInfoContainer<VehicleInfo> Container => container;

        public new int Id => base.Id;
        public IReadOnlyList<VehicleSeatInfo> Seats => vehicleSeats;
    }
}
