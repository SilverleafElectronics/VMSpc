using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace VMSpc.JsonFileManagers
{
    public interface IJsonContents { }
    public abstract class JsonFileReader<T>
        where T : IJsonContents
    {
        private string filepath;
        public T Contents;
        protected static readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        protected static readonly string cwd = Directory.GetCurrentDirectory();
        public JsonFileReader(string filepath)
        {
            this.filepath = filepath;
            if (File.Exists(filepath))
            {
                LoadJson(filepath);
            }
            else
            {
                SaveDefault();
            }
        }

        public virtual void LoadJson(string filepath)
        {
            string rawJson;
            using (StreamReader reader = new StreamReader(filepath))
            {
                rawJson = reader.ReadToEnd();
            }
            Contents = JsonConvert.DeserializeObject<T>(rawJson, serializerSettings);
        }

        public void SaveConfiguration()
        {
            SaveJson();
        }

        public void SaveJson()
        {
            using (StreamWriter sw = new StreamWriter(filepath))
            {
                string json = JsonConvert.SerializeObject(Contents, Formatting.Indented, serializerSettings);
                sw.Write(json);
            }
        }

        public void SaveDefault()
        {
            Contents = GetDefaultContents();
            SaveJson();
        }

        protected abstract T GetDefaultContents();
    }
}
