namespace FirstResponse
{
    public class ChoiceStep : ProcedureStep
    {
        private readonly string[] options;
        private readonly int correct;
        private readonly int critical;
        private StepContext context;
        private bool rightFirstTime;

        public ChoiceStep(string id, string title, string instruction, string icon, string[] options, int correct, int critical = -1)
            : base(id, title, instruction, icon)
        {
            this.options = options;
            this.correct = correct;
            this.critical = critical;
        }

        public string[] Options => options;

        protected override void OnEnter(StepContext stepContext)
        {
            context = stepContext;
            rightFirstTime = true;
        }

        public bool Choose(int index)
        {
            if (IsComplete || index < 0 || index >= options.Length)
            {
                return false;
            }

            if (index == critical)
            {
                context.CriticalErrors.Add(Id + ": " + options[index]);
            }

            if (index != correct)
            {
                rightFirstTime = false;
                Log("wrong_" + options[index]);
                EventBus<Feedback>.Publish(Feedback.Mistake);
                return false;
            }

            Complete();
            return true;
        }

        protected override float Score()
        {
            return rightFirstTime ? 1f : 0.5f;
        }
    }

    public class TapStep : ProcedureStep
    {
        private readonly int required;
        private int done;

        public TapStep(string id, string title, string instruction, string icon, int required)
            : base(id, title, instruction, icon)
        {
            this.required = required;
        }

        public int Remaining => required - done;

        protected override void OnEnter(StepContext context)
        {
            done = 0;
        }

        public void Tap()
        {
            if (IsComplete)
            {
                return;
            }

            done++;
            if (done >= required)
            {
                Complete();
            }
        }

        protected override float Score()
        {
            return 1f;
        }
    }
}
