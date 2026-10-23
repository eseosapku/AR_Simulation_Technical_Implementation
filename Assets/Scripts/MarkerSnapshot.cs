using UnityEngine;

namespace FirstResponse
{
    public enum MarkerId
    {
        Wound,
        Tourniquet,
        Windlass,
        Joint
    }

    public class MarkerSnapshot
    {
        public const int Count = 4;

        readonly bool[] visible = new bool[Count];
        readonly Pose[] poses = new Pose[Count];

        public void Set(MarkerId id, bool isVisible, Pose pose)
        {
            visible[(int)id] = isVisible;
            poses[(int)id] = pose;
        }

        public bool IsVisible(MarkerId id) => visible[(int)id];

        public Vector3 Position(MarkerId id) => poses[(int)id].position;

        public Quaternion Rotation(MarkerId id) => poses[(int)id].rotation;

        public float DistanceCm(MarkerId a, MarkerId b)
        {
            return Vector3.Distance(Position(a), Position(b)) * 100f;
        }

        public static bool TryParse(string imageName, out MarkerId id)
        {
            foreach (MarkerId candidate in System.Enum.GetValues(typeof(MarkerId)))
            {
                if (imageName.EndsWith(candidate.ToString()))
                {
                    id = candidate;
                    return true;
                }
            }

            id = MarkerId.Wound;
            return false;
        }
    }
}
