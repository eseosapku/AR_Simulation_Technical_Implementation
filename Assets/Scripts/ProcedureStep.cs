using System.Collections.Generic;

namespace FirstResponse
{
    public readonly struct StepStarted
    {
        public readonly ProcedureStep Step;
        public readonly int Index;
        public readonly int Count;

        public StepStarted(ProcedureStep step, int index, int count)
        {
            Step = step;
            Index = index;
            Count = count;
        }
    }

    public class StepContext
    {
        public readonly ScenarioConfig Config;
        public readonly BleedingModel Bleeding;
        public readonly MarkerSnapshot Markers;
        public readonly List<string> CriticalErrors = new List<string>();

        public StepContext(ScenarioConfig config, BleedingModel bleeding, MarkerSnapshot markers)
        {
            Config = config;
            Bleeding = bleeding;
            Markers = markers;
        }
    }

    public abstract class ProcedureStep
    {
        private readonly List<string> events = new List<string>();

        protected ProcedureStep(string id, string title, string instruction, string icon)
        {
            Id = id;
            Title = title;
            Instruction = instruction;
            Icon = icon;
        }

        public string Id { get; }

        public string Title { get; }

        public string Instruction { get; }

        public string Icon { get; }

        public float Elapsed { get; private set; }

        public bool IsComplete { get; private set; }

        public virtual bool NeedsMarkers => false;

        public void Enter(StepContext context)
        {
            Elapsed = 0f;
            IsComplete = false;
            events.Clear();
            OnEnter(context);
        }

        public void Tick(StepContext context, float deltaTime)
        {
            if (IsComplete)
            {
                return;
            }

            Elapsed += deltaTime;
            OnTick(context, deltaTime);
        }

        public StepResult Finish()
        {
            return new StepResult
            {
                stepId = Id,
                completed = IsComplete,
                durationSec = Elapsed,
                score = IsComplete ? Score() : 0f,
                events = new List<string>(events)
            };
        }

        protected virtual void OnEnter(StepContext context)
        {
        }

        protected virtual void OnTick(StepContext context, float deltaTime)
        {
        }

        protected abstract float Score();

        protected void Complete()
        {
            IsComplete = true;
        }

        protected void Log(string message)
        {
            events.Add(message);
        }

        protected int Count(string message)
        {
            return events.FindAll(e => e == message).Count;
        }
    }
}
