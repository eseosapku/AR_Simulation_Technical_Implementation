using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace FirstResponse
{
    public class ARPlacement : MonoBehaviour
    {
        [SerializeField] ARRaycastManager raycaster;
        [SerializeField] ARPlaneManager planes;
        [SerializeField] Transform reticle;
        [SerializeField] Transform crashSite;

        static readonly List<ARRaycastHit> Hits = new List<ARRaycastHit>();
        readonly PlacementFlow flow = new PlacementFlow();
        ARAnchor anchor;
        bool listening;

        void OnEnable()
        {
            EventBus<ScreenRequested>.Subscribe(OnScreen);
            EventBus<PlacementRequested>.Subscribe(OnRequest);
        }

        void OnDisable()
        {
            EventBus<ScreenRequested>.Unsubscribe(OnScreen);
            EventBus<PlacementRequested>.Unsubscribe(OnRequest);
        }

        void Start()
        {
            crashSite.gameObject.SetActive(false);
            reticle.gameObject.SetActive(false);
        }

        void OnScreen(ScreenRequested message)
        {
            listening = message.Screen == ScreenId.Setup;
            if (listening)
            {
                Announce();
            }
        }

        void Update()
        {
            if (!listening || flow.State == PlacementState.Confirmed)
            {
                return;
            }

            if (flow.PlanesFound(planes.trackables.count))
            {
                Announce();
            }

            var aimed = Cast(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f), out var centre);
            reticle.gameObject.SetActive(aimed);
            if (aimed)
            {
                reticle.SetPositionAndRotation(centre.position, centre.rotation);
            }

            if (ScreenTap.TryGet(out var point) && flow.Tapped(Cast(point, out var pose)))
            {
                Preview(pose.position);
                Announce();
            }
        }

        bool Cast(Vector2 point, out Pose pose)
        {
            pose = default;
            if (!raycaster.Raycast(point, Hits, TrackableType.PlaneWithinPolygon))
            {
                return false;
            }

            pose = Hits[0].pose;
            return true;
        }

        void Preview(Vector3 position)
        {
            var toCamera = Camera.main.transform.position - position;
            toCamera.y = 0f;
            var facing = toCamera.sqrMagnitude > 0.001f ? Quaternion.LookRotation(toCamera) : Quaternion.identity;
            crashSite.SetParent(null);
            crashSite.SetPositionAndRotation(position, facing);
            crashSite.gameObject.SetActive(true);
        }

        void OnRequest(PlacementRequested message)
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

        void Lock()
        {
            var holder = new GameObject("CrashSiteAnchor");
            holder.transform.SetPositionAndRotation(crashSite.position, crashSite.rotation);
            anchor = holder.AddComponent<ARAnchor>();
            crashSite.SetParent(anchor.transform, true);
            reticle.gameObject.SetActive(false);
            ShowPlanes(false);
        }

        void Unlock()
        {
            crashSite.SetParent(null, true);
            if (anchor != null)
            {
                Destroy(anchor.gameObject);
            }

            anchor = null;
            ShowPlanes(true);
        }

        void ShowPlanes(bool visible)
        {
            planes.enabled = visible;
            foreach (var plane in planes.trackables)
            {
                plane.gameObject.SetActive(visible);
            }
        }

        void Announce()
        {
            EventBus<PlacementChanged>.Publish(new PlacementChanged(flow.State));
        }
    }
}
