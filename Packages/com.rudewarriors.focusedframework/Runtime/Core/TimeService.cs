using UnityEngine;
using System.Collections;
using RudeWarriors.Framework.Core;

namespace RudeWarriors.Framework.Core
{
    public interface ITimeService
    {
        float DeltaTime { get; }
        float TimeScale { get; }
        bool IsPaused { get; }

        void SetTimeScale(float scale);
        void Pause();
        void Resume();
        void SmoothTimeScale(float target, float duration);
    }

    /// <summary>
    /// Controls Unity's time scale, pause state, and smooth time transitions.
    /// Register this through FrameworkBootstrapper.
    /// </summary>
    public class TimeService : MonoBehaviour, ITimeService
    {
        [Range(0f, 3f)] [SerializeField] private float defaultTimeScale = 1f;
        [SerializeField] private bool autoRegister = true;

        private bool _isPaused;
        private Coroutine _smoothRoutine;

        public float DeltaTime => _isPaused ? 0f : UnityEngine.Time.deltaTime;
        public float TimeScale => UnityEngine.Time.timeScale;
        public bool IsPaused => _isPaused;

        private void Awake()
        {
            if (autoRegister)
                ServiceLocator.Register<ITimeService>(this);

            UnityEngine.Time.timeScale = defaultTimeScale;
        }

        public void SetTimeScale(float scale)
        {
            UnityEngine.Time.timeScale = Mathf.Clamp(scale, 0f, 3f);
            _isPaused = UnityEngine.Time.timeScale <= 0.001f;
        }

        public void Pause()
        {
            if (_isPaused) return;
            UnityEngine.Time.timeScale = 0f;
            _isPaused = true;
        }

        public void Resume()
        {
            if (!_isPaused) return;
            UnityEngine.Time.timeScale = defaultTimeScale;
            _isPaused = false;
        }

        public void SmoothTimeScale(float target, float duration)
        {
            if (_smoothRoutine != null)
                StopCoroutine(_smoothRoutine);

            _smoothRoutine = StartCoroutine(SmoothScaleCoroutine(target, duration));
        }

        private IEnumerator SmoothScaleCoroutine(float target, float duration)
        {
            float start = UnityEngine.Time.timeScale;
            float t = 0f;

            while (t < duration)
            {
                UnityEngine.Time.timeScale = Mathf.Lerp(start, target, t / duration);
                t += UnityEngine.Time.unscaledDeltaTime;
                yield return null;
            }

            UnityEngine.Time.timeScale = target;
            _isPaused = target <= 0.001f;
        }
    }
}
