using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace FirstResponse
{
    public class ARTrackingManager : MonoBehaviour
    {
        [SerializeField] ARTrackedImageManager imageManager;

        readonly MarkerSnapshot snapshot = new MarkerSnapshot();

        public MarkerSnapshot Snapshot => snapshot;

        void OnEnable()
        {
            imageManager.trackablesChanged.AddListener(OnChanged);
        }

        void OnDisable()
        {
            imageManager.trackablesChanged.RemoveListener(OnChanged);
        }

        void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> changes)
        {
            foreach (var image in changes.added)
            {
                Record(image);
            }

            foreach (var image in changes.updated)
            {
                Record(image);
            }

            foreach (var removed in changes.removed)
            {
                Record(removed.Value, false);
            }
        }

        void Record(ARTrackedImage image, bool present = true)
        {
            if (image == null || !MarkerSnapshot.TryParse(image.referenceImage.name, out var id))
            {
                return;
            }

            var tracking = present && image.trackingState == TrackingState.Tracking;
            snapshot.Set(id, tracking, new Pose(image.transform.position, image.transform.rotation));
        }
    }
}
