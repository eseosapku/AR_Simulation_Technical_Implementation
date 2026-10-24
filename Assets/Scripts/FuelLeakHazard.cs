using UnityEngine;

namespace FirstResponse
{
    public class FuelLeakHazard : SceneHazard
    {
        private const float MoveAwayMetres = 0.4f;

        public override string Description => "Leaking fuel moved away and capped";

        protected override void OnSecured()
        {
            StandUpright();
            Vector3 away = transform.localPosition;
            away.y = 0f;
            transform.localPosition += away.normalized * MoveAwayMetres;
        }
    }
}
