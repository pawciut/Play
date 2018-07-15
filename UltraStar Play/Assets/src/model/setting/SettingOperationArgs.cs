using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SettingOperationArgs : EventArgs
{
    public ESetting Setting { get; private set; }
    public ESettingOperation Operation { get; private set; }
    public Object Parameter { get; private set; }

    public SettingOperationArgs(ESetting setting, ESettingOperation operation, Object parameter = null)
    {
        Setting = setting;
        Operation = operation;
        Parameter = parameter;
    }
}

