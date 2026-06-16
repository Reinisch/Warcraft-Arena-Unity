using System;
using System.Collections.Generic;

namespace Common
{
    public class EventBus
    {
        private readonly Dictionary<object, Dictionary<GameEvents, List<Delegate>>> eventsByTarget = new();
        private readonly Dictionary<GameEvents, List<Delegate>> globalEvents = new();

        private void AddEvent(object target, GameEvents gameEvent, Delegate action)
        {
            if (!eventsByTarget.TryGetValue(target, out var targetEvents))
            {
                targetEvents = new Dictionary<GameEvents, List<Delegate>>();
                eventsByTarget.Add(target, targetEvents);
            }

            if (targetEvents.TryGetValue(gameEvent, out var eventActions))
                eventActions.Add(action);
            else
                targetEvents.Add(gameEvent, new List<Delegate> { action });
        }

        private void RemoveEvent(object target, GameEvents gameEvent, Delegate action)
        {
            var eventActions = FindEventActions(target, gameEvent);
            if (eventActions != null)
            {
                for (int i = 0; i < eventActions.Count; ++i)
                {
                    if (eventActions[i].Equals(action))
                    {
                        eventActions.RemoveAt(i);
                        break;
                    }
                }

                if (eventActions.Count == 0 && eventsByTarget.TryGetValue(target, out var targetEvents))
                {
                    targetEvents.Remove(gameEvent);
                    if (targetEvents.Count == 0)
                        eventsByTarget.Remove(target);
                }
            }
        }

        private void AddGlobalEvent(GameEvents gameEvent, Delegate action)
        {
            if (globalEvents.TryGetValue(gameEvent, out var eventActions))
                eventActions.Add(action);
            else
                globalEvents.Add(gameEvent, new List<Delegate> { action });
        }

        private void RemoveGlobalEvent(GameEvents gameEvent, Delegate action)
        {
            var eventActions = FindGlobalEventActions(gameEvent);
            if (eventActions != null)
            {
                for (int i = 0; i < eventActions.Count; ++i)
                {
                    if (eventActions[i].Equals(action))
                    {
                        eventActions.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private List<Delegate> FindEventActions(object target, GameEvents gameEvent)
        {
            return eventsByTarget.TryGetValue(target, out var actions) && actions.TryGetValue(gameEvent, out var targetActions) ? targetActions : null;
        }

        private List<Delegate> FindGlobalEventActions(GameEvents gameEvent)
        {
            return globalEvents.TryGetValue(gameEvent, out var targetActions) ? targetActions : null;
        }

        public void RegisterEvent(object target, GameEvents gameEvent, Action action) => AddEvent(target, gameEvent, action);
        public void RegisterEvent<T1>(object target, GameEvents gameEvent, Action<T1> action) => AddEvent(target, gameEvent, action);
        public void RegisterEvent<T1, T2>(object target, GameEvents gameEvent, Action<T1, T2> action) => AddEvent(target, gameEvent, action);
        public void RegisterEvent<T1, T2, T3>(object target, GameEvents gameEvent, Action<T1, T2, T3> action) => AddEvent(target, gameEvent, action);
        public void RegisterEvent<T1, T2, T3, T4>(object target, GameEvents gameEvent, Action<T1, T2, T3, T4> action) => AddEvent(target, gameEvent, action);
        public void RegisterEvent<T1, T2, T3, T4, T5>(object target, GameEvents gameEvent, Action<T1, T2, T3, T4, T5> action) => AddEvent(target, gameEvent, action);

        public void RegisterEvent(GameEvents gameEvent, Action action) => AddGlobalEvent(gameEvent, action);
        public void RegisterEvent<T1>(GameEvents gameEvent, Action<T1> action) => AddGlobalEvent(gameEvent, action);
        public void RegisterEvent<T1, T2>(GameEvents gameEvent, Action<T1, T2> action) => AddGlobalEvent(gameEvent, action);
        public void RegisterEvent<T1, T2, T3>(GameEvents gameEvent, Action<T1, T2, T3> action) => AddGlobalEvent(gameEvent, action);
        public void RegisterEvent<T1, T2, T3, T4>(GameEvents gameEvent, Action<T1, T2, T3, T4> action) => AddGlobalEvent(gameEvent, action);
        public void RegisterEvent<T1, T2, T3, T4, T5>(GameEvents gameEvent, Action<T1, T2, T3, T4, T5> action) => AddGlobalEvent(gameEvent, action);

        public void UnregisterEvent(object target, GameEvents gameEvent, Action action) => RemoveEvent(target, gameEvent, action);
        public void UnregisterEvent<T1>(object target, GameEvents gameEvent, Action<T1> action) => RemoveEvent(target, gameEvent, action);
        public void UnregisterEvent<T1, T2>(object target, GameEvents gameEvent, Action<T1, T2> action) => RemoveEvent(target, gameEvent, action);
        public void UnregisterEvent<T1, T2, T3>(object target, GameEvents gameEvent, Action<T1, T2, T3> action) => RemoveEvent(target, gameEvent, action);
        public void UnregisterEvent<T1, T2, T3, T4>(object target, GameEvents gameEvent, Action<T1, T2, T3, T4> action) => RemoveEvent(target, gameEvent, action);
        public void UnregisterEvent<T1, T2, T3, T4, T5>(object target, GameEvents gameEvent, Action<T1, T2, T3, T4, T5> action) => RemoveEvent(target, gameEvent, action);

        public void UnregisterEvent(GameEvents gameEvent, Action action) => RemoveGlobalEvent(gameEvent, action);
        public void UnregisterEvent<T1>(GameEvents gameEvent, Action<T1> action) => RemoveGlobalEvent(gameEvent, action);
        public void UnregisterEvent<T1, T2>(GameEvents gameEvent, Action<T1, T2> action) => RemoveGlobalEvent(gameEvent, action);
        public void UnregisterEvent<T1, T2, T3>(GameEvents gameEvent, Action<T1, T2, T3> action) => RemoveGlobalEvent(gameEvent, action);
        public void UnregisterEvent<T1, T2, T3, T4>(GameEvents gameEvent, Action<T1, T2, T3, T4> action) => RemoveGlobalEvent(gameEvent, action);
        public void UnregisterEvent<T1, T2, T3, T4, T5>(GameEvents gameEvent, Action<T1, T2, T3, T4, T5> action) => RemoveGlobalEvent(gameEvent, action);

        public void ExecuteEvent(object target, GameEvents gameEvent)
        {
            var eventActions = FindEventActions(target, gameEvent);
            if (eventActions != null)
                for (int i = 0; i < eventActions.Count; i++)
                    (eventActions[i] as Action)?.Invoke();
        }

        public void ExecuteEvent<T1>(object target, GameEvents gameEvent, T1 arg1)
        {
            var eventActions = FindEventActions(target, gameEvent);
            if (eventActions != null)
                for (int i = 0; i < eventActions.Count; i++)
                    (eventActions[i] as Action<T1>)?.Invoke(arg1);
        }

        public void ExecuteEvent<T1, T2>(object target, GameEvents gameEvent, T1 arg1, T2 arg2)
        {
            var eventActions = FindEventActions(target, gameEvent);
            if (eventActions != null)
                for (int i = 0; i < eventActions.Count; i++)
                    (eventActions[i] as Action<T1, T2>)?.Invoke(arg1, arg2);
        }

        public void ExecuteEvent<T1, T2, T3>(object target, GameEvents gameEvent, T1 arg1, T2 arg2, T3 arg3)
        {
            var eventActions = FindEventActions(target, gameEvent);
            if (eventActions != null)
                for (int i = 0; i < eventActions.Count; i++)
                    (eventActions[i] as Action<T1, T2, T3>)?.Invoke(arg1, arg2, arg3);
        }

        public void ExecuteEvent<T1, T2, T3, T4>(object target, GameEvents gameEvent, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var eventActions = FindEventActions(target, gameEvent);
            if (eventActions != null)
                for (int i = 0; i < eventActions.Count; i++)
                    (eventActions[i] as Action<T1, T2, T3, T4>)?.Invoke(arg1, arg2, arg3, arg4);
        }

        public void ExecuteEvent<T1, T2, T3, T4, T5>(object target, GameEvents gameEvent, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            var eventActions = FindEventActions(target, gameEvent);
            if (eventActions != null)
                for (int i = 0; i < eventActions.Count; i++)
                    (eventActions[i] as Action<T1, T2, T3, T4, T5>)?.Invoke(arg1, arg2, arg3, arg4, arg5);
        }

        public void ExecuteEvent(GameEvents gameEvent)
        {
            var eventActions = FindGlobalEventActions(gameEvent);
            if (eventActions != null)
                for (int i = 0; i < eventActions.Count; i++)
                    (eventActions[i] as Action)?.Invoke();
        }

        public void ExecuteEvent<T1>(GameEvents gameEvent, T1 arg1)
        {
            var eventActions = FindGlobalEventActions(gameEvent);
            if (eventActions != null)
                for (int i = 0; i < eventActions.Count; i++)
                    (eventActions[i] as Action<T1>)?.Invoke(arg1);
        }

        public void ExecuteEvent<T1, T2>(GameEvents gameEvent, T1 arg1, T2 arg2)
        {
            var eventActions = FindGlobalEventActions(gameEvent);
            if (eventActions != null)
                for (int i = 0; i < eventActions.Count; i++)
                    (eventActions[i] as Action<T1, T2>)?.Invoke(arg1, arg2);
        }

        public void ExecuteEvent<T1, T2, T3>(GameEvents gameEvent, T1 arg1, T2 arg2, T3 arg3)
        {
            var eventActions = FindGlobalEventActions(gameEvent);
            if (eventActions != null)
                for (int i = 0; i < eventActions.Count; i++)
                    (eventActions[i] as Action<T1, T2, T3>)?.Invoke(arg1, arg2, arg3);
        }

        public void ExecuteEvent<T1, T2, T3, T4>(GameEvents gameEvent, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var eventActions = FindGlobalEventActions(gameEvent);
            if (eventActions != null)
                for (int i = 0; i < eventActions.Count; i++)
                    (eventActions[i] as Action<T1, T2, T3, T4>)?.Invoke(arg1, arg2, arg3, arg4);
        }

        public void ExecuteEvent<T1, T2, T3, T4, T5>(GameEvents gameEvent, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            var eventActions = FindGlobalEventActions(gameEvent);
            if (eventActions != null)
                for (int i = 0; i < eventActions.Count; i++)
                    (eventActions[i] as Action<T1, T2, T3, T4, T5>)?.Invoke(arg1, arg2, arg3, arg4, arg5);
        }
    }
}
