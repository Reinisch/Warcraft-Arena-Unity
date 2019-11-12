using UnityEngine;
using UnityEngine.Events;
using System.Collections;


namespace Client.Tweener
{
    internal interface ITweenValue
    {
        bool IgnoreTimeScale { get; }
        float Duration { get; }

        void TweenValue(float floatPercentage);
        bool ValidTarget();
    }

    internal struct ColorTween : ITweenValue
    {
        public enum ColorTweenMode
        {
            All,
            Rgb,
            Alpha
        }

        public class ColorTweenCallback : UnityEvent<Color> { }

        private ColorTweenCallback target;
        private Color startColor;
        private Color targetColor;
        private ColorTweenMode tweenMode;

        private float duration;
        private bool ignoreTimeScale;

        public Color StartColor
        {
            get => startColor;
            set => startColor = value;
        }

        public Color TargetColor
        {
            get => targetColor;
            set => targetColor = value;
        }

        public ColorTweenMode TweenMode
        {
            get => tweenMode;
            set => tweenMode = value;
        }

        public float Duration
        {
            get => duration;
            set => duration = value;
        }

        public bool IgnoreTimeScale
        {
            get => ignoreTimeScale;
            set => ignoreTimeScale = value;
        }

        public void TweenValue(float floatPercentage)
        {
            if (!ValidTarget())
                return;

            var newColor = Color.Lerp(startColor, targetColor, floatPercentage);

            if (tweenMode == ColorTweenMode.Alpha)
            {
                newColor.r = startColor.r;
                newColor.g = startColor.g;
                newColor.b = startColor.b;
            }
            else if (tweenMode == ColorTweenMode.Rgb)
            {
                newColor.a = startColor.a;
            }
            target.Invoke(newColor);
        }

        public void AddOnChangedCallback(UnityAction<Color> callback)
        {
            if (target == null)
                target = new ColorTweenCallback();

            target.AddListener(callback);
        }

        public bool ValidTarget()
        {
            return target != null;
        }
    }

    internal struct FloatTween : ITweenValue
    {
        public class FloatTweenCallback : UnityEvent<float> { }

        private FloatTweenCallback target;
        private float startValue;
        private float targetValue;

        private float duration;
        private bool ignoreTimeScale;

        public float StartValue
        {
            get => startValue;
            set => startValue = value;
        }

        public float TargetValue
        {
            get => targetValue;
            set => targetValue = value;
        }

        public float Duration
        {
            get => duration;
            set => duration = value;
        }

        public bool IgnoreTimeScale
        {
            get => ignoreTimeScale;
            set => ignoreTimeScale = value;
        }

        public void TweenValue(float floatPercentage)
        {
            if (!ValidTarget())
                return;

            var newValue = Mathf.Lerp(startValue, targetValue, floatPercentage);
            target.Invoke(newValue);
        }

        public void AddOnChangedCallback(UnityAction<float> callback)
        {
            if (target == null)
                target = new FloatTweenCallback();

            target.AddListener(callback);
        }

        public bool ValidTarget()
        {
            return target != null;
        }
    }

    internal class TweenRunner<T> where T : struct, ITweenValue
    {
        protected MonoBehaviour CoroutineContainer;
        protected IEnumerator Tween;

        private static IEnumerator Start(T tweenInfo)
        {
            if (!tweenInfo.ValidTarget())
                yield break;

            var elapsedTime = 0.0f;
            while (elapsedTime < tweenInfo.Duration)
            {
                elapsedTime += tweenInfo.IgnoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                var percentage = Mathf.Clamp01(elapsedTime / tweenInfo.Duration);
                tweenInfo.TweenValue(percentage);
                yield return null;
            }
            tweenInfo.TweenValue(1.0f);
        }

        public void Init(MonoBehaviour coroutineContainer)
        {
            CoroutineContainer = coroutineContainer;
        }

        public void StartTween(T info)
        {
            if (CoroutineContainer == null)
            {
                Debug.LogWarning("Coroutine container not configured... did you forget to call Init?");
                return;
            }

            StopTween();

            if (!CoroutineContainer.gameObject.activeInHierarchy)
            {
                info.TweenValue(1.0f);
                return;
            }

            Tween = Start(info);
            CoroutineContainer.StartCoroutine(Tween);
        }

        public void StopTween()
        {
            if (Tween != null)
            {
                CoroutineContainer.StopCoroutine(Tween);
                Tween = null;
            }
        }
    }
}