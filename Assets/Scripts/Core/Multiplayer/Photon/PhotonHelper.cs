using System.Collections.Generic;

namespace Core
{
    public static class PhotonHelper
    {
        public static readonly List<CloudRegionCode> FullAvailableRegions = new List<CloudRegionCode>
        {
            CloudRegionCode.eu,
            CloudRegionCode.us,
            CloudRegionCode.asia,
            CloudRegionCode.jp,
            CloudRegionCode.au,
            CloudRegionCode.usw,
            CloudRegionCode.sa,
            CloudRegionCode.cae,
            CloudRegionCode.kr,
            CloudRegionCode.@in,
            CloudRegionCode.ru,
            CloudRegionCode.rue,
        };

        public static string RegionToString(CloudRegionCode code)
        {
            switch (code)
            {
                case CloudRegionCode.eu:
                    return "Europe";
                case CloudRegionCode.us:
                    return "US East";
                case CloudRegionCode.asia:
                    return "Asia";
                case CloudRegionCode.jp:
                    return "Japan";
                case CloudRegionCode.au:
                    return "Australia";
                case CloudRegionCode.usw:
                    return "US West";
                case CloudRegionCode.sa:
                    return "South America";
                case CloudRegionCode.cae:
                    return "Canada East";
                case CloudRegionCode.kr:
                    return "South Korea";
                case CloudRegionCode.@in:
                    return "India";
                case CloudRegionCode.ru:
                    return "Russia";
                case CloudRegionCode.rue:
                    return "Russia East";
                default:
                    goto case CloudRegionCode.eu;
            }
        }
    }
}
