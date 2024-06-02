using System;
using System.Collections.Generic;

namespace BouncingBall;

public static class Util {
    public static string AddSpaces(string str) {
        string result = "";
        foreach (char c in str) {
            if (char.IsUpper(c)) {
                result += " ";
            }
            result += c;
        }
        return result;
    }

    public static Dictionary<K, V> CreateEnumDict<K, V>(Func<K, V> populator) where K : Enum {
        Dictionary<K, V> result = [];
        foreach (K key in Enum.GetValues(typeof(K))) {
            result.Add(key, populator.Invoke(key));
        }
        return result;
    }
}