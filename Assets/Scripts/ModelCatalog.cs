using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FirstResponse
{
    public enum HazardKind
    {
        None,
        Traffic,
        FuelLeak
    }

    [CreateAssetMenu(fileName = "ModelCatalog", menuName = "First Response/Model Catalog")]
    public class ModelCatalog : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public string label;
            public AssetReferenceGameObject model;
            public Vector3 position;
            public Vector3 rotation;
            [Min(0.05f)] public float size = 1f;
            public string animation;
            public HazardKind hazard;
        }

        [SerializeField] private List<Entry> entries = new List<Entry>();

        public IReadOnlyList<Entry> Entries => entries;
    }
}
