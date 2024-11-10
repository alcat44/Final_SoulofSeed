using UnityEngine;

public class TriggerSuaraManager : MonoBehaviour
{
    public TriggerSound[] triggers; // Assign all trigger scripts in the Inspector

    public void DisableAllTriggers()
    {
        foreach (var trigger in triggers)
        {
            trigger.DisableTrigger();
        }
    }
}
