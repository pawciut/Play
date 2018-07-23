using UnityEngine;
using UnityEngine.EventSystems;

public interface IOptionsService
{
    void SetCurrent(GameObject gameObject);
    OptionButton GetCurrent();
}

public class OptionsService : IOptionsService
{
    public void SetCurrent(GameObject gameObject)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
    public OptionButton GetCurrent()
    {
        return EventSystem.current.currentSelectedGameObject.GetComponent<OptionButton>();
    }
}

