using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace FirstResponse
{
    public interface ISessionStorage
    {
        void Save(SessionRecord record);

        IReadOnlyList<SessionRecord> LoadRecent(int count);

        void DeleteAll();
    }

    public class SessionStorage : ISessionStorage
    {
        private static ISessionStorage device;

        private readonly string folder;
        private readonly int keep;

        public static ISessionStorage Device
        {
            get
            {
                if (device == null)
                {
                    device = new SessionStorage(Path.Combine(Application.persistentDataPath, "sessions"));
                }

                return device;
            }
        }

        public SessionStorage(string folder, int keep = 100)
        {
            this.folder = folder;
            this.keep = keep;
            Directory.CreateDirectory(folder);
        }

        public void Save(SessionRecord record)
        {
            string path = Path.Combine(folder, record.sessionId + ".json");
            string temp = path + ".tmp";
            File.WriteAllText(temp, JsonUtility.ToJson(record, true));
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.Move(temp, path);
            Prune();
        }

        public IReadOnlyList<SessionRecord> LoadRecent(int count)
        {
            return LoadAll().Take(count).ToList();
        }

        public void DeleteAll()
        {
            foreach (string file in Directory.GetFiles(folder, "*.json"))
            {
                File.Delete(file);
            }
        }

        private List<SessionRecord> LoadAll()
        {
            List<SessionRecord> records = new List<SessionRecord>();
            foreach (string file in Directory.GetFiles(folder, "*.json"))
            {
                SessionRecord record = Read(file);
                if (record != null)
                {
                    records.Add(record);
                }
            }

            return records.OrderByDescending(r => r.endedAt, StringComparer.Ordinal).ToList();
        }

        private static SessionRecord Read(string file)
        {
            try
            {
                SessionRecord record = JsonUtility.FromJson<SessionRecord>(File.ReadAllText(file));
                return record != null && record.schemaVersion > 0 ? record : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void Prune()
        {
            foreach (SessionRecord old in LoadAll().Skip(keep))
            {
                File.Delete(Path.Combine(folder, old.sessionId + ".json"));
            }
        }
    }
}
