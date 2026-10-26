namespace FirstResponse
{
    public enum Treatment
    {
        None,
        Pressure,
        Tourniquet,
        Tighten
    }

    public readonly struct DemoPlayed
    {
        public readonly string Clip;

        public DemoPlayed(string clip)
        {
            Clip = clip;
        }
    }

    public readonly struct CardAccepted
    {
        public readonly MarkerId Card;
        public readonly string Clip;

        public CardAccepted(MarkerId card, string clip)
        {
            Card = card;
            Clip = clip;
        }
    }

    public class DemoStep : ChoiceStep
    {
        private readonly Treatment treatment;

        public DemoStep(string id, string title, string instruction, string icon, string[] options, int correct, int critical, string clip, Treatment treatment)
            : base(id, title, instruction, icon, options, correct, critical)
        {
            Clip = clip;
            this.treatment = treatment;
        }

        public string Clip { get; }

        protected override void OnCorrect(StepContext stepContext)
        {
            Apply(stepContext.Bleeding, treatment);
            EventBus<DemoPlayed>.Publish(new DemoPlayed(Clip));
        }

        public static void Apply(BleedingModel bleeding, Treatment treatment)
        {
            if (treatment == Treatment.Pressure)
            {
                bleeding.PressureStarted();
                bleeding.PressureHeld();
            }
            else if (treatment == Treatment.Tourniquet)
            {
                bleeding.TourniquetPlaced();
            }
            else if (treatment == Treatment.Tighten)
            {
                bleeding.Tighten(1f);
            }
        }
    }

    public class CardStep : ProcedureStep
    {
        private const float HoldSeconds = 1f;
        private readonly Treatment treatment;
        private readonly bool[] flagged = new bool[MarkerSnapshot.Count];
        private float shownFor;

        public CardStep(string id, string title, string instruction, string icon, MarkerId card, string clip, Treatment treatment)
            : base(id, title, instruction, icon)
        {
            Card = card;
            Clip = clip;
            this.treatment = treatment;
        }

        public MarkerId Card { get; }

        public string Clip { get; }

        public override bool NeedsMarkers => true;

        protected override void OnEnter(StepContext context)
        {
            shownFor = 0f;
            System.Array.Clear(flagged, 0, flagged.Length);
        }

        protected override void OnTick(StepContext context, float deltaTime)
        {
            MarkerSnapshot markers = context.Markers;
            bool right = markers.IsVisible(Card) || markers.ScreenHeld;
            for (int i = 0; i < MarkerSnapshot.Count; i++)
            {
                MarkerId other = (MarkerId)i;
                if (other != Card && !right && markers.IsVisible(other) && !flagged[i])
                {
                    flagged[i] = true;
                    Log("wrong_card_" + other);
                    EventBus<Feedback>.Publish(Feedback.Mistake);
                }
            }

            shownFor = right ? shownFor + deltaTime : 0f;
            if (shownFor >= HoldSeconds)
            {
                DemoStep.Apply(context.Bleeding, treatment);
                EventBus<CardAccepted>.Publish(new CardAccepted(Card, Clip));
                Complete();
            }
        }

        protected override float Score()
        {
            return System.Array.IndexOf(flagged, true) < 0 ? 1f : 0.5f;
        }
    }
}
