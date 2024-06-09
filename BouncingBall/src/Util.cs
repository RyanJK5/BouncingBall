using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

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

    public static Color ColorFromHSV(int h, float s, float v) {
        var rgb = new int[3];

        var baseColor = (h + 60) % 360 / 120;
        var shift = (h + 60) % 360 - (120 * baseColor + 60 );
        var secondaryColor = (baseColor + (shift >= 0 ? 1 : -1) + 3) % 3;
        
        rgb[baseColor] = 255;
        rgb[secondaryColor] = (int) (MathF.Abs(shift) / 60.0f * 255.0f);
        
        for (var i = 0; i < 3; i++) {
            rgb[i] += (int) ((255 - rgb[i]) * (1 - s));
        }
        for (var i = 0; i < 3; i++) {
            rgb[i] -= (int) (rgb[i] * (1 - v));
        }

        return new Color(rgb[0], rgb[1], rgb[2]);
    }

    public static float Clamp(float min, float max, float value) =>  MathF.Max(min, MathF.Min(max, value));
}