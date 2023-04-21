using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LanguageType
{

}

public class Localization
{
    [SerializeField]
    private static Dictionary<string, string> _dictionary;

    public static string GetString(string value)
        => _dictionary[value];
}
