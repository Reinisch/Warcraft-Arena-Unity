using System.Collections.Generic;

namespace Core
{
    public static class PhotonBoltHelper
    {
        public static readonly List<string> FullAvailableRegions = new List<string>
        {
            "eu",
            "us",
            "asia",
            "jp",
            "au",
            "usw",
            "sa",
            "cae",
            "kr",
            "in",
            "ru",
            "rue",
        };

        public static string RegionToString(string code)
        {
            switch (code)
            {
                case "eu":
                    return "Europe";
                case "us":
                    return "US East";
                case "asia":
                    return "Asia";
                case "jp":
                    return "Japan";
                case "au":
                    return "Australia";
                case "usw":
                    return "US West";
                case "sa":
                    return "South America";
                case "cae":
                    return "Canada East";
                case "kr":
                    return "South Korea";
                case "in":
                    return "India";
                case "ru":
                    return "Russia";
                case "rue":
                    return "Russia East";
                default:
                    goto case "eu";
            }
        }
    }
}
