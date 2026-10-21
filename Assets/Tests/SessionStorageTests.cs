using System;
using System.IO;
using NUnit.Framework;

namespace FirstResponse.Tests
{
    public class SessionStorageTests
    {
        string folder;
        SessionStorage storage;

        static SessionRecord Record(string ended, float score)
        {
            return new SessionRecord { scenarioId = "Thigh", outcome = "Passed", endedAt = ended, scorePercent = score };
        }

        [SetUp]
        public void Create()
        {
            folder = Path.Combine(Path.GetTempPath(), "FirstResponseTests", Guid.NewGuid().ToString());
            storage = new SessionStorage(folder, 3);
        }

        [TearDown]
        public void Destroy()
        {
            Directory.Delete(folder, true);
        }

        [Test]
        public void SavedSessionLoadsBackIdentically()
        {
            var record = Record("2026-10-20T10:00:00Z", 86f);
            record.steps.Add(new StepResult { stepId = "DirectPressure", completed = true, score = 0.8f });

            storage.Save(record);
            var loaded = storage.LoadRecent(5)[0];

            Assert.AreEqual(record.sessionId, loaded.sessionId);
            Assert.AreEqual(86f, loaded.scorePercent);
            Assert.AreEqual("DirectPressure", loaded.steps[0].stepId);
            Assert.IsTrue(loaded.Passed);
        }

        [Test]
        public void NewestSessionComesFirst()
        {
            storage.Save(Record("2026-10-18T10:00:00Z", 50f));
            storage.Save(Record("2026-10-20T10:00:00Z", 90f));
            storage.Save(Record("2026-10-19T10:00:00Z", 70f));

            var recent = storage.LoadRecent(5);

            Assert.AreEqual(90f, recent[0].scorePercent);
            Assert.AreEqual(50f, recent[2].scorePercent);
        }

        [Test]
        public void OldestSessionsArePrunedPastTheLimit()
        {
            for (var day = 10; day < 15; day++)
            {
                storage.Save(Record($"2026-10-{day}T10:00:00Z", day));
            }

            var recent = storage.LoadRecent(10);

            Assert.AreEqual(3, recent.Count);
            Assert.AreEqual(14f, recent[0].scorePercent);
        }

        [Test]
        public void CorruptFileIsSkipped()
        {
            storage.Save(Record("2026-10-20T10:00:00Z", 80f));
            File.WriteAllText(Path.Combine(folder, "broken.json"), "{ not json");

            Assert.AreEqual(1, storage.LoadRecent(5).Count);
        }

        [Test]
        public void SavingLeavesNoTemporaryFile()
        {
            storage.Save(Record("2026-10-20T10:00:00Z", 80f));

            Assert.IsEmpty(Directory.GetFiles(folder, "*.tmp"));
        }
    }
}
