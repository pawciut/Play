﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using UnityEngine.EventSystems;

public class SOptionsGraphicsController : SOptionsController
{
    public Text FullscreenDisplayValue;

    // Use this for initialization
    void Start()
    {
        InitServices();
        AdjustGraphic();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// Set current settings and view to application state (when resolution dialog is displayed before game starts)
    /// </summary>
    void AdjustGraphic()
    {
        SettingsManager.SetSetting(ESetting.FullScreen, Screen.fullScreen);
        SettingsManager.SetSetting(ESetting.Resolution,
            SettingUtils.FormatResolution(Screen.width, Screen.height));
        FullscreenDisplayValue.text = SettingUtils.ToYesNo(Screen.fullScreen);//SettingUtils.ToYesNo(SettingsManager.GetSetting<bool>(ESetting.FullScreen));
    }

    public override void HandleSetting(SettingOperationArgs e)
    {
        switch (e.Setting)
        {
            case ESetting.FullScreen:
                HandleFullScreen(e);
                break;
            default:
                break;
        }
    }

    void HandleFullScreen(SettingOperationArgs e)
    {
        int width = Screen.currentResolution.width;
        int height = Screen.currentResolution.height;
        System.Object resolutionSetting = SettingsManager.GetSetting(ESetting.Resolution);
        SettingUtils.ParseResolution(resolutionSetting, out width, out height);

        System.Object fullScreenSetting = SettingsManager.GetSetting(ESetting.FullScreen);

        bool fullScreen;
        SettingUtils.ParseBool(fullScreenSetting, out fullScreen);


        fullScreen = !fullScreen;

        SettingsManager.SetSetting(ESetting.FullScreen, fullScreen);
        FullscreenDisplayValue.text = SettingUtils.ToYesNo(fullScreen);
        Screen.SetResolution(width, height, fullScreen);
    }

}
