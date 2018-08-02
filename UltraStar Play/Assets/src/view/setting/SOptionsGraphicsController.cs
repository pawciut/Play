using System.Collections;
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
    public Text ResolutionDisplayValue;
    public Text FullscreenDisplayValue;

    int? _currentResolutionIndex;
    int[,] _supportedResolutions = new[,]
    {
        //https://en.wikipedia.org/wiki/Computer_display_standard
        //https://freegamedev.net/wiki/Screen_Resolutions
        { 800, 600 },//SVG
        { 1024,768 },//1024x768    4:3     common LCD format
        //1200x900    4:3     OLPC
        { 1280,720 },//1280x720    16:9        HD-TV (720p)
        { 1280,1024 },//1280x1024   5:4     common LCD format
        { 1440,900 },//1440x900    16:10       common LCD format
        //1680x1050   16:10       common LCD format
        { 1600,900 },//1600x900    16:9        common LCD format, often found on 17" laptops these days
        { 1600,1200 },//1600x1200   4:3     common LCD format
        //1366x768    ~16:9       common resolution of HD-TVs, even so its not actually an official HD-TV resolution
        //1368x768    ~16:9       common resolution of HD-TVs, even so its not actually an official HD-TV resolution
        //1920x1200   16:10       common LCD format
        //2560x1600   16:10       30" LCD
        { 1920,1080 },//1920x1080   16:9        HD-TV (1080p)
        //2560x1440   16:9        Apple iMac
        //2560x1600
        };


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
        AdjustResolutionIndex();

        UpdateResolutionText();
        UpdateFullScreenText(Screen.fullScreen);
    }

    void AdjustResolutionIndex()
    {
        for (int rowIndex = 0; rowIndex < _supportedResolutions.GetLength(0); ++rowIndex)
        {
            if (_supportedResolutions[rowIndex, 0] == Screen.width
                && _supportedResolutions[rowIndex, 1] == Screen.height)
            {
                _currentResolutionIndex = rowIndex;
                break;
            }
        }
    }

    void UpdateResolutionText()
    {
        if (_currentResolutionIndex != null)
            ResolutionDisplayValue.text = SettingUtils.FormatResolution(_supportedResolutions[_currentResolutionIndex.Value, 0], _supportedResolutions[_currentResolutionIndex.Value, 1]);
        else
            ResolutionDisplayValue.text = "???";
    }

    void UpdateFullScreenText(bool value)
    {
        FullscreenDisplayValue.text = SettingUtils.ToYesNo(value);//SettingUtils.ToYesNo(SettingsManager.GetSetting<bool>(ESetting.FullScreen));
    }

    public override void HandleSetting(SettingOperationArgs e)
    {
        switch (e.Setting)
        {
            case ESetting.FullScreen:
                HandleFullScreen(e);
                break;
            case ESetting.Resolution:
                HandleResolution(e);
                break;
            default:
                break;
        }
    }

    void HandleFullScreen(SettingOperationArgs e)
    {
        int width = _supportedResolutions[_currentResolutionIndex.Value, 0];//Screen.currentResolution.width;
        int height = _supportedResolutions[_currentResolutionIndex.Value, 1];//Screen.currentResolution.height;

        //System.Object resolutionSetting = SettingsManager.GetSetting(ESetting.Resolution);
        //SettingUtils.ParseResolution(resolutionSetting, out width, out height);

        System.Object fullScreenSetting = SettingsManager.GetSetting(ESetting.FullScreen);

        bool fullScreen;
        SettingUtils.ParseBool(fullScreenSetting, out fullScreen);


        fullScreen = !fullScreen;

        SettingsManager.SetSetting(ESetting.FullScreen, fullScreen);
        FullscreenDisplayValue.text = SettingUtils.ToYesNo(fullScreen);
        Screen.SetResolution(width, height, fullScreen);
    }


    void HandleResolution(SettingOperationArgs e)
    {
        if (e.Operation == ESettingOperation.Toggle)
        {
            System.Object fullScreenSetting = SettingsManager.GetSetting(ESetting.FullScreen);

            bool fullScreen;
            SettingUtils.ParseBool(fullScreenSetting, out fullScreen);

            Screen.SetResolution(
                _supportedResolutions[_currentResolutionIndex.Value, 0],
                _supportedResolutions[_currentResolutionIndex.Value, 1],
                fullScreen);
            return;
        }
        else if (e.Operation == ESettingOperation.ToggleLeft)
        {
            if (_currentResolutionIndex > 0)
                --_currentResolutionIndex;
            else
                _currentResolutionIndex = _supportedResolutions.GetLength(0) - 1;//Loop through resolutions
        }
        else if (e.Operation == ESettingOperation.ToggleRight)
        {
            if (_currentResolutionIndex < _supportedResolutions.GetLength(0) - 1)
                ++_currentResolutionIndex;
            else
                _currentResolutionIndex = 0;//Loop through resolutions
        }

        UpdateResolutionText();
    }

}
