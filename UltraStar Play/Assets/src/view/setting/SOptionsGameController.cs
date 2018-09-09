using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using UnityEngine.EventSystems;

public class SOptionsGameController : SOptionsController
{
    public Text DebugDisplayValue;//test

    // Use this for initialization
    void Start()
    {
        InitServices();
        UpdateDebugText(SettingsManager.GetSetting<bool>(ESetting.Debug));
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    void UpdateDebugText(bool value)
    {
        DebugDisplayValue.text = SettingUtils.ToYesNo(value);
    }

    public override void HandleSetting(SettingOperationArgs e)
    {
        switch (e.Setting)
        {
            case ESetting.Debug:
                HandleDebug(e);
                break;
            default:
                break;
        }
    }

    void HandleDebug(SettingOperationArgs e)
    {
        bool debug = SettingsManager.GetSetting<bool>(ESetting.Debug);

        debug = !debug;

        SettingsManager.SetSetting(ESetting.Debug, debug);
        UpdateDebugText(debug);
    }
}
