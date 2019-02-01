using System;
 
using UnityEngine;
 
// ---------------
//  String => Int
// ---------------
[Serializable]
public class StringIntDictionary : SerializableDictionary<string, int> {}

// ---------------
//  Int => String
// ---------------
[Serializable]
public class StringStringDictionary : SerializableDictionary<string, string> {}
 
// ---------------
//  GameObject => Float
// ---------------
[Serializable]
public class GameObjectFloatDictionary : SerializableDictionary<GameObject, float> {}
