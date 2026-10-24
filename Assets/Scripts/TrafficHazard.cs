namespace FirstResponse
{
    public class TrafficHazard : SceneHazard
    {
        public override string Description => "Warning cone set up for traffic";

        protected override void OnSecured()
        {
            StandUpright();
        }
    }
}
