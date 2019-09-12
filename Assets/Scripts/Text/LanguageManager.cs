using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public static readonly Dictionary<string, string> Texts = new Dictionary<string, string>();
    public static string CurrentLanguageKey { get; private set; }
    public string defaultLanguageKey = "ENG";
    [Header("Editor")]
    [Tooltip("You can add new language by `Add New Language` context menu")]
    public string newLanguageKey;
    [Header("Language List")]
    public List<Language> languageList = new List<Language>();
    public readonly Dictionary<string, Language> LanguageMap = new Dictionary<string, Language>();
    private void Awake()
    {
        SetupDefaultTexts();
        foreach (var language in languageList)
        {
            LanguageMap[language.languageKey] = language;
        }
        ChangeLanguage(defaultLanguageKey);
    }

    private void SetupDefaultTexts()
    {
        Texts.Clear();
        foreach (var pair in DefaultLocale.Texts)
        {
            Texts.Add(pair.Key, pair.Value);
        }
    }

    private void ChangeLanguage(string languageKey)
    {
        if (!LanguageMap.ContainsKey(languageKey))
            return;

        CurrentLanguageKey = languageKey;
        var languageDataList = LanguageMap[languageKey].dataList;
        foreach (var data in languageDataList)
        {
            if (Texts.ContainsKey(data.key))
                Texts[data.key] = data.value;
        }
    }

    public Language GetLanguageFromList(string languageKey)
    {
        foreach (var language in languageList)
        {
            if (language.languageKey == languageKey)
                return language;
        }
        return null;
    }

    [ContextMenu("Add New Language")]
    public void AddNewLanguage()
    {
        if (string.IsNullOrEmpty(newLanguageKey))
        {
            Debug.LogWarning("`New Language Key` is null or empty");
            return;
        }
        var newLang = GetLanguageFromList(newLanguageKey);
        if (newLang == null)
        {
            newLang = new Language();
            newLang.languageKey = newLanguageKey;
            languageList.Add(newLang);
        }

        foreach (var pair in DefaultLocale.Texts)
        {
            if (newLang.ContainKey(pair.Key))
                continue;

            newLang.dataList.Add(new LanguageData()
            {
                key = pair.Key,
                value = pair.Value,
            });
        }
    }

    public static string GetText(string key)
    {
        if (Texts.ContainsKey(key))
            return Texts[key];
        return "";
    }

    public static string ReplaceFormat(string key, string replacingText)
    {
        return ReplaceFormat("{" + key + "}", Texts[key], replacingText);
    }

    public static string ReplaceFormat(string key, string value, string replacingText)
    {
        return replacingText.Replace("{" + key + "}", value);
    }

    public static string FormatNumber(int value, bool asPercent = false)
    {
        return asPercent ? (value * 100).ToString("N0") + "%" : value.ToString("N0");
    }

    public static string FormatNumber(float value, bool asPercent = false)
    {
        return asPercent ? (value * 100).ToString("N0") + "%" : value.ToString("N0");
    }

    public static string FormatInfo(string key, int value, bool asPercent = false)
    {
        return string.Format(Texts[GameText.FORMAT_INFO], Texts[key], FormatNumber(value, asPercent));
    }

    public static string FormatInfo(string key, float value, bool asPercent = false)
    {
        return string.Format(Texts[GameText.FORMAT_INFO], Texts[key], FormatNumber(value, asPercent));
    }

    public static string FormatAttribute(string key, int value, int bonusValue, bool asPercent = false)
    {
        return string.Format(Texts[GameText.FORMAT_ATTRIBUTE], Texts[key], FormatNumber(value, asPercent), FormatBonus(value, asPercent));
    }

    public static string FormatAttribute(string key, float value, float bonusValue, bool asPercent = false)
    {
        return string.Format(Texts[GameText.FORMAT_ATTRIBUTE], Texts[key], FormatNumber(value, asPercent), FormatBonus(value, asPercent));
    }

    public static string FormatBonus(int value, bool asPercent = false)
    {
        if (value > 0)
            return string.Format(Texts[GameText.FORMAT_BONUS], "+", FormatNumber(value, asPercent));
        else if (value < 0)
            return string.Format(Texts[GameText.FORMAT_BONUS], "-", FormatNumber(value, asPercent));
        else
            return "";
    }

    public static string FormatBonus(float value, bool asPercent = false)
    {
        if (value > 0)
            return string.Format(Texts[GameText.FORMAT_BONUS], "+", FormatNumber(value, asPercent));
        else if (value < 0)
            return string.Format(Texts[GameText.FORMAT_BONUS], "-", FormatNumber(value, asPercent));
        else
            return "";
    }

    public static string ReplaceFormatAsFormatBonus(string key, int value, string replacingText, bool asPercent = false)
    {
        return replacingText.Replace("{" + key + "}", FormatBonus(value, asPercent));
    }

    public static string ReplaceFormatAsFormatBonus(string key, float value, string replacingText, bool asPercent = false)
    {
        return replacingText.Replace("{" + key + "}", FormatBonus(value, asPercent));
    }
}
