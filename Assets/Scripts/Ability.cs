using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TARGET_TYPE
{
    ALL,
    FOES,
    ALLIES,
    SELF,
    OBJECTS
}

public enum DISTANCE_TYPE
{
    MELEE,
    RANGED,
    MAGICAL
}

public class Ability : MonoBehaviour
{
    public GameObject effectPrefab;

    public TARGET_TYPE targetType;

    public DISTANCE_TYPE distanceType;

    public float MaxUseDistance = 2f;

    GameObject effectInstance;

    SelectableCharacter character;

    public void CreateAimingEffect(GameObject target)
    {
        if (effectPrefab != null)
        {
            if (effectInstance == null)
            {
                effectInstance = GameObject.Instantiate(effectPrefab);
                Debug.Log("Created effect " + effectInstance.name, effectInstance);
            }
            effectInstance.transform.parent = target.transform;
            effectInstance.transform.SetPositionAndRotation(target.transform.position, target.transform.rotation);
            effectInstance.SetActive(true);
            effectInstance.SendMessage("invokeCustomEvent", "Aim", SendMessageOptions.DontRequireReceiver);

            Debug.Log("Effect aim at " + target.name, effectInstance);
        }
        else
        {
            Debug.LogWarning("effectPrefab == null for " + this);
        }
    }

    public void DisableAimingEffect()
    {
        if (effectInstance != null)
            effectInstance.SetActive(false);
    }

    public void PlayEffectAnimation(GameObject target)
    {
        if (effectInstance != null)
        {
            effectInstance.transform.parent = target.transform;
            effectInstance.transform.SetPositionAndRotation(target.transform.position, target.transform.rotation);
            effectInstance.SetActive(true);
            effectInstance.SendMessage("invokeCustomEvent", "Play", SendMessageOptions.DontRequireReceiver);
            Debug.Log("Effect Play at " + target.name, effectInstance);
        }
    }

    public void ApplyEffectTo(GameObject target)
    {
        PlayEffectAnimation(target);

        target.SendMessage("invokeCustomEvent", "OnHit" + name, SendMessageOptions.DontRequireReceiver);
    }

    public bool CanBeAppliedTo(GameObject target)
    {

        var myChar = transform.GetComponentInParent<SelectableCharacter>();
        if (myChar == null)
        {
            return false;
        }

        var targetChar = target.GetComponent<SelectableCharacter>();

        bool result = CanBeAppliedTypeCheck(myChar, targetChar) && CanBeAppliedDistanceCheck(myChar, targetChar);

        return result;
    }

    bool CanBeAppliedTypeCheck(SelectableCharacter myChar, SelectableCharacter targetChar)
    {
        switch (targetType)
        {
            case TARGET_TYPE.ALL:
                return true;
            case TARGET_TYPE.ALLIES:

                if (myChar != null && myChar.characterSheet.teamId == targetChar.characterSheet.teamId)
                {
                    return true;
                }
                return false;
            case TARGET_TYPE.FOES:

                if (myChar != null && myChar.characterSheet.teamId != targetChar.characterSheet.teamId)
                {
                    return true;
                }
                return false;
            case TARGET_TYPE.SELF:
                if (myChar == targetChar)
                {
                    return true;
                }
                return false;
            case TARGET_TYPE.OBJECTS:
                if (targetChar == null)
                {
                    return true;
                }
                return false;
        }
        return true;
    }

    bool CanBeAppliedDistanceCheck(SelectableCharacter myChar, SelectableCharacter targetChar)
    {
        bool result = false;
        var distance = (myChar.gameObject.transform.position - targetChar.gameObject.transform.position).magnitude;
        if (distance <= MaxUseDistance)
        {
            switch (distanceType)
            {
                case DISTANCE_TYPE.MELEE:
                    result = true;
                    var rayStart = myChar.GetComponent<Collider>().bounds.center;
                    var ray = new Ray(rayStart, targetChar.GetComponent<Collider>().bounds.center - rayStart);
                    RaycastHit hitInfo;
                    if (Physics.Raycast(ray, out hitInfo, MaxUseDistance))
                    {
                        Debug.DrawLine(ray.origin, hitInfo.point, Color.red, 1.0f);
                        if (hitInfo.collider.gameObject == targetChar.gameObject)
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                            Debug.Log(myChar.name + "CAN'T perform MELEE attack to " + targetChar.name + " due to occlusion");
                        }
                    }
                    else
                    {
                        Debug.DrawRay(ray.origin, ray.direction, Color.green, 1.0f);
                    }
                    break;
                case DISTANCE_TYPE.RANGED:
                    result = true;
                    break;
                case DISTANCE_TYPE.MAGICAL:
                    result = true;
                    break;
            }
        }
        else
        {
            Debug.Log(myChar.name + "CAN'T perform attack to " + targetChar.name + " due to DISTANCE: " + distance + ">" + MaxUseDistance);
            result = false;
        }
        return result;
    }
}