using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffect : MonoBehaviour, IActionEffect
{
    public void ApplyOnTarget(GameObject target)
    {
        Debug.Log("ActionEffect applied to " + target.name, target);
    }

    public bool CanBeUsedOnTarget(GameObject target)
    {
        return true;
    }

    public bool HasBeenAppliedToTarget(GameObject target)
    {
        return false;
    }

    public void PrepareToApplyOnTarget(GameObject target)
    {
        throw new System.NotImplementedException();
    }

    public void RevertEffectFromTarget(GameObject target)
    {
        throw new System.NotImplementedException();
    }
}

public interface IActionEffect
{
    bool CanBeUsedOnTarget(GameObject target);
    void PrepareToApplyOnTarget(GameObject target);
    void ApplyOnTarget(GameObject target);
    void RevertEffectFromTarget(GameObject target);
    bool HasBeenAppliedToTarget(GameObject target);

}