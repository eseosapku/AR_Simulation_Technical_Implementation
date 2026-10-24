using UnityEngine;

namespace FirstResponse
{
    public enum Feedback
    {
        Mistake
    }

    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource effects;
        [SerializeField] private AudioSource heartbeat;
        [SerializeField] private AudioClip tap;
        [SerializeField] private AudioClip stepChime;
        [SerializeField] private AudioClip mistake;
        [SerializeField] private AudioClip pressure;
        [SerializeField] private AudioClip warning;
        [SerializeField] private AudioClip controlled;
        [SerializeField] private AudioClip lost;
        [SerializeField] private AudioClip start;
        [SerializeField] private AudioClip finish;

        public static AudioManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnEnable()
        {
            EventBus<ScreenRequested>.Subscribe(OnScreen);
            EventBus<StepStarted>.Subscribe(OnStep);
            EventBus<BleedingChanged>.Subscribe(OnBleeding);
            EventBus<SessionFinished>.Subscribe(OnFinished);
            EventBus<Feedback>.Subscribe(OnFeedback);
        }

        private void OnDisable()
        {
            EventBus<ScreenRequested>.Unsubscribe(OnScreen);
            EventBus<StepStarted>.Unsubscribe(OnStep);
            EventBus<BleedingChanged>.Unsubscribe(OnBleeding);
            EventBus<SessionFinished>.Unsubscribe(OnFinished);
            EventBus<Feedback>.Unsubscribe(OnFeedback);
        }

        private void OnScreen(ScreenRequested message)
        {
            Play(message.Screen == ScreenId.Simulation ? start : tap);
            if (message.Screen != ScreenId.Simulation && message.Screen != ScreenId.Paused)
            {
                SetHeartbeat(0f);
            }
        }

        private void OnStep(StepStarted message)
        {
            if (message.Index > 0)
            {
                Play(stepChime);
            }
        }

        private void OnBleeding(BleedingChanged message)
        {
            Play(ClipFor(message.State));
            if (message.State == BleedingState.Controlled || message.State == BleedingState.Exsanguinated)
            {
                SetHeartbeat(0f);
            }
        }

        private void OnFinished(SessionFinished message)
        {
            if (message.Record.Passed)
            {
                Play(finish);
            }
        }

        private void OnFeedback(Feedback message)
        {
            Play(mistake);
        }

        public void SetHeartbeat(float urgency)
        {
            if (urgency <= 0f)
            {
                heartbeat.Stop();
                return;
            }

            if (!heartbeat.isPlaying)
            {
                heartbeat.Play();
            }

            heartbeat.volume = Mathf.Lerp(0.2f, 1f, urgency);
            heartbeat.pitch = Mathf.Lerp(1f, 1.8f, urgency);
        }

        private AudioClip ClipFor(BleedingState state)
        {
            switch (state)
            {
                case BleedingState.PressureApplied:
                    return pressure;
                case BleedingState.Uncontrolled:
                case BleedingState.PressureInsufficient:
                    return warning;
                case BleedingState.Controlled:
                    return controlled;
                case BleedingState.Exsanguinated:
                    return lost;
                default:
                    return null;
            }
        }

        private void Play(AudioClip clip)
        {
            if (clip != null)
            {
                effects.PlayOneShot(clip);
            }
        }
    }
}
