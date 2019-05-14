using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TARGET_TYPE //Типы целей
{
    ALL,
    FOES,
    ALLIES,
    SELF,
    OBJECTS
}

public enum DISTANCE_TYPE //Типы дистанций
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

    public void CreateAimingEffect(GameObject target) //Создание эффекта при наведении на цель (подсветка)
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

    public void DisableAimingEffect() //Изничтожение эффекта наведения при убирании мышки
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

    public bool CanBeAppliedTo(GameObject target) //Проверка возможности применения
    {

        var myChar = transform.GetComponentInParent<SelectableCharacter>(); //Получаем инфу о выделенном чаре (положение)
        if (myChar == null)
        {
            return false;
        }

        var targetChar = target.GetComponent<SelectableCharacter>(); //Получаем инфу о цели (враг/союзник и дистанция до него)

        bool result = CanBeAppliedTypeCheck(myChar, targetChar) && CanBeAppliedDistanceCheck(myChar, targetChar); //Сравнение возможности применения по типу и дистанции

        return result;
    }

    bool CanBeAppliedTypeCheck(SelectableCharacter myChar, SelectableCharacter targetChar) //Проверка применяемости по типу (враги-союзники)
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

    /// <summary>
    /// Возвращает ближайшего к myChar персонажа типа SelectableCharacter 
    /// путем перебора сначала по близости на оси X, а затем выбирая ближайшего из них по оси Z
    /// </summary>
    /// <param name="myChar"></param>
    /// <returns></returns>
    SelectableCharacter FindClosestEnemy(SelectableCharacter myChar)
    {
        float closestEnemyDistanceX = Mathf.Infinity; // приравняли в рекорд бесконечность
        SelectableCharacter closestEnemy = null; // приравняли что нету клозеста

        List<SelectableCharacter> closestEnemiesByX = new List<SelectableCharacter>(); //массив перебора ближайших по Х

        
        var otherCharacters = FindObjectsOfType<SelectableCharacter>(); //найдем всех персонажей

        foreach (SelectableCharacter otherChar in otherCharacters)
        {
            if (otherChar.characterSheet.teamId == myChar.characterSheet.teamId) //пропускаем персонажей своей команды
            {
                continue;
            }

            
            var newAttackVector = (otherChar.gameObject.transform.position - myChar.gameObject.transform.position); //получим вектор направленный от нашего персонажа на врага - на того которого мы перебираем

            var distance = Mathf.Abs(Mathf.Round(newAttackVector.x)); //дистанция до перебираемого врага. мы ещё и округляем

            
            if (distance < closestEnemyDistanceX) //наиболее близкого запишем
            {
                closestEnemiesByX.Clear(); //очистим список ближайших
                
                closestEnemyDistanceX = distance; //наименьшая дистанция по X теперь такая
                
                closestEnemy = otherChar; //ближайший враг
                
                closestEnemiesByX.Add(otherChar); //он лишь один среди ближайших по оси Х
            }
            else if (distance == closestEnemyDistanceX)
            {
                closestEnemiesByX.Add(otherChar); //другие враги столь же близкие по оси Х добавляем в массив
            }
        }
        Debug.Log("Число врагов в ближайшем не пустом ряду " + closestEnemiesByX.Count);

        var closestEnemyDistanceZ = Mathf.Infinity;

        //найдем ближайнего по Z из тех, кто в ближайшем непустом ряду по X
        foreach (var enemy in closestEnemiesByX)
        {
            var newAttackVector = (enemy.gameObject.transform.position - myChar.gameObject.transform.position);
            float distance = Mathf.Abs(newAttackVector.z);

            if (distance <= closestEnemyDistanceZ)
            {
                closestEnemyDistanceZ = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    bool AllyOnTheWay(SelectableCharacter myChar)
    {
        bool allyOnTheWay = false;
        var otherCharacters = FindObjectsOfType<SelectableCharacter>(); //найдем всех персонажей

        foreach (SelectableCharacter otherChar in otherCharacters)
        {
            if (otherChar.characterSheet.teamId == myChar.characterSheet.teamId)
            {
                var oneStep = myChar.transform.position.x + 0.1f;
                if (otherChar.transform.position.x >= oneStep)
                {
                    allyOnTheWay = true;
                    break;
                }
            }
        }
        return allyOnTheWay;
    }

    bool CanBeAppliedDistanceCheck(SelectableCharacter myChar, SelectableCharacter targetChar) //Проверка применяемости по дистанции
    {
        bool result = false;
        switch (distanceType)
        {
            case DISTANCE_TYPE.MELEE:
                result = true;
                var attackVector = (targetChar.gameObject.transform.position - myChar.gameObject.transform.position);
                var distance = attackVector.magnitude;

                if (distance <= MaxUseDistance)
                {
                    //Если цель находится в радиусе, значит можем бить всегда
                    //Debug.Log("Can attack in radius " + distance);
                }
                else
                {
                        var onTheWay = AllyOnTheWay(myChar);
                        if (onTheWay == true)
                        {
                            result = false;
                        }
                        else
                        {
                            //Если цель делеко, то проверим, нет ли кого-то ближе
                            var closestEnemy = FindClosestEnemy(myChar);
                            if (closestEnemy != targetChar)
                            {

                                result = false;

                                if (Mathf.Round(closestEnemy.transform.position.x) == Mathf.Round(targetChar.transform.position.x))
                                {
                                    if (Mathf.Abs(targetChar.transform.position.z - myChar.transform.position.z) <= MaxUseDistance)
                                    {
                                        result = true;
                                    }
                                }

                                //Debug.LogWarning("Can't attack, closest enemy is " + closestEnemy.gameObject.name);
                            }

                        }
                }
                break;
            case DISTANCE_TYPE.RANGED:
                result = true;
                break;
            case DISTANCE_TYPE.MAGICAL:
                result = true;
                break;
        }
        return result;
    }
}