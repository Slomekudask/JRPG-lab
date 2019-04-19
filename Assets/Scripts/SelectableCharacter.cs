using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class UnityEventPointer: UnityEvent<PointerEventData>
{
}


public class SelectableCharacter : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEventPointer onClicked;

    public UnityEvent onSelected;

    public UnityEvent onDeselected;

    public UnityEventPointer onPointerEnter;

    public UnityEventPointer onPointerExit;

    public CharacterSheet characterSheet;

    //public InventoryItemList inventory;

    Slider healthBarSlider;

    [HideInInspector]
    public Ability[] abilities;

    void OnSelected()
    {
        onSelected.Invoke();
    }

    void OnDeselected()
    {
        onDeselected.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Character clicked: "+name, gameObject);

        onClicked.Invoke(eventData);
    }

    // Start is called before the first frame update
    void Start()
    {
        abilities = GetComponentsInChildren<Ability>();

        var infoCanvas = transform.Find("Character Canvas");
        if (infoCanvas != null)
            healthBarSlider = infoCanvas.GetComponentInChildren<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.value = characterSheet.health;
        }
    }

    public void ChangeHealth(float amount)
    {
        characterSheet.health += amount;
    }

    public void ChangePower(float amount)
    {
        characterSheet.power += amount;
    }

    void ApplyEffect(string effectName)
    {
        Debug.Log("Effect "+effectName+" applied", gameObject);
    }
    public void UseAbility(ref Ability ability, SelectableCharacter targetCharacter)
    {
        if (ability == null || string.IsNullOrEmpty(ability.name))
        {
            Debug.LogWarning("No ability was selected to use", gameObject);
            return;
        }

        if (targetCharacter == null)
        {
            Debug.LogWarning("No targetCharacter was selected to use onto", gameObject);
            return;
        }

        Debug.Log(name + " used ability " + ability.name + " on " + targetCharacter.name, this);

        //if (ability.effectPrefab != null)
        //{
        //    GameObject.Instantiate(ability.effectPrefab, targetCharacter.transform);
        //}
        ability.ApplyEffectTo(targetCharacter.gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnter.Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExit.Invoke(eventData);
    }
}

[System.Serializable]
public class CharacterSheet
{
    public string name = "DefaultCharacter";
    public float health = 1.0f;
    public float power = 1.0f;
    public int teamId = 0;

    public Texture2D avatar = null;
}



