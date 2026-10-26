using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace FirstResponse
{
    public class ARTrackingManager : MonoBehaviour
    {
        [SerializeField] private ARTrackedImageManager imageManager;

        private readonly MarkerSnapshot snapshot = new MarkerSnapshot();

        public MarkerSnapshot Snapshot => snapshot;

        private void OnEnable()
        {
            imageManager.trackablesChanged.AddListener(OnChanged);
        }

        private void OnDisable()
        {
            imageManager.trackablesChanged.RemoveListener(OnChanged);
        }

        private void Update()
        {
            snapshot.ScreenHeld = ScreenTap.IsHeld();
        }

        private void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> changes)
        {
            foreach (ARTrackedImage image in changes.added)
            {
                Record(image);
            }

            foreach (ARTrackedImage image in changes.updated)
            {
                Record(image);
            }

            foreach (KeyValuePair<TrackableId, ARTrackedImage> removed in changes.removed)
            {
                Record(removed.Value, false);
            }
        }

        private void Record(ARTrackedImage image, bool present = true)
        {
            if (image == null || !MarkerSnapshot.TryParse(image.referenceImage.name, out MarkerId id))
            {
                return;
            }

            bool tracking = present && image.trackingState == TrackingState.Tracking;
            snapshot.Set(id, tracking, new Pose(image.transform.position, image.transform.rotation));
        }
    }
}
