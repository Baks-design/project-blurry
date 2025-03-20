using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Persistence
{
    public class FileDataService : IDataService
    {
        readonly string dataPath, fileExtension;
        readonly ISerializer serializer;

        public FileDataService(ISerializer serializer)
        {
            dataPath = Application.persistentDataPath;
            fileExtension = "json";
            this.serializer = serializer;
        }

        string GetPathToFile(string fileName) => Path.Combine(dataPath, string.Concat(fileName, ".", fileExtension));

        public void Save(GameData data, bool overwrite = true)
        {
            var fileLocation = GetPathToFile(data.Name);

            if (!overwrite && File.Exists(fileLocation))
                throw new IOException($"The file '{data.Name}.{fileExtension}' already exists and cannot be overwritten.");

            File.WriteAllText(fileLocation, serializer.Serialize(data));
        }

        public GameData Load(string name)
        {
            var fileLocation = GetPathToFile(name);

            if (!File.Exists(fileLocation))
                throw new ArgumentException($"No persisted GameData with name '{name}'");

            return serializer.Deserialize<GameData>(File.ReadAllText(fileLocation));
        }

        public void Delete(string name)
        {
            var fileLocation = GetPathToFile(name);

            if (File.Exists(fileLocation))
                File.Delete(fileLocation);
        }

        public void DeleteAll()
        {
            foreach (var filePath in Directory.GetFiles(dataPath))
                File.Delete(filePath);
        }

        public IEnumerable<string> ListSaves()
        {
            foreach (var path in Directory.EnumerateFiles(dataPath))
                if (Path.GetExtension(path) == fileExtension)
                    yield return Path.GetFileNameWithoutExtension(path);
        }
    }
}