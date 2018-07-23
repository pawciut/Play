using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SOptionsController : MonoBehaviour
{
    protected virtual float TimeBetweenInputs { get; set; } //in seconds
    protected virtual float DpadSensitivity { get; set; } //Axis range 0-1
    private float timer = 0;

    public IUnityService unityService;
    public IOptionsService optionsService;
    
    private void Awake()
    {
        TimeBetweenInputs = 0.35f;
        DpadSensitivity = 0.5f;
    }

    void Start()
    {
        InitServices();
    }

    public void InitServices()
    {
        if (unityService == null)
            unityService = new UnityService();
        if (optionsService == null)
            optionsService = new OptionsService();
    }

    // Update is called once per frame
    protected virtual void Update()
    {

        if (timer == 0)
        {
            if(unityService.GetAxis("Submit") > 0)
            {
                ToggleSubmit();
                timer = TimeBetweenInputs;
            }
            else if (unityService.GetAxis("Horizontal") > DpadSensitivity) //Right
            {
                ToggleRight();
                timer = TimeBetweenInputs;
            }
            else if (unityService.GetAxis("Horizontal") < -DpadSensitivity) //Left
            {
                ToggleLeft();
                timer = TimeBetweenInputs;
            }
        }
        if (timer > 0) { timer -= Time.deltaTime; } else { timer = 0; }
    }

    public virtual void ToggleSubmit()
    {
        Debug.Log("Submit:");
        OnSettingOperation(ESettingOperation.Toggle);
    }

    public virtual void ToggleLeft()
    {
        Debug.Log("ToggleLeft");
        OnSettingOperation(ESettingOperation.ToggleLeft);
    }

    public virtual void ToggleRight()
    {
        Debug.Log("ToggleRight");
        OnSettingOperation(ESettingOperation.ToggleRight);
    }

    public virtual void HandleSetting(SettingOperationArgs e)
    {
    }

    void OnSettingOperation(ESettingOperation operation)
    {
        OptionButton button = optionsService.GetCurrent();
        if (button != null)
            HandleSetting(new SettingOperationArgs(button.Setting, operation));
    }
}
