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
        readonly string folder;
        readonly int keep;

        public SessionStorage(string folder, int keep = 100)
        {
            this.folder = folder;
            this.keep = keep;
            Directory.CreateDirectory(folder);
        }

        public void Save(SessionRecord record)
        {
            var path = Path.Combine(folder, record.sessionId + ".json");
            var temp = path + ".tmp";
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
            foreach (var file in Directory.GetFiles(folder, "*.json"))
            {
                File.Delete(file);
            }
        }

        List<SessionRecord> LoadAll()
        {
            var records = new List<SessionRecord>();
            foreach (var file in Directory.GetFiles(folder, "*.json"))
            {
                var record = Read(file);
                if (record != null)
                {
                    records.Add(record);
                }
            }

            return records.OrderByDescending(r => r.endedAt, StringComparer.Ordinal).ToList();
        }

        static SessionRecord Read(string file)
        {
            try
            {
                var record = JsonUtility.FromJson<SessionRecord>(File.ReadAllText(file));
                return record != null && record.schemaVersion > 0 ? record : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        void Prune()
        {
            foreach (var old in LoadAll().Skip(keep))
            {
                File.Delete(Path.Combine(folder, old.sessionId + ".json"));
            }
        }
    }
}
