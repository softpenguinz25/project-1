using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class StringStringDictionary : SerializableDictionary<string, string> {}

[Serializable]
public class ObjectColorDictionary : SerializableDictionary<UnityEngine.Object, Color> {}

[Serializable]
public class ColorArrayStorage : SerializableDictionary.Storage<Color[]> {}

[Serializable]
public class StringColorArrayDictionary : SerializableDictionary<string, Color[], ColorArrayStorage> {}

[Serializable]
public class TileTypeGameObjectDictionary : SerializableDictionary<TileV2.TileType, GameObject> { }

[Serializable]
public class TileTypeSpawnChanceDictionary : SerializableDictionary<TileV2.TileType, TileSpawnChance> { }

[Serializable]
public class TileSpawnChanceStorage : SerializableDictionary.Storage<TileSpawnChance[]> { }

[Serializable]
public class TileTypeSpawnChanceArrayDictionary : SerializableDictionary<TileV2.TileType, TileSpawnChance[], TileSpawnChanceStorage> { }

[Serializable]
public class TileTypeIntDictionary : SerializableDictionary<TileV2.TileType, int> { }

[Serializable]
public class MyClass
{
    public int i;
    public string str;
}

[Serializable]
public class QuaternionMyClassDictionary : SerializableDictionary<Quaternion, MyClass> {}