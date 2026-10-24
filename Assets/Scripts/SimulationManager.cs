using System.Collections.Generic;
using UnityEngine;

namespace FirstResponse
{
    public class SimulationManager : MonoBehaviour
    {
        [SerializeField] private ScenarioConfig[] scenarios;
        [SerializeField] private ARTrackingManager tracking;

        private readonly List<StepResult> results = new List<StepResult>();
        private List<ProcedureStep> steps;
        private ScenarioConfig config;
        private StepContext context;
        private SessionRecord record;
        private int index;
        private bool running;

        public ProcedureStep CurrentStep => running ? steps[index] : null;

        public BleedingModel Bleeding => context == null ? null : context.Bleeding;

        public float Elapsed { get; private set; }

        private void OnEnable()
        {
            EventBus<ScenarioChosen>.Subscribe(OnChosen);
            EventBus<ScreenRequested>.Subscribe(OnScreen);
        }

        private void OnDisable()
        {
            EventBus<ScenarioChosen>.Unsubscribe(OnChosen);
            EventBus<ScreenRequested>.Unsubscribe(OnScreen);
        }

        private void OnChosen(ScenarioChosen message)
        {
            config = System.Array.Find(scenarios, s => s.scenario == message.Scenario);
        }

        private void OnScreen(ScreenRequested message)
        {
            if (message.Screen == ScreenId.Simulation && !running && config != null)
            {
                Begin();
            }
        }

        public void Begin()
        {
            steps = ScenarioFactory.Build(config);
            context = new StepContext(config, config.CreateBleeding(), tracking.Snapshot);
            record = new SessionRecord { scenarioId = config.scenario.ToString(), startedAt = SessionRecord.Now() };
            results.Clear();
            index = 0;
            Elapsed = 0f;
            running = true;
            StartStep();
        }

        private void Update()
        {
            if (!running)
            {
                return;
            }

            float deltaTime = Time.deltaTime;
            Elapsed += deltaTime;
            context.Bleeding.Tick(deltaTime);
            CurrentStep.Tick(context, deltaTime);
            if (context.Bleeding.State == BleedingState.Controlled && record.timeToControlSec <= 0f)
            {
                record.timeToControlSec = Elapsed;
            }

            if (context.Bleeding.State == BleedingState.Exsanguinated)
            {
                End();
            }
            else if (CurrentStep.IsComplete)
            {
                NextStep();
            }
        }

        public void Choose(int option)
        {
            if (CurrentStep is ChoiceStep choice)
            {
                choice.Choose(option);
            }
        }

        public void Tap()
        {
            if (CurrentStep is TapStep tap)
            {
                tap.Tap();
            }
        }

        private void StartStep()
        {
            CurrentStep.Enter(context);
            EventBus<StepStarted>.Publish(new StepStarted(CurrentStep, index, steps.Count));
        }

        private void NextStep()
        {
            results.Add(CurrentStep.Finish());
            index++;
            if (index >= steps.Count)
            {
                End();
                return;
            }

            StartStep();
        }

        private void End()
        {
            running = false;
            Scoring.Complete(record, results, steps.Count, context, config);
            SessionStorage.Device.Save(record);
            EventBus<SessionFinished>.Publish(new SessionFinished(record, Scoring.Tip(record)));
            EventBus<ScreenRequested>.Publish(new ScreenRequested(ScreenId.Completion));
        }
    }
}
