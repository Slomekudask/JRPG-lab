using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class UnityEventPointer: UnityEvent<PointerEventData> // делаем новый класс свой который наследуем от ЮнитиЭвент. Изначально юнитиэвент без аргументов, но добавляем пойнтер эвент дата для того чтобы дать аргумент координат
{
}


public class SelectableCharacter : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler // Делаем класс персонажей для врагов и героев, наследуем его от моноброви и интерфейсов
{
    public UnityEventPointer onClicked; // делаем переменные, которые по типу относятся к классу какого-нибудь события (это навроде инт и так далее для них). а само они внутри класса персонажа

    public UnityEvent onSelected;

    public UnityEvent onDeselected;

    public UnityEventPointer onPointerEnter;

    public UnityEventPointer onPointerExit;

    public CharacterSheet characterSheet;

    public GameObject defeatedCharPrefab, defeatedCharPrefab2;

    private SpriteRenderer rend;

   // public Sprite sprite1, sprite2, sprite3;

    public Sprite[] SpritesCollection;

    //public InventoryItemList inventory;

    Slider healthBarSlider; //делаем хелсбар - только объявляем! Он не появляется от объявления!

    Text healthBarText;

    [HideInInspector]
    public Ability[] abilities; //массив абилок делаем чтобы их было легко перебирать, у каждого юнита будет свой массив

    void OnSelected() //когда выбираем чара
    {
        onSelected.Invoke();
    }

    void OnDeselected() //когда перестаём выбирать
    {
        onDeselected.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData) //когда кликаем на перса
    {
        Debug.Log("Character clicked: "+name, gameObject);

        onClicked.Invoke(eventData);
    }

    // Start is called before the first frame update
    void Start()
    {
        abilities = GetComponentsInChildren<Ability>(); //переменная абилмитиз заполняется из массива абилок. который найдёт в дочерних объектах

        var characterCanvasGameObject = transform.Find("Character Canvas"); //ищем в объекте чароканвас и заполняем переменную-ссылку на него
        if (characterCanvasGameObject != null)// если инфоканвас есть (есть чараканвас)
        {
            healthBarSlider = characterCanvasGameObject.GetComponentInChildren<Slider>();
            healthBarText = characterCanvasGameObject.GetComponentInChildren<Text>();
        }
        // ищем через ссылку на чароканвас слайдер в нём

        rend = GetComponent<SpriteRenderer>();
        //sprite1 = Sprites.Load<Sprite>("Sprite1");
        //sprite2 = Resources.Load<Sprite>("Sprite2");
        //sprite3 = Resources.Load<Sprite>("Sprite3");
        //rend.sprite = sprite1;

    }

    // Update is called once per frame
    void Update()
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.value = characterSheet.health;
        }

        if (healthBarText != null)
        {
            healthBarText.text = characterSheet.health + "/" + characterSheet.maxHealth;
        }
        IfCharDefeated();

        rend.sprite = SpritesCollection[(int)healthBarSlider.value];

    }

    public void IfCharDefeated()
    {
        if (characterSheet.health >= characterSheet.maxHealth)
        {
            if (JRPGGameManager.Instance.popperchicks)
            {
                GameObject.Instantiate(defeatedCharPrefab, transform.position, transform.rotation);
            } 
            else
            {
                GameObject.Instantiate(defeatedCharPrefab2, transform.position, transform.rotation);
            }

            Destroy(gameObject);

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

    void ApplyEffect(string effectName) //заготовка чтобы применять маг-эффекты и вообще эффекты
    {
        Debug.Log("Effect "+effectName+" applied", gameObject);
    }
    public void UseAbility(ref Ability ability, SelectableCharacter targetCharacter) //ссылка на конкретную применяемую абилку, потом пишем когда надо - если что-то то юз абилити
    {
        if (ability == null || string.IsNullOrEmpty(ability.name)) // если абюилки нет или её имя не задано
        {
            Debug.LogWarning("No ability was selected to use", gameObject); // то тогда сообщение об ошибке
            return;
        }

        if (targetCharacter == null)
        {
            Debug.LogWarning("No targetCharacter was selected to use onto", gameObject);
            return;
        }

        Debug.Log(name + " used ability " + ability.name + " on " + targetCharacter.name, this);

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

[System.Serializable] //это значит что можно в редакторе отображать
public class CharacterSheet
{
    public string name = "DefaultCharacter";
    public float health = 1.0f;
    public float maxHealth = 3.0f;
    public float power = 1.0f;
    public int teamId = 0;
    public Texture2D avatar = null;
}
