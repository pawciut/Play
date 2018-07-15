using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SOptionsController : MonoBehaviour
{
    protected virtual float TimeBetweenInputs { get; set; } //in seconds
    protected virtual float DpadSensitivity { get; set; } //Axis range 0-1
    private float timer = 0;

    private void Awake()
    {
        TimeBetweenInputs = 0.35f;
        DpadSensitivity = 0.5f;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (timer == 0)
        {
            if (Input.GetAxis("Horizontal") > DpadSensitivity) //Right
            {
                ToggleRight();
                timer = TimeBetweenInputs;
            }
            else if (Input.GetAxis("Horizontal") < -DpadSensitivity) //Left
            {
                ToggleLeft();
                timer = TimeBetweenInputs;
            }
        }
        if (timer > 0) { timer -= Time.deltaTime; } else { timer = 0; }
    }

    public virtual void ToggleLeft()
    {
        Debug.Log("ToggleLeft");
    }

    public virtual void ToggleRight()
    {
        Debug.Log("ToggleRight");

    }
}
