using System;
using UnityEngine;

public class SaveManager : Singelton<SaveManager>
{
    #region SAVING

    public void SaveValue(string valueName, int value)
    {
        PlayerPrefs.SetInt(valueName, value);
        PlayerPrefs.Save();
    }

    public void SaveValue(string valueName, float value)
    {
        PlayerPrefs.SetFloat(valueName, value);
        PlayerPrefs.Save();
    }

    public void SaveValue(string valueName, bool value)
    {
        PlayerPrefs.SetInt(valueName, Convert.ToInt32(value));
        PlayerPrefs.Save();
    }

    #endregion SAVING

    #region LOADING

    public int LoadInt(string valueName)
    {
        return PlayerPrefs.GetInt(valueName);
    }

    public float LoadFloat(string valueName)
    {
        return PlayerPrefs.GetFloat(valueName);
    }

    public bool LoadBool(string valueName, bool defaultValue = false)
    {
        return Convert.ToBoolean(PlayerPrefs.GetInt(valueName, Convert.ToInt32(defaultValue)));
    }

    #endregion LOADING

    #region DELETING

    public void DeleteKey(string keyName)
    {
        PlayerPrefs.DeleteKey(keyName);
    }

    public void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }

    #endregion DELETING
}
