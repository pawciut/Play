using System;
using System.Collections;
using System.Collections.Generic;

public class SettingUtils
{
    public static void ParseResolution(Object resolutionSetting, out int width, out int height)
    {
        string resolutionSettingAsString = resolutionSetting as String;
        if (String.IsNullOrEmpty(resolutionSettingAsString))
            throw new InvalidCastException();
        string[] resolutionDimensions = resolutionSettingAsString.Split('x');
        
        if(resolutionDimensions == null || resolutionDimensions.Length != 2)
            throw new InvalidCastException();
        width = Int32.Parse(resolutionDimensions[0]);
        height = Int32.Parse(resolutionDimensions[1]);
    }

    public static void ParseBool(Object boolSetting, out bool fullScreen)
    {
        if (boolSetting is bool)
            fullScreen = (bool)boolSetting;
        else
        {
            fullScreen = (bool)Convert.ChangeType(boolSetting, typeof(bool));
        }
    }

    public static string ToYesNo(bool settingValue)
    {
        return settingValue ? "Yes" : "No";
    }

    public static string FormatResolution(int width, int height)
    {
        return width + "x" + height;
    }
}
