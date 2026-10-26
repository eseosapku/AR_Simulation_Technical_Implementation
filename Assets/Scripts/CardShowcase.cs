using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirstResponse
{
    public class CardShowcase : MonoBehaviour
    {
        private const float FlySeconds = 0.8f;
        private const float HoverMetres = 0.06f;

        [SerializeField] private ARTrackingManager tracking;
        [SerializeField] private DemoStage stage;
        [SerializeField] private GameObject itemsModel;
        [SerializeField] private Material paintTemplate;

        private readonly Dictionary<string, Transform> items = new Dictionary<string, Transform>();
        private readonly HashSet<string> shown = new HashSet<string>();
        private string flying = "";

        private void Awake()
        {
            GameObject root = Instantiate(itemsModel, transform);
            ModelPaint.Apply(root, paintTemplate);
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                if (child.name.StartsWith("Item"))
                {
                    items[child.name] = child;
                    child.gameObject.SetActive(false);
                }
            }
        }

        private void OnEnable()
        {
            EventBus<CardAccepted>.Subscribe(OnAccepted);
        }

        private void OnDisable()
        {
            EventBus<CardAccepted>.Unsubscribe(OnAccepted);
        }

        public static string ItemFor(MarkerId card, bool thigh)
        {
            switch (card)
            {
                case MarkerId.Wound:
                    return "ItemGauze";
                case MarkerId.Tourniquet:
                    return thigh ? "ItemCloth" : "ItemBandage";
                case MarkerId.Windlass:
                    return "ItemStick";
                default:
                    return "ItemPen";
            }
        }

        private void Update()
        {
            MarkerSnapshot markers = tracking.Snapshot;
            shown.Clear();
            for (int i = 0; i < MarkerSnapshot.Count; i++)
            {
                MarkerId card = (MarkerId)i;
                string itemName = ItemFor(card, stage.IsThigh);
                if (!markers.IsVisible(card) || itemName == flying || !items.TryGetValue(itemName, out Transform item))
                {
                    continue;
                }

                shown.Add(itemName);
                Quaternion spin = Quaternion.AngleAxis(Time.time * 60f, markers.Rotation(card) * Vector3.up);
                item.SetPositionAndRotation(markers.Position(card) + markers.Rotation(card) * Vector3.up * HoverMetres, spin * markers.Rotation(card));
            }

            foreach (KeyValuePair<string, Transform> pair in items)
            {
                if (pair.Key != flying)
                {
                    pair.Value.gameObject.SetActive(shown.Contains(pair.Key));
                }
            }
        }

        private void OnAccepted(CardAccepted message)
        {
            if (items.TryGetValue(ItemFor(message.Card, stage.IsThigh), out Transform item) && stage.Focus != null)
            {
                StartCoroutine(Fly(item, message.Clip));
            }
            else
            {
                EventBus<DemoPlayed>.Publish(new DemoPlayed(message.Clip));
            }
        }

        private IEnumerator Fly(Transform item, string clip)
        {
            flying = item.name;
            item.gameObject.SetActive(true);
            Vector3 from = item.position;
            for (float t = 0f; t < FlySeconds; t += Time.deltaTime)
            {
                float eased = Mathf.SmoothStep(0f, 1f, t / FlySeconds);
                item.position = Vector3.Lerp(from, stage.Focus.position, eased) + Vector3.up * Mathf.Sin(eased * Mathf.PI) * 0.25f;
                yield return null;
            }

            item.gameObject.SetActive(false);
            flying = "";
            EventBus<DemoPlayed>.Publish(new DemoPlayed(clip));
        }
    }
}
