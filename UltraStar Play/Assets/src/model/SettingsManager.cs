﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ISettingsManager
{
    T GetSetting<T>(ESetting key);

    void SetSetting(ESetting key, System.Object settingValue);
}


public static class SettingsManager //: ISettingsManager
{
    private static GameSetting s_setting = new GameSetting();

    public static System.Object GetSetting(ESetting key)
    {
        return GetSetting<System.Object>(key);
        //lock (s_setting)
        //{
        //    return s_setting.GetSettingNotNull(key);
        //}
    }
    public static T GetSetting<T>(ESetting key)
    {
        lock (s_setting)
        {
            return (T)s_setting.GetSettingNotNull(key);
        }
    }

    public static void SetSetting(ESetting key, System.Object settingValue)
    {
        if(settingValue == null)
        {
            throw new UnityException("Can not set setting because value is null!");
        }
        lock (s_setting)
        {
            s_setting.SetSetting(key, settingValue);
        }
    }

    public static void Reload()
    {
        lock (s_setting)
        {
            s_setting = new GameSetting();
        }
    }
}
