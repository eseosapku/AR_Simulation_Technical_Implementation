namespace FirstResponse
{
    public enum ScreenId
    {
        Start,
        HowItWorks,
        ScenarioSelect,
        SafetyNotice,
        Setup,
        Simulation,
        Paused,
        Completion,
        History
    }

    public enum ScenarioId
    {
        Forearm,
        Thigh
    }

    public readonly struct ScreenRequested
    {
        public readonly ScreenId Screen;

        public ScreenRequested(ScreenId screen)
        {
            Screen = screen;
        }
    }

    public readonly struct ScenarioChosen
    {
        public readonly ScenarioId Scenario;

        public ScenarioChosen(ScenarioId scenario)
        {
            Scenario = scenario;
        }
    }
}
