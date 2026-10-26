using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirstResponse
{
    public class DemoStage : MonoBehaviour
    {
        [SerializeField] private Transform crashSite;
        [SerializeField] private BleedingEffect bleeding;
        [SerializeField] private GameObject legModel;
        [SerializeField] private GameObject armModel;
        [SerializeField] private Material paintTemplate;

        private readonly KitPlayer kit = new KitPlayer();
        private readonly Queue<string> queue = new Queue<string>();
        private bool playing;
        private bool thigh = true;

        public Transform Focus => kit.Focus;

        public bool IsThigh => thigh;

        private void OnEnable()
        {
            EventBus<PlacementChanged>.Subscribe(OnPlacement);
            EventBus<ScenarioChosen>.Subscribe(OnScenario);
            EventBus<StepStarted>.Subscribe(OnStepStarted);
            EventBus<DemoPlayed>.Subscribe(OnDemo);
            EventBus<ScreenRequested>.Subscribe(OnScreen);
        }

        private void OnDisable()
        {
            EventBus<PlacementChanged>.Unsubscribe(OnPlacement);
            EventBus<ScenarioChosen>.Unsubscribe(OnScenario);
            EventBus<StepStarted>.Unsubscribe(OnStepStarted);
            EventBus<DemoPlayed>.Unsubscribe(OnDemo);
            EventBus<ScreenRequested>.Unsubscribe(OnScreen);
        }

        private void OnDestroy()
        {
            Stop();
            kit.Clear();
        }

        private void OnScenario(ScenarioChosen message)
        {
            thigh = message.Scenario == ScenarioId.Thigh;
            if (kit.Root != null)
            {
                Spawn();
            }
        }

        private void OnPlacement(PlacementChanged message)
        {
            if (message.State == PlacementState.Previewing && kit.Root == null)
            {
                Spawn();
            }
        }

        private void OnScreen(ScreenRequested message)
        {
            if (message.Screen == ScreenId.Start)
            {
                Stop();
                kit.Clear();
            }
        }

        private void OnStepStarted(StepStarted message)
        {
            if (message.Index == 0)
            {
                Stop();
                kit.Reset();
            }
        }

        private void OnDemo(DemoPlayed message)
        {
            queue.Enqueue(message.Clip);
            if (!playing && isActiveAndEnabled)
            {
                StartCoroutine(PlayQueue());
            }
        }

        private IEnumerator PlayQueue()
        {
            playing = true;
            while (queue.Count > 0)
            {
                yield return kit.Play(queue.Dequeue());
            }

            playing = false;
        }

        private void Stop()
        {
            StopAllCoroutines();
            queue.Clear();
            playing = false;
        }

        private void Spawn()
        {
            Stop();
            kit.Spawn(thigh ? legModel : armModel, crashSite, paintTemplate);
            bleeding.SetWound(kit.Focus);
        }
    }
}
