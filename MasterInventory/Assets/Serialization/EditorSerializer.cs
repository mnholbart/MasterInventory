using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEditor;

[CreateAssetMenu]
[System.Serializable]
public class EditorSerializer : Serializer
{
    private string inventoryDataFileName = "{0}.json";
    private string gameDataProjectFilePath = "/StreamingAssets/{0}.json";

    public override void Serialize(string key, object data)
    {
        string dataAsJson = EditorJsonUtility.ToJson(data);
        string filePath = Application.dataPath + string.Format(gameDataProjectFilePath, key);
        File.WriteAllText(filePath, dataAsJson);        
    }

    public override string SerializeWithoutKey(object data)
    {
        string json = EditorJsonUtility.ToJson(data);
        return json;
    }

    public override T Deserialize<T>(string key, T target, out bool success)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, string.Format(inventoryDataFileName, key));
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            EditorJsonUtility.FromJsonOverwrite(dataAsJson, target);
            success = true;
        }
        else
        {
            success = false;
        }
        
        return target;
    }

    public override T DeserializeWithoutKey<T>(string data, T target)
    {
        EditorJsonUtility.FromJsonOverwrite(data, target);
        return target;
    }
}
