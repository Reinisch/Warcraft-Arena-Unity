using System;
using System.Collections.Generic;

namespace Common
{
    public class EventHandler
    {
        private static readonly Dictionary<object, Dictionary<GameEvents, List<Delegate>>> EventActionsByTarget = new Dictionary<object, Dictionary<GameEvents, List<Delegate>>>();
        private static readonly Dictionary<GameEvents, List<Delegate>> GlobalEventActions = new Dictionary<GameEvents, List<Delegate>>();

        private static void AddEvent(object target, GameEvents gameGameEvent, Delegate action)
        {
            if (!EventActionsByTarget.TryGetValue(target, out var targetEvents))
            {
                targetEvents = new Dictionary<GameEvents, List<Delegate>>();
                EventActionsByTarget.Add(target, targetEvents);
            }

            if (targetEvents.TryGetValue(gameGameEvent, out var eventActions))
                eventActions.Add(action);
            else
                targetEvents.Add(gameGameEvent, new List<Delegate> { action });
        }

        private static void RemoveEvent(object target, GameEvents gameGameEvent, Delegate action)
        {
            var eventActions = FindEventActions(target, gameGameEvent);
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

                if (eventActions.Count == 0 && EventActionsByTarget.TryGetValue(target, out var targetEvents))
                {
                    targetEvents.Remove(gameGameEvent);
                    if (targetEvents.Count == 0)
                        EventActionsByTarget.Remove(target);
                }
            }
        }

        private static void AddGlobalEvent(GameEvents gameGameEvent, Delegate action)
        {
            if (GlobalEventActions.TryGetValue(gameGameEvent, out var eventActions))
                eventActions.Add(action);
            else
                GlobalEventActions.Add(gameGameEvent, new List<Delegate> { action });
        }

        private static void RemoveGlobalEvent(GameEvents gameGameEvent, Delegate action)
        {
            var eventActions = FindGlobalEventActions(gameGameEvent);
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

        private static List<Delegate> FindEventActions(object target, GameEvents gameGameEvent)
        {
            return EventActionsByTarget.TryGetValue(target, out var actions) && actions.TryGetValue(gameGameEvent, out var targetActions) ? targetActions : null;
        }

        private static List<Delegate> FindGlobalEventActions(GameEvents gameGameEvent)
        {
            return GlobalEventActions.TryGetValue(gameGameEvent, out var targetActions) ? targetActions : null;
        }

        public static void RegisterEvent(object target, GameEvents gameGameEvent, Action action)
        {
            AddEvent(target, gameGameEvent, action);
        }

        public static void RegisterEvent<T1>(object target, GameEvents gameGameEvent, Action<T1> action)
        {
            AddEvent(target, gameGameEvent, action);
        }

        public static void RegisterEvent<T1, T2>(object target, GameEvents gameGameEvent, Action<T1, T2> action)
        {
            AddEvent(target, gameGameEvent, action);
        }

        public static void RegisterEvent<T1, T2, T3>(object target, GameEvents gameGameEvent, Action<T1, T2, T3> action)
        {
            AddEvent(target, gameGameEvent, action);
        }

        public static void RegisterEvent<T1, T2, T3, T4>(object target, GameEvents gameGameEvent, Action<T1, T2, T3, T4> action)
        {
            AddEvent(target, gameGameEvent, action);
        }

        public static void RegisterEvent<T1, T2, T3, T4, T5>(object target, GameEvents gameGameEvent, Action<T1, T2, T3, T4, T5> action)
        {
            AddEvent(target, gameGameEvent, action);
        }

        public static void RegisterEvent(GameEvents gameGameEvent, Action action)
        {
            AddGlobalEvent(gameGameEvent, action);
        }

        public static void RegisterEvent<T1>(GameEvents gameGameEvent, Action<T1> action)
        {
            AddGlobalEvent(gameGameEvent, action);
        }

        public static void RegisterEvent<T1, T2>(GameEvents gameGameEvent, Action<T1, T2> action)
        {
            AddGlobalEvent(gameGameEvent, action);
        }

        public static void RegisterEvent<T1, T2, T3>(GameEvents gameGameEvent, Action<T1, T2, T3> action)
        {
            AddGlobalEvent(gameGameEvent, action);
        }

        public static void RegisterEvent<T1, T2, T3, T4>(GameEvents gameGameEvent, Action<T1, T2, T3, T4> action)
        {
            AddGlobalEvent(gameGameEvent, action);
        }

        public static void RegisterEvent<T1, T2, T3, T4, T5>(GameEvents gameGameEvent, Action<T1, T2, T3, T4, T5> action)
        {
            AddGlobalEvent(gameGameEvent, action);
        }

        public static void UnregisterEvent(object target, GameEvents gameGameEvent, Action action)
        {
            RemoveEvent(target, gameGameEvent, action);
        }

        public static void UnregisterEvent<T1>(object target, GameEvents gameGameEvent, Action<T1> action)
        {
            RemoveEvent(target, gameGameEvent, action);
        }

        public static void UnregisterEvent<T1, T2>(object target, GameEvents gameGameEvent, Action<T1, T2> action)
        {
            RemoveEvent(target, gameGameEvent, action);
        }

        public static void UnregisterEvent<T1, T2, T3>(object target, GameEvents gameGameEvent, Action<T1, T2, T3> action)
        {
            RemoveEvent(target, gameGameEvent, action);
        }

        public static void UnregisterEvent<T1, T2, T3, T4>(object target, GameEvents gameGameEvent, Action<T1, T2, T3, T4> action)
        {
            RemoveEvent(target, gameGameEvent, action);
        }

        public static void UnregisterEvent<T1, T2, T3, T4, T5>(object target, GameEvents gameGameEvent, Action<T1, T2, T3, T4, T5> action)
        {
            RemoveEvent(target, gameGameEvent, action);
        }

        public static void UnregisterEvent(GameEvents gameGameEvent, Action action)
        {
            RemoveGlobalEvent(gameGameEvent, action);
        }

        public static void UnregisterEvent<T1>(GameEvents gameGameEvent, Action<T1> action)
        {
            RemoveGlobalEvent(gameGameEvent, action);
        }

        public static void UnregisterEvent<T1, T2>(GameEvents gameGameEvent, Action<T1, T2> action)
        {
            RemoveGlobalEvent(gameGameEvent, action);
        }

        public static void UnregisterEvent<T1, T2, T3>(GameEvents gameGameEvent, Action<T1, T2, T3> action)
        {
            RemoveGlobalEvent(gameGameEvent, action);
        }

        public static void UnregisterEvent<T1, T2, T3, T4>(GameEvents gameGameEvent, Action<T1, T2, T3, T4> action)
        {
            RemoveGlobalEvent(gameGameEvent, action);
        }

        public static void UnregisterEvent<T1, T2, T3, T4, T5>(GameEvents gameGameEvent, Action<T1, T2, T3, T4, T5> action)
        {
            RemoveGlobalEvent(gameGameEvent, action);
        }

        public static void ExecuteEvent(object target, GameEvents gameGameEvent)
        {
            var eventActions = FindEventActions(target, gameGameEvent);
            if (eventActions != null)
                for (int i = 0; i < eventActions.Count; i++)
                    (eventActions[i] as Action)?.Invoke();
        }

        public static void ExecuteEvent<T1>(object target, GameEvents gameGameEvent, T1 arg1)
        {
            var eventActions = FindEventActions(target, gameGameEvent);
            if (eventActions != null)
                for (int i = 0; i < eventActions.Count; i++)
                    (eventActions[i] as Action<T1>)?.Invoke(arg1);
        }

        public static void ExecuteEvent<T1, T2>(object target, GameEvents gameGameEvent, T1 arg1, T2 arg2)
        {
            var eventActions = FindEventActions(target, gameGameEvent);
            if (eventActions != null)
                for (int i = 0; i < eventActions.Count; i++)
                    (eventActions[i] as Action<T1, T2>)?.Invoke(arg1, arg2);
        }

        public static void ExecuteEvent<T1, T2, T3>(object target, GameEvents gameGameEvent, T1 arg1, T2 arg2, T3 arg3)
        {
            var eventActions = FindEventActions(target, gameGameEvent);
            if (eventActions != null)
                for (int i = 0; i < eventActions.Count; i++)
                    (eventActions[i] as Action<T1, T2, T3>)?.Invoke(arg1, arg2, arg3);
        }

        public static void ExecuteEvent<T1, T2, T3, T4>(object target, GameEvents gameGameEvent, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var eventActions = FindEventActions(target, gameGameEvent);
            if (eventActions != null)
                for (int i = 0; i < eventActions.Count; i++)
                    (eventActions[i] as Action<T1, T2, T3, T4>)?.Invoke(arg1, arg2, arg3, arg4);
        }

        public static void ExecuteEvent<T1, T2, T3, T4, T5>(object target, GameEvents gameGameEvent, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            var eventActions = FindEventActions(target, gameGameEvent);
            if (eventActions != null)
                for (int i = 0; i < eventActions.Count; i++)
                    (eventActions[i] as Action<T1, T2, T3, T4, T5>)?.Invoke(arg1, arg2, arg3, arg4, arg5);
        }

        public static void ExecuteEvent(GameEvents gameGameEvent)
        {
            var eventActions = FindGlobalEventActions(gameGameEvent);
            if (eventActions != null)
                for (int i = 0; i < eventActions.Count; i++)
                    (eventActions[i] as Action)?.Invoke();
        }

        public static void ExecuteEvent<T1>(GameEvents gameGameEvent, T1 arg1)
        {
            var eventActions = FindGlobalEventActions(gameGameEvent);
            if (eventActions != null)
                for (int i = 0; i < eventActions.Count; i++)
                    (eventActions[i] as Action<T1>)?.Invoke(arg1);
        }

        public static void ExecuteEvent<T1, T2>(GameEvents gameGameEvent, T1 arg1, T2 arg2)
        {
            var eventActions = FindGlobalEventActions(gameGameEvent);
            if (eventActions != null)
                for (int i = 0; i < eventActions.Count; i++)
                    (eventActions[i] as Action<T1, T2>)?.Invoke(arg1, arg2);
        }

        public static void ExecuteEvent<T1, T2, T3>(GameEvents gameGameEvent, T1 arg1, T2 arg2, T3 arg3)
        {
            var eventActions = FindGlobalEventActions(gameGameEvent);
            if (eventActions != null)
                for (int i = 0; i < eventActions.Count; i++)
                    (eventActions[i] as Action<T1, T2, T3>)?.Invoke(arg1, arg2, arg3);
        }

        public static void ExecuteEvent<T1, T2, T3, T4>(GameEvents gameGameEvent, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var eventActions = FindGlobalEventActions(gameGameEvent);
            if (eventActions != null)
                for (int i = 0; i < eventActions.Count; i++)
                    (eventActions[i] as Action<T1, T2, T3, T4>)?.Invoke(arg1, arg2, arg3, arg4);
        }

        public static void ExecuteEvent<T1, T2, T3, T4, T5>(GameEvents gameGameEvent, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            var eventActions = FindGlobalEventActions(gameGameEvent);
            if (eventActions != null)
                for (int i = 0; i < eventActions.Count; i++)
                    (eventActions[i] as Action<T1, T2, T3, T4, T5>)?.Invoke(arg1, arg2, arg3, arg4, arg5);
        }
    }
}
