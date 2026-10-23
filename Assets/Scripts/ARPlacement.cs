using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace FirstResponse
{
    public class ARPlacement : MonoBehaviour
    {
        [SerializeField] private ARRaycastManager raycaster;
        [SerializeField] private ARPlaneManager planes;
        [SerializeField] private Transform reticle;
        [SerializeField] private Transform crashSite;

        private static readonly List<ARRaycastHit> Hits = new List<ARRaycastHit>();
        private readonly PlacementFlow flow = new PlacementFlow();
        private ARAnchor anchor;
        private bool listening;

        private void OnEnable()
        {
            EventBus<ScreenRequested>.Subscribe(OnScreen);
            EventBus<PlacementRequested>.Subscribe(OnRequest);
        }

        private void OnDisable()
        {
            EventBus<ScreenRequested>.Unsubscribe(OnScreen);
            EventBus<PlacementRequested>.Unsubscribe(OnRequest);
        }

        private void Start()
        {
            crashSite.gameObject.SetActive(false);
            reticle.gameObject.SetActive(false);
        }

        private void OnScreen(ScreenRequested message)
        {
            listening = message.Screen == ScreenId.Setup;
            if (listening)
            {
                Announce();
            }
        }

        private void Update()
        {
            if (!listening || flow.State == PlacementState.Confirmed)
            {
                return;
            }

            if (flow.PlanesFound(planes.trackables.count))
            {
                Announce();
            }

            bool aimed = Cast(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f), out Pose centre);
            reticle.gameObject.SetActive(aimed);
            if (aimed)
            {
                reticle.SetPositionAndRotation(centre.position, centre.rotation);
            }

            if (ScreenTap.TryGet(out Vector2 point) && flow.Tapped(Cast(point, out Pose pose)))
            {
                Preview(pose.position);
                Announce();
            }
        }

        private bool Cast(Vector2 point, out Pose pose)
        {
            pose = default;
            if (!raycaster.Raycast(point, Hits, TrackableType.PlaneWithinPolygon))
            {
                return false;
            }

            pose = Hits[0].pose;
            return true;
        }

        private void Preview(Vector3 position)
        {
            Vector3 toCamera = Camera.main.transform.position - position;
            toCamera.y = 0f;
            Quaternion facing = toCamera.sqrMagnitude > 0.001f ? Quaternion.LookRotation(toCamera) : Quaternion.identity;
            crashSite.SetParent(null);
            crashSite.SetPositionAndRotation(position, facing);
            crashSite.gameObject.SetActive(true);
        }

        private void OnRequest(PlacementRequested message)
        {
            if (message.Action == PlacementAction.Confirm && flow.Confirm())
            {
                Lock();
            }
            else if (message.Action == PlacementAction.Move && flow.Move())
            {
                Unlock();
            }

            Announce();
        }

        private void Lock()
        {
            GameObject holder = new GameObject("CrashSiteAnchor");
            holder.transform.SetPositionAndRotation(crashSite.position, crashSite.rotation);
            anchor = holder.AddComponent<ARAnchor>();
            crashSite.SetParent(anchor.transform, true);
            reticle.gameObject.SetActive(false);
            ShowPlanes(false);
        }

        private void Unlock()
        {
            crashSite.SetParent(null, true);
            if (anchor != null)
            {
                Destroy(anchor.gameObject);
            }

            anchor = null;
            ShowPlanes(true);
        }

        private void ShowPlanes(bool visible)
        {
            planes.enabled = visible;
            foreach (ARPlane plane in planes.trackables)
            {
                plane.gameObject.SetActive(visible);
            }
        }

        private void Announce()
        {
            EventBus<PlacementChanged>.Publish(new PlacementChanged(flow.State));
        }
    }
}
