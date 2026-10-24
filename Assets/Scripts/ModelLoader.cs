using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace FirstResponse
{
    public class ModelLoader : MonoBehaviour
    {
        [SerializeField] private ModelCatalog catalog;
        [SerializeField] private Transform crashSite;
        [SerializeField] private GameObject placeholder;

        private readonly List<AsyncOperationHandle<GameObject>> handles = new List<AsyncOperationHandle<GameObject>>();

        public bool IsLoaded { get; private set; }

        public int LoadedCount => handles.Count;

        private void OnEnable()
        {
            EventBus<PlacementChanged>.Subscribe(OnPlacement);
            EventBus<ScreenRequested>.Subscribe(OnScreen);
        }

        private void OnDisable()
        {
            EventBus<PlacementChanged>.Unsubscribe(OnPlacement);
            EventBus<ScreenRequested>.Unsubscribe(OnScreen);
        }

        private void OnDestroy()
        {
            Release();
        }

        private void OnPlacement(PlacementChanged message)
        {
            if (message.State == PlacementState.Previewing && !IsLoaded)
            {
                Load();
            }
        }

        private void OnScreen(ScreenRequested message)
        {
            if (message.Screen == ScreenId.Start && IsLoaded)
            {
                Release();
            }
        }

        public void Load()
        {
            IsLoaded = true;
            if (placeholder != null)
            {
                placeholder.SetActive(false);
            }

            foreach (ModelCatalog.Entry entry in catalog.Entries)
            {
                ModelCatalog.Entry current = entry;
                AsyncOperationHandle<GameObject> handle = current.model.InstantiateAsync(crashSite);
                handles.Add(handle);
                handle.Completed += operation => Arrange(operation.Result, current);
            }
        }

        public void Release()
        {
            foreach (AsyncOperationHandle<GameObject> handle in handles)
            {
                if (handle.IsValid())
                {
                    Addressables.ReleaseInstance(handle);
                }
            }

            handles.Clear();
            IsLoaded = false;
            if (placeholder != null)
            {
                placeholder.SetActive(true);
            }
        }

        private void Arrange(GameObject model, ModelCatalog.Entry entry)
        {
            if (model == null)
            {
                Debug.LogWarning("Could not load " + entry.label);
                return;
            }

            model.name = entry.label;
            Transform body = model.transform;
            body.localPosition = entry.position;
            body.localRotation = Quaternion.Euler(entry.rotation);
            bool animated = ModelPose.PlayAnimation(model, entry.animation);
            if (animated)
            {
                body.localRotation = Quaternion.Euler(0f, entry.rotation.y, 0f);
            }

            ModelPose.Fit(body, entry.size, crashSite);
            AddHazard(model, entry.hazard);
        }

        private static void AddHazard(GameObject model, HazardKind kind)
        {
            if (kind == HazardKind.None)
            {
                return;
            }

            ModelPose.AddCollider(model);
            if (kind == HazardKind.Traffic)
            {
                model.AddComponent<TrafficHazard>();
            }
            else
            {
                model.AddComponent<FuelLeakHazard>();
            }
        }
    }
}
