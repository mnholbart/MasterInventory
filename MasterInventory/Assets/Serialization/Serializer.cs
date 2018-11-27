using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Serializer : ScriptableObject
{

    public abstract void Serialize(string key, object data);
    public abstract T Deserialize<T>(string key, T target, out bool success);
    public abstract string SerializeWithoutKey(object data);
    public abstract T DeserializeWithoutKey<T>(string data, T target);
}
