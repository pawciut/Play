using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnityService
{
    float GetAxis(string name);
}


public class UnityService : IUnityService
{
    public float GetAxis(string axisName)
    {
        return Input.GetAxis(axisName);
    }
}
