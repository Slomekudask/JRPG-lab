using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JRPGGameManager : Singleton<JRPGGameManager>
{
    private SelectableCharacter _selectedCharacter;

    public AbilityBar abilityBar;

    SelectableCharacter[] characters;

    public SelectableCharacter pointedCharacter;

    public bool popperchicks;
    
    public SelectableCharacter selectedCharacter //это геттер - это переменная у которой есть функция порлучения и назначения
    {
        get { return _selectedCharacter; }
        set
        {
            if ( _selectedCharacter != value )
            {
                if (_selectedCharacter != null)
                {
                    _selectedCharacter.SendMessage("OnDeselected", SendMessageOptions.DontRequireReceiver); // уведомляем объект который мы выбрали до этого о том что он более не избран. Путин уходи.
                }

                _selectedCharacter = value;

                _selectedCharacter.SendMessage("OnSelected", SendMessageOptions.DontRequireReceiver);

                ShowAbilityBar(_selectedCharacter);


            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        characters = FindObjectsOfType<SelectableCharacter>();

        foreach(SelectableCharacter character in characters)
        {
                character.onClicked.AddListener((eventData) => { OnCharacterWasClicked(character); });

                character.onPointerEnter.AddListener((eventData) => { OnCharacterPointed(character); });

                character.onPointerExit.AddListener((eventData) => { OnCharacterPointerExited(character); });
        }

        if (abilityBar == null)
        {
            abilityBar = GetComponentInChildren<AbilityBar>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnCharacterPointed(SelectableCharacter pointedCharacter)
    {
        Debug.Log("pointed "+ pointedCharacter.name);
        var selectedAbility = abilityBar.getSelectedAbility();
        if (selectedAbility != null)
        {
            if (selectedAbility.CanBeAppliedTo(pointedCharacter.gameObject))
            {
                //Debug.Log("abilityBar.selectedAbility " + abilityBar.getSelectedAbility().name + " CAN be applied to " + pointedCharacter.name);
                selectedAbility.CreateAimingEffect(pointedCharacter.gameObject);
            }
            else
            {
                Debug.Log("abilityBar.selectedAbility " + abilityBar.getSelectedAbility().name+" CAN'T be applied to "+ pointedCharacter.name);
            }
        }
    }

    public void OnCharacterPointerExited(SelectableCharacter clickedCharacter)
    {
        var selectedAbility = abilityBar.getSelectedAbility();
        if (selectedAbility != null)
        {
            selectedAbility.DisableAimingEffect();
        }
    }

    public void OnCharacterWasClicked(SelectableCharacter clickedCharacter) //При клике на чара если он в нашей тиме то селектед станеи им
    {
        //Debug.Log("OnCharacterWasClicked "+character);

        if (clickedCharacter.characterSheet.teamId == 0)
        {
            selectedCharacter = clickedCharacter;
            Debug.Log("selectedCharacter: " + selectedCharacter, selectedCharacter);
        }
        else
        {
            if (selectedCharacter != null)
            {
                var ability = abilityBar.getSelectedAbility();
                if (ability.CanBeAppliedTo(clickedCharacter.gameObject))
                {
                    selectedCharacter.UseAbility(ref ability, clickedCharacter);
                }
                
            }
        }
    }

    public void Popping(bool pop)
    {
        popperchicks = pop;
    }

    void ShowAbilityBar(SelectableCharacter character)
    {
        if (abilityBar == null)
        {
            Debug.LogError("abilityBar was not set properly!", gameObject);
            enabled = false;
            return;
        }

        if (character != null)
        {
            abilityBar.gameObject.SetActive(true);
            abilityBar.ShowAbilities(character.abilities);
        }
        else
        {
            abilityBar.gameObject.SetActive(false);
        }

        abilityBar.DeselectAbility();
    }
}
