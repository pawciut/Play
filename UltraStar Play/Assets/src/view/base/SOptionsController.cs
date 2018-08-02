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
        TimeBetweenInputs = 0.20f;
        DpadSensitivity = 0.01f;//with adjustments to GetAxisRaw * Time.deltaTime, previously was 0.5f but keyboard was lagging because of this without AxisRaw;
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
            if(unityService.GetAxisRaw("Submit") > 0)
            {
                ToggleSubmit();
                timer = TimeBetweenInputs;
            }
            else if (unityService.GetAxisRaw("Horizontal") * Time.deltaTime > DpadSensitivity) //Right
            {
                ToggleRight();
                timer = TimeBetweenInputs;
            }
            else if (unityService.GetAxisRaw("Horizontal") * Time.deltaTime < -DpadSensitivity) //Left
            {
                ToggleLeft();
                timer = TimeBetweenInputs;
            }
        }
        if (timer > 0) { timer -= Time.deltaTime; } else { timer = 0; }
    }

    public virtual void ToggleSubmit()
    {
        Debug.Log("Submit:" + System.DateTime.Now.ToLongTimeString());
        OnSettingOperation(ESettingOperation.Toggle);
    }

    public virtual void ToggleLeft()
    {
        Debug.Log("ToggleLeft:"+System.DateTime.Now.ToLongTimeString());
        OnSettingOperation(ESettingOperation.ToggleLeft);
    }

    public virtual void ToggleRight()
    {
        Debug.Log("ToggleRight:" + System.DateTime.Now.ToLongTimeString());
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
