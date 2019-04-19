using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityBar : MonoBehaviour
{
    public Button abilityButtonSample;

    Dictionary<Button, Ability> abilityButtons = new Dictionary<Button, Ability>();

    List<GameObject> abilityButtonsObjects = new List<GameObject>();

    Ability selectedAbility = null;

    public Ability getSelectedAbility()
    {
        return selectedAbility;
    }

    private void Start()
    {
        foreach(Transform child in transform)
        {
            if (child.GetComponent<Button>() != null)
            {
                abilityButtonsObjects.Add(child.gameObject);
            }
        }
    }

    public void ShowAbilities(ICollection<Ability> abilities )
    {
        foreach (var btn in abilityButtons.Keys)
        {
            btn.gameObject.SetActive(false);
        }

        int btnIndex = 0;
        foreach (var ability in abilities)
        {
            Button btn;
            
            if (btnIndex < abilityButtonsObjects.Count)
            {
                btn = abilityButtonsObjects[btnIndex].GetComponent<Button>();
            }
            else
            {
                var buttonPrefab = abilityButtonsObjects[0];
                var btnObject = GameObject.Instantiate(buttonPrefab, buttonPrefab.transform.parent);
                btnObject.name = buttonPrefab.name + " " + btnIndex;
                abilityButtonsObjects.Add(btnObject);
                btnObject.SetActive(true);
                btn = btnObject.GetComponent<Button>();
                
            }

            if (btn == null)
            {
                Debug.LogError("Can't create button for ability " + btnIndex);
            }
            else
            {
                var btnText = btn.GetComponentInChildren<Text>();
                if (btnText != null)
                    btnText.text = ability.name;

                btn.gameObject.SetActive(true);

                btn.onClick.AddListener(() => { onButtonClicked(btn); });

                if (abilityButtons.ContainsKey(btn))
                {
                    abilityButtons[btn] = ability;
                }
                else
                {
                    abilityButtons.Add(btn, ability);
                }
            }

            btnIndex++;
        }
    }

    void onButtonClicked(Button btn)
    {
        if (abilityButtons.TryGetValue(btn, out Ability ability))
        {
            selectedAbility = ability;
        }
    }
}

//public class Ability
//{
//    public GameObject effectPrefab;

//    GameObject effectInstance;

//    public void CreateAimingEffect(GameObject target)
//    {
//        if (effectPrefab != null)
//        {
//            if (effectInstance == null)
//            {
//                effectInstance = GameObject.Instantiate(effectPrefab);
//                Debug.Log("Created effect "+effectInstance.name, effectInstance);
//            }
//            effectInstance.transform.parent = target.transform;
//            effectInstance.transform.SetPositionAndRotation(target.transform.position, target.transform.rotation);
//            effectInstance.SetActive(true);
//            effectInstance.SendMessage("invokeCustomEvent", "Aim", SendMessageOptions.DontRequireReceiver);

//            Debug.Log("Effect aim at " + target.name, effectInstance);
//        }
//        else
//        {
//            Debug.LogWarning("effectPrefab == null for "+this);
//        }
//    }

//    public void DisableAimingEffect()
//    {
//        if (effectInstance != null)
//            effectInstance.SetActive(false);
//    }

//    public void PlayEffect(GameObject target)
//    {
//        if (effectInstance != null)
//        {
//            effectInstance.transform.parent = target.transform;
//            effectInstance.transform.SetPositionAndRotation(target.transform.position, target.transform.rotation);
//            effectInstance.SetActive(true);
//            effectInstance.SendMessage("invokeCustomEvent", "Play", SendMessageOptions.DontRequireReceiver);
//            Debug.Log("Effect Play at " + target.name, effectInstance);
//        }
//    }
//}
