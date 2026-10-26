using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace FirstResponse.Tests
{
    public class ScenarioRulesTests
    {
        private ScenarioConfig config;

        private StepContext Context()
        {
            return new StepContext(config, config.CreateBleeding(), new MarkerSnapshot());
        }

        [SetUp]
        public void Create()
        {
            config = ScriptableObject.CreateInstance<ScenarioConfig>();
        }

        [TearDown]
        public void Destroy()
        {
            Object.DestroyImmediate(config);
        }

        [Test]
        public void ForearmBandagesAfterPressure()
        {
            config.pressureIsEnough = true;
            List<ProcedureStep> steps = ScenarioFactory.Build(config);

            Assert.AreEqual(9, steps.Count);
            Assert.AreEqual(MarkerId.Wound, ((CardStep)steps[5]).Card);
            Assert.AreEqual("Bandage", ((CardStep)steps[6]).Clip);
        }

        [Test]
        public void ThighBuildsTheTourniquetInOrder()
        {
            config.pressureIsEnough = false;
            List<string> clips = new List<string>();
            foreach (ProcedureStep step in ScenarioFactory.Build(config))
            {
                if (step is CardStep card)
                {
                    clips.Add(card.Clip);
                }
                else if (step is DemoStep demo)
                {
                    clips.Add(demo.Clip);
                }
            }

            CollectionAssert.AreEqual(new[] { "Pressure", "Cloth", "Wrap", "Knot", "Stick", "Twist", "Lock", "Mark" }, clips);
        }

        [Test]
        public void WireTourniquetIsACriticalError()
        {
            config.pressureIsEnough = false;
            StepContext context = Context();
            ChoiceStep improvise = (ChoiceStep)ScenarioFactory.Build(config)[6];
            improvise.Enter(context);
            improvise.Choose(1);

            Assert.AreEqual(1, context.CriticalErrors.Count);
        }

        [Test]
        public void TourniquetOnAForearmIsACriticalError()
        {
            config.pressureIsEnough = true;
            StepContext context = Context();
            ChoiceStep intervention = (ChoiceStep)ScenarioFactory.Build(config)[4];
            intervention.Enter(context);
            intervention.Choose(1);

            Assert.AreEqual(1, context.CriticalErrors.Count);
        }

        [Test]
        public void ScoreIsTheAverageOfAllSteps()
        {
            List<StepResult> results = new List<StepResult>
            {
                new StepResult { score = 1f },
                new StepResult { score = 0.5f }
            };

            Assert.AreEqual(50f, Scoring.Percent(results, 3), 0.01f);
        }

        [Test]
        public void CriticalErrorOrLostCasualtyFails()
        {
            Assert.AreEqual("Passed", Scoring.Outcome(90f, 0, false, 80f));
            Assert.AreEqual("Failed", Scoring.Outcome(95f, 1, false, 80f));
            Assert.AreEqual("Failed", Scoring.Outcome(95f, 0, true, 80f));
            Assert.AreEqual("Failed", Scoring.Outcome(70f, 0, false, 80f));
        }

        [Test]
        public void DebriefCountsEveryMistake()
        {
            SessionRecord record = new SessionRecord();
            record.criticalErrors.Add("ChooseIntervention: Tourniquet");
            record.steps.Add(new StepResult { events = new List<string> { "wrong_Bare hands", "early_release", "distance_6.1cm" } });

            Assert.AreEqual(3, CompletionScreen.ErrorCount(record));
        }

        [Test]
        public void DebriefDescribesTheResult()
        {
            SessionRecord lost = new SessionRecord { outcome = "Failed" };
            SessionRecord saved = new SessionRecord { outcome = "Passed", timeToControlSec = 74f };

            Assert.AreEqual("The casualty was lost", CompletionScreen.Headline(lost));
            Assert.AreEqual("Bleeding controlled", CompletionScreen.Headline(saved));
            Assert.AreEqual("74 s", CompletionScreen.TimeText(74.2f));
            Assert.AreEqual("Not controlled", CompletionScreen.TimeText(0f));
        }

        [Test]
        public void TipPointsAtTheBiggestMistake()
        {
            SessionRecord record = new SessionRecord();
            record.steps.Add(new StepResult { events = new List<string> { "early_release" } });

            StringAssert.Contains("Hold pressure", Scoring.Tip(record));
            record.criticalErrors.Add("ChooseIntervention: Tourniquet");
            StringAssert.Contains("tourniquet", Scoring.Tip(record));
        }
    }
}
