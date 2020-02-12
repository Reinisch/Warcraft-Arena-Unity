using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Vehicle Seat Info", menuName = "Game Data/Entities/Vehicle Seat Info", order = 1)]
    public sealed class VehicleSeatInfo : ScriptableUniqueInfo<VehicleSeatInfo>
    {
        [SerializeField, UsedImplicitly] 
        private VehicleSeatInfoContainer container;
        [SerializeField, UsedImplicitly, EnumFlag] 
        private VehicleSeatFlags flags;

        protected override VehicleSeatInfo Data => this;
        protected override ScriptableUniqueInfoContainer<VehicleSeatInfo> Container => container;

        public new int Id => base.Id;
        public VehicleSeatFlags Flags => flags;
    }
}