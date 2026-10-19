namespace FirstResponse
{
    public readonly struct ScenarioChosen
    {
        public readonly ScenarioId Scenario;

        public ScenarioChosen(ScenarioId scenario)
        {
            Scenario = scenario;
        }
    }
}
