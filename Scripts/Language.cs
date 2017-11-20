using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Language : ScriptableObject
{
    //标签
    public List<Language_Sheet> Localization;
}

[System.Serializable]
public class Language_Sheet
{
    //标签页内的字段
    public string TextID;
    public string ZH;
    public string EN;
}

enum LanguageKind
{
    zh = 2,
    en = 3
}

public enum LanguageChange
{
    left,
    right,
    init    
}

public class LocalizationEx
{
    // Currently selected language
    static int mLanguage = 0;

    public static string LoadLanguageTextName(string textName)
    {        
        foreach (Language_Sheet text in DataManager.Language_Localization)
        {
            if (textName == text.TextID)
            {
                //根据当前语言选择使用那个字段
                switch (mLanguage)
                {
                    case (int)LanguageKind.zh:
                        return text.ZH;
                    case (int)LanguageKind.en:
                        return text.EN;
                }
                break;
            }
        }
        
        return "";
    }
    
    public static void LoadLanguage()
    {
        mLanguage = PlayerPrefs.GetInt("Language");
    }

    //设置当前语言 setup current language
    public static void SaveLanguage(LanguageChange lc)
    {
        switch (lc)
        {
            case LanguageChange.left:
                mLanguage -= 1;
                if (mLanguage < (int)LanguageKind.zh)
                {
                    mLanguage = (int)LanguageKind.en;
                }
                break;
            case LanguageChange.right:
                mLanguage += 1;
                if (mLanguage > (int)LanguageKind.en)
                {
                    mLanguage = (int)LanguageKind.zh;
                }
                break;
            case LanguageChange.init:
                mLanguage = (int)GetDefaultLanguage(Application.systemLanguage);
                break;
        }

        //存档
        PlayerPrefs.SetInt("Language", mLanguage);

        //return mLanguage;
    }
    
    //根据系统语言决定用什么语言去翻译 translate accoring to system language, english as default
    static LanguageKind GetDefaultLanguage(SystemLanguage language)
    {
        switch (language)
        {
            case SystemLanguage.Afrikaans:
            case SystemLanguage.Arabic:
            case SystemLanguage.Basque:
            case SystemLanguage.Belarusian:
            case SystemLanguage.Bulgarian:
            case SystemLanguage.Catalan:
                return LanguageKind.en;
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseTraditional:
            case SystemLanguage.ChineseSimplified:
                return LanguageKind.zh;
            case SystemLanguage.Czech:
            case SystemLanguage.Danish:
            case SystemLanguage.Dutch:
            case SystemLanguage.English:
            case SystemLanguage.Estonian:
            case SystemLanguage.Faroese:
            case SystemLanguage.Finnish:
                return LanguageKind.en;
            case SystemLanguage.French:
                //return LANGUAGE_FRENCH;
                return LanguageKind.en;
            case SystemLanguage.German:
                //return LANGUAGE_GERMAN;
                return LanguageKind.en;
            case SystemLanguage.Greek:
            case SystemLanguage.Hebrew:
            case SystemLanguage.Icelandic:
            case SystemLanguage.Indonesian:
                return LanguageKind.en;
            case SystemLanguage.Italian:
                //return LANGUAGE_ITALY;
                return LanguageKind.en;
            case SystemLanguage.Japanese:
                //return LANGUAGE_JAPANESE;
                return LanguageKind.en;
            case SystemLanguage.Korean:
                //return LANGUAGE_KOREA;
                return LanguageKind.en;
            case SystemLanguage.Latvian:
            case SystemLanguage.Lithuanian:
            case SystemLanguage.Norwegian:
            case SystemLanguage.Polish:
            case SystemLanguage.Portuguese:
            case SystemLanguage.Romanian:
                return LanguageKind.en;
            case SystemLanguage.Russian:
                //return LANGUAGE_RUSSIA;
                return LanguageKind.en;
            case SystemLanguage.SerboCroatian:
            case SystemLanguage.Slovak:
            case SystemLanguage.Slovenian:
                return LanguageKind.en;
            case SystemLanguage.Spanish:
                //return LANGUAGE_SPANISH;
                return LanguageKind.en;
            case SystemLanguage.Swedish:
            case SystemLanguage.Thai:
            case SystemLanguage.Turkish:
            case SystemLanguage.Ukrainian:
            case SystemLanguage.Vietnamese:
            case SystemLanguage.Unknown:
                return LanguageKind.en;
        }

        return LanguageKind.en;
    }
}
