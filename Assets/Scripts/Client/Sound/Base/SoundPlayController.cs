using Common;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace Client.Sound
{
    internal class SoundPlayController : ITickable
    {
        private class LoadState
        {
            public int RefCount;
            public AsyncOperationHandle<AudioClip> Handle;
            public AudioClip LoadedClip;
            public UniTaskCompletionSource TaskSource;
            public bool IsLoading;
            public readonly HashSet<long> ActiveLoadIds = new();
        }

        [Inject] private SoundModule soundModule;

        private long nextPlayId = 1;
        private long nextLoadId = 1;
        private readonly HashSet<long> pendingRequests = new();
        private readonly Dictionary<long, SoundHandleComponent> activePlays = new();
        private readonly Dictionary<SoundEntry, LoadState> loadStates = new();
        private readonly Dictionary<SoundEntry, SoundEntry> lastPicks = new();
        private readonly List<(long id, float time)> scheduledReleases = new(64);

        internal void Register()
        {
            foreach (SoundEntry soundEntry in soundModule.SoundEntries.ItemList)
                loadStates[soundEntry] = new LoadState();
        }

        internal void Unregister()
        {
            foreach (var soundHandle in activePlays.Values.ToList())
                soundHandle.Release();

            foreach (var state in loadStates.Values)
            {
                state.RefCount = 0;
                state.ActiveLoadIds.Clear();

                if (state.IsLoading)
                    continue;

                if (state.Handle.IsValid())
                {
                    Addressables.Release(state.Handle);
                    state.Handle = default;
                }

                state.LoadedClip = null;
                state.TaskSource = null;
            }
            
            activePlays.Clear();
            pendingRequests.Clear();
            loadStates.Clear();
            lastPicks.Clear();
            scheduledReleases.Clear();
        }

        internal SoundPlayHandle Play(SoundEntry primaryEntry, Vector3 point, Transform parent = null)
        {
            long playId = nextPlayId++;
            pendingRequests.Add(playId);

            SoundEntry selectedEntry = SelectEntry(primaryEntry);
            SoundLoadHandle nextLoadHandle = Preload(selectedEntry, true);
            var nextPlayHandle = new SoundPlayHandle(playId, primaryEntry.Settings.ShouldScheduleDestroy, nextLoadHandle, this);

            PlayAsync(nextPlayHandle, nextLoadHandle, selectedEntry, playId, point, parent).Forget();

            for (var index = 0; index < primaryEntry.ExtraEntries.Count; index++)
                Play(primaryEntry.ExtraEntries[index], point, parent);

            if (primaryEntry != selectedEntry)
                for (var index = 0; index < selectedEntry.ExtraEntries.Count; index++)
                    Play(selectedEntry.ExtraEntries[index], point, parent);

            return nextPlayHandle;

            async UniTaskVoid PlayAsync(
                SoundPlayHandle playHandle,
                SoundLoadHandle loadHandle,
                SoundEntry playedEntry,
                long playEntryId,
                Vector3 targetPoint,
                Transform sourceParent)
            {
                if (loadStates.TryGetValue(loadHandle.Entry, out var state) && state.TaskSource != null)
                    await state.TaskSource.Task;

                if (!TryPlayingClip(playHandle, loadHandle, playedEntry, state, playEntryId, targetPoint, sourceParent))
                    playHandle.Release();

                pendingRequests.Remove(playEntryId);
            }

            bool TryPlayingClip(
                SoundPlayHandle playHandle,
                SoundLoadHandle loadHandle,
                SoundEntry playedEntry,
                LoadState loadState,
                long playEntryId,
                Vector3 targetPoint,
                Transform sourceParent)
            {
                if (!pendingRequests.Contains(playEntryId))
                    return false;

                AudioClip clip = loadState.LoadedClip ?? loadHandle.Entry.AudioClip;
                if (clip == null)
                    return false;

                SoundHandleComponent handleComponent = soundModule.PlayAtPoint(
                    clip,
                    playedEntry.Settings,
                    targetPoint,
                    playHandle,
                    sourceParent,
                    playedEntry.VolumeModifier,
                    playedEntry.Delay,
                    playedEntry.Speed,
                    playedEntry.StartTime);

                if (handleComponent == null)
                    return false;

                activePlays[playEntryId] = handleComponent;

                return true;
            }

            SoundEntry SelectEntry(SoundEntry baseEntry)
            {
                if (baseEntry.AlternativeEntries.Count == 0)
                    return baseEntry;

                int totalCount = 1 + baseEntry.AlternativeEntries.Count;
                int pickedIndex = RandomUtils.Next(0, totalCount);

                SoundEntry picked = pickedIndex == 0 ? baseEntry : baseEntry.AlternativeEntries[pickedIndex - 1];

                // Avoid repeating the last picked entry if there are alternatives
                if (totalCount > 1 && picked == lastPicks.GetValueOrDefault(baseEntry))
                {
                    pickedIndex = (pickedIndex + 1) % totalCount;
                    picked = pickedIndex == 0 ? baseEntry : baseEntry.AlternativeEntries[pickedIndex - 1];
                }

                lastPicks[baseEntry] = picked;

                return picked;
            }
        }

        internal SoundLoadHandle Preload(SoundEntry entry, bool single)
        {
            long loadId = nextLoadId++;
            LoadState primaryState = AcquireLoad(entry, single);
            primaryState.ActiveLoadIds.Add(loadId);

            return new SoundLoadHandle(entry, this, single, loadId);

            LoadState AcquireLoad(SoundEntry loadEntry, bool loadSingle)
            {
                if (!loadStates.TryGetValue(loadEntry, out var state))
                    loadStates[loadEntry] = state = new LoadState();

                state.RefCount++;

                if (state.RefCount == 1 && !state.IsLoading)
                {
                    state.IsLoading = true;
                    state.TaskSource = new UniTaskCompletionSource();
                    LoadAsync(loadEntry, state).Forget();
                }

                if (!loadSingle)
                {
                    for (var index = 0; index < loadEntry.ExtraEntries.Count; index++)
                        AcquireLoad(loadEntry.ExtraEntries[index], false);

                    for (var index = 0; index < loadEntry.AlternativeEntries.Count; index++)
                        AcquireLoad(loadEntry.AlternativeEntries[index], false);
                }

                return state;

                async UniTaskVoid LoadAsync(SoundEntry loadedEntry, LoadState loadState)
                {
                    if (loadedEntry.LocalizedAudioClip is { IsEmpty: false })
                    {
                        loadState.Handle = loadedEntry.LocalizedAudioClip.LoadAssetAsync();
                        await loadState.Handle.ToUniTask();
                        loadState.LoadedClip = loadState.Handle.Result;
                    }
                    else
                        loadState.LoadedClip = loadedEntry.AudioClip;

                    loadState.IsLoading = false;

                    if (loadState.RefCount == 0)
                    {
                        if (loadState.Handle.IsValid())
                        {
                            Addressables.Release(loadState.Handle);
                            loadState.Handle = default;
                        }

                        loadState.LoadedClip = null;
                    }

                    UniTaskCompletionSource taskSource = loadState.TaskSource;
                    loadState.TaskSource = null;
                    taskSource.TrySetResult();
                }
            }
        }

        internal void ScheduleRelease(long playId, float duration)
        {
            scheduledReleases.Add((playId, Time.time + duration));
        }

        internal void ReleasePlay(SoundPlayHandle playHandle)
        {
            pendingRequests.Remove(playHandle.PlayId);
            if (activePlays.Remove(playHandle.PlayId, out SoundHandleComponent soundHandle))
                soundHandle.Release();
        }

        internal void ReleaseLoad(SoundLoadHandle loadHandle)
        {
            if (!loadStates.TryGetValue(loadHandle.Entry, out LoadState state))
                return;

            if (!state.ActiveLoadIds.Remove(loadHandle.LoadId))
                return;

            ReleaseInternal(loadHandle.Entry, loadHandle.Single, state);

            void ReleaseInternal(SoundEntry releaseEntry, bool singleRelease, LoadState loadState)
            {
                loadState.RefCount--;
                Assert.IsTrue(loadState.RefCount >= 0);

                if (!singleRelease)
                {
                    for (var index = 0; index < releaseEntry.ExtraEntries.Count; index++)
                        if (loadStates.TryGetValue(releaseEntry.ExtraEntries[index], out var extraState))
                            ReleaseInternal(releaseEntry.ExtraEntries[index], false, extraState);

                    for (var index = 0; index < releaseEntry.AlternativeEntries.Count; index++)
                        if (loadStates.TryGetValue(releaseEntry.AlternativeEntries[index], out var altState))
                            ReleaseInternal(releaseEntry.AlternativeEntries[index], false, altState);
                }

                if (loadState.RefCount != 0)
                    return;

                if (loadState.IsLoading)
                    return;

                if (loadState.Handle.IsValid())
                {
                    Addressables.Release(loadState.Handle);
                    loadState.Handle = default;
                }
                loadState.LoadedClip = null;
                loadState.TaskSource = null;
            }
        }
        
        internal bool IsValid(long playId) => activePlays.ContainsKey(playId) || pendingRequests.Contains(playId);

        internal bool IsPlaying(long playId) => activePlays.TryGetValue(playId, out var handle) && handle.Source.isPlaying;

        internal AudioSource GetSource(long playId) => activePlays.GetValueOrDefault(playId).Source;

        void ITickable.Tick()
        {
            if (scheduledReleases.Count == 0)
                return;

            float currentTime = Time.time;
            for (int i = scheduledReleases.Count - 1; i >= 0; i--)
            {
                if (currentTime >= scheduledReleases[i].time)
                {
                    long playId = scheduledReleases[i].id;

                    scheduledReleases[i] = scheduledReleases[^1];
                    scheduledReleases.RemoveAt(scheduledReleases.Count - 1);

                    if (activePlays.TryGetValue(playId, out var handleComponent))
                        handleComponent.Release();
                }
            }
        }
    }
}
