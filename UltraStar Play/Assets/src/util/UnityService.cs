using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnityService
{
    float GetAxis(string name);
    float GetAxisRaw(string name);
}


public class UnityService : IUnityService
{
    public float GetAxis(string axisName)
    {
        return Input.GetAxis(axisName);
    }
    public float GetAxisRaw(string axisName)
    {
        return Input.GetAxisRaw(axisName);
    }
}
