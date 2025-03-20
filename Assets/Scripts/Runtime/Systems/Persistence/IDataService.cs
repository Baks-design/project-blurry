using System.Collections.Generic;

namespace Assets.Scripts.Runtime.Systems.Persistence
{
    public interface IDataService
    {
        IEnumerable<string> ListSaves();
        
        void Save(GameData data, bool overwrite = true);
        GameData Load(string name);
        void Delete(string name);
        void DeleteAll();
    }
}