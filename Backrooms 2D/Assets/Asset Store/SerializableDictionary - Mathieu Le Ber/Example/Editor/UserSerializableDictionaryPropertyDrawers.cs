﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(StringStringDictionary))]
[CustomPropertyDrawer(typeof(ObjectColorDictionary))]
[CustomPropertyDrawer(typeof(StringColorArrayDictionary))]
[CustomPropertyDrawer(typeof(TileTypeSpawnChanceArrayDictionary))]
[CustomPropertyDrawer(typeof(TileTypeGameObjectDictionary))]
[CustomPropertyDrawer(typeof(TileTypeSpawnChanceDictionary))]
[CustomPropertyDrawer(typeof(TileTypeIntDictionary))]
[CustomPropertyDrawer(typeof(CodeValueCodeGODictionary))]
[CustomPropertyDrawer(typeof(IntGameObjectDictionary))]
public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer {}

[CustomPropertyDrawer(typeof(ColorArrayStorage))]
public class AnySerializableDictionaryStoragePropertyDrawer: SerializableDictionaryStoragePropertyDrawer {}
