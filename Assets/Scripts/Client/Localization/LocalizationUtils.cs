namespace Client
{
    public static class LocalizationUtils
    {
        public static string LobbyDisconnectedReasonStringFormat = "Session ended! Reason: {0}";
        public static string LobbyClientConnectFailedString = "Failed to connect";
        public static string LobbyClientConnectSuccessString = "Connected!";
        public static string LobbyClientStartFailedString = "Client failed to start";
        public static string LobbyClientStartSuccessString = "Client started";
        public static string LobbyServerStartFailedString = "Server failed to start";
        public static string LobbyServerStartSuccessString = "Server started";
        public static string LobbyServerStartString = "Starting server...";
        public static string LobbyClientStartString = "Starting client...";
        public static string LobbyConnectionStartString = "Connecting...";

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
