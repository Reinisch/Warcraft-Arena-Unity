namespace Core
{
    public class GarrisonShipmentEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.GarrisonShipment;

        public uint ShipmentContainer { get; set; }                       // 0 Shipment Container, References: CharShipmentContainer, NoValue = 0
        public uint GiganticAoi { get; set; }                             // 1 Gigantic AOI, enum { false, true, }; Default: false
        public uint LargeAoi { get; set; }                                // 2 Large AOI, enum { false, true, }; Default: false
    }
}