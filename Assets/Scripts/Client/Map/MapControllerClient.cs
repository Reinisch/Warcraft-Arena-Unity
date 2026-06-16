using Core;

namespace Client
{
    /// <summary>
    /// Ensures that map-local presentation objects (e.g. lights) are only active for the map
    /// the player is currently on, so that loaded-but-inactive maps don't affect the visuals
    /// of the active one.
    /// </summary>
    public class MapControllerClient : ScriptableReferenceClient
    {
        private Map activeMap;
        private int trackedMapCount;

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            Map currentMap = Player != null ? Player.Map : World.MapController.PrimaryMap;
            int loadedMapCount = World.MapController.LoadedMaps.Count;

            if (currentMap == activeMap && loadedMapCount == trackedMapCount)
                return;

            activeMap = currentMap;
            trackedMapCount = loadedMapCount;

            foreach (Map map in World.MapController.LoadedMaps)
            {
                MapSettingsClient settingsClient = map.Settings.GetComponent<MapSettingsClient>();
                if (settingsClient != null)
                    settingsClient.SetActive(map == activeMap);
            }
        }
    }
}
