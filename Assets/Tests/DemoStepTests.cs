using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace FirstResponse.Tests
{
    public class DemoStepTests
    {
        private ScenarioConfig config;
        private StepContext context;
        private readonly List<string> played = new List<string>();

        [SetUp]
        public void Create()
        {
            config = ScriptableObject.CreateInstance<ScenarioConfig>();
            context = new StepContext(config, config.CreateBleeding(), new MarkerSnapshot());
            played.Clear();
            EventBus<DemoPlayed>.Subscribe(m => played.Add(m.Clip));
            EventBus<CardAccepted>.Subscribe(m => played.Add(m.Clip));
        }

        [TearDown]
        public void Destroy()
        {
            Object.DestroyImmediate(config);
            EventBus<DemoPlayed>.Clear();
            EventBus<CardAccepted>.Clear();
        }

        private void Show(MarkerId card, bool visible)
        {
            context.Markers.Set(card, visible, new Pose(Vector3.zero, Quaternion.identity));
        }

        [Test]
        public void HoldingUpTheRightCardPlaysItsStep()
        {
            CardStep step = new CardStep("Stick", "Add the stick", "Show card C", "Rotate", MarkerId.Windlass, "Stick", Treatment.None);
            step.Enter(context);
            Show(MarkerId.Windlass, true);
            step.Tick(context, 0.5f);
            Assert.IsFalse(step.IsComplete);

            step.Tick(context, 0.6f);
            Assert.IsTrue(step.IsComplete);
            CollectionAssert.AreEqual(new[] { "Stick" }, played);
        }

        [Test]
        public void WrongCardIsAMistakeAndDoesNothing()
        {
            CardStep step = new CardStep("Stick", "Add the stick", "Show card C", "Rotate", MarkerId.Windlass, "Stick", Treatment.None);
            step.Enter(context);
            Show(MarkerId.Joint, true);
            step.Tick(context, 2f);

            Assert.IsFalse(step.IsComplete);
            Assert.AreEqual(0, played.Count);
            Assert.AreEqual("wrong_card_Joint", step.Finish().events[0]);
        }

        [Test]
        public void FullThighProcedureControlsTheBleeding()
        {
            config.pressureIsEnough = false;
            context = new StepContext(config, config.CreateBleeding(), new MarkerSnapshot());
            List<ProcedureStep> steps = ScenarioFactory.Build(config);
            for (int i = 5; i <= 13; i++)
            {
                steps[i].Enter(context);
                if (steps[i] is CardStep card)
                {
                    Show(card.Card, true);
                    steps[i].Tick(context, 1.1f);
                    Show(card.Card, false);
                }
                else
                {
                    ChoiceStep choice = (ChoiceStep)steps[i];
                    choice.Choose(choice.Correct);
                }

                Assert.IsTrue(steps[i].IsComplete, steps[i].Id);
            }

            Assert.AreEqual(BleedingState.Controlled, context.Bleeding.State);
            Assert.AreEqual(0, context.CriticalErrors.Count);
        }

        [Test]
        public void TrainerStartsEmptyWithOnlyTheWound()
        {
            GameObject model = new GameObject("TrainerLeg");
            foreach (string part in new[] { "Leg", "Wound", "Pad", "Band", "Stick" })
            {
                new GameObject(part).transform.SetParent(model.transform);
            }

            KitPlayer kit = new KitPlayer();
            kit.Spawn(model, null, null);

            Assert.AreEqual("Wound", kit.Focus.name);
            Assert.IsFalse(kit.IsShowing("Pad"));
            Assert.IsFalse(kit.IsShowing("Band"));
            Assert.IsFalse(kit.IsShowing("Stick"));
            Assert.IsTrue(kit.Root.Find("Leg").gameObject.activeSelf);
            Object.DestroyImmediate(kit.Root.gameObject);
            Object.DestroyImmediate(model);
        }

        [Test]
        public void BandGoesOnLooseAndTwistsTight()
        {
            Vector3 loose = KitMotion.Loose(new Vector3(1f, 0.9f, 0.9f));

            Assert.AreEqual(new Vector3(1f, 1f, 1f), loose);
        }

        [Test]
        public void EachCardShowsItsItem()
        {
            Assert.AreEqual("ItemGauze", CardShowcase.ItemFor(MarkerId.Wound, true));
            Assert.AreEqual("ItemCloth", CardShowcase.ItemFor(MarkerId.Tourniquet, true));
            Assert.AreEqual("ItemBandage", CardShowcase.ItemFor(MarkerId.Tourniquet, false));
            Assert.AreEqual("ItemStick", CardShowcase.ItemFor(MarkerId.Windlass, true));
            Assert.AreEqual("ItemPen", CardShowcase.ItemFor(MarkerId.Joint, true));
        }
    }
}
