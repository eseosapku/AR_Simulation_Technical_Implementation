namespace FirstResponse
{
    public enum BleedingState
    {
        Uncontrolled,
        PressureApplied,
        PressureInsufficient,
        TourniquetPlaced,
        Tightening,
        Controlled,
        Exsanguinated
    }

    public readonly struct BleedingChanged
    {
        public readonly BleedingState State;

        public BleedingChanged(BleedingState state)
        {
            State = state;
        }
    }

    public class BleedingModel
    {
        readonly float baseFlow;
        readonly float fatalLoss;
        readonly bool pressureIsEnough;

        public BleedingModel(float baseFlow, float fatalLoss, bool pressureIsEnough)
        {
            this.baseFlow = baseFlow;
            this.fatalLoss = fatalLoss;
            this.pressureIsEnough = pressureIsEnough;
            FlowRate = baseFlow;
        }

        public BleedingState State { get; private set; } = BleedingState.Uncontrolled;

        public float FlowRate { get; private set; }

        public float TotalLoss { get; private set; }

        public float LossFraction => TotalLoss / fatalLoss;

        public bool IsOver => State == BleedingState.Controlled || State == BleedingState.Exsanguinated;

        public void Tick(float deltaTime)
        {
            if (IsOver)
            {
                return;
            }

            TotalLoss += FlowRate * deltaTime;
            if (TotalLoss >= fatalLoss)
            {
                TotalLoss = fatalLoss;
                Set(BleedingState.Exsanguinated, 0f);
            }
        }

        public void PressureStarted()
        {
            if (State == BleedingState.Uncontrolled)
            {
                Set(BleedingState.PressureApplied, baseFlow * 0.25f);
            }
        }

        public void PressureReleased()
        {
            if (State == BleedingState.PressureApplied)
            {
                Set(BleedingState.Uncontrolled, baseFlow);
            }
        }

        public void PressureHeld()
        {
            if (State != BleedingState.PressureApplied)
            {
                return;
            }

            if (pressureIsEnough)
            {
                Set(BleedingState.Controlled, 0f);
            }
            else
            {
                Set(BleedingState.PressureInsufficient, baseFlow * 0.6f);
            }
        }

        public void TourniquetPlaced()
        {
            if (State == BleedingState.PressureInsufficient)
            {
                Set(BleedingState.TourniquetPlaced, FlowRate);
            }
        }

        public void Tighten(float progress)
        {
            if (State != BleedingState.TourniquetPlaced && State != BleedingState.Tightening)
            {
                return;
            }

            if (progress >= 1f)
            {
                Set(BleedingState.Controlled, 0f);
                return;
            }

            Set(BleedingState.Tightening, baseFlow * 0.6f * (1f - progress));
        }

        void Set(BleedingState next, float flow)
        {
            FlowRate = flow;
            if (next == State)
            {
                return;
            }

            State = next;
            EventBus<BleedingChanged>.Publish(new BleedingChanged(next));
        }
    }
}
