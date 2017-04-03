using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusController : MonoBehaviour {

    public static BonusController bonusController;

    public SlotController slotController;

    [Header("LineBonus")]
    public GameObject lineEffectPrefab;
    public GameObject lineDestroyEffectPrefab;
    [Header("BoxBonus")]
    public List<BoxBonusSprite> boxBonusSprites = new List<BoxBonusSprite>();
    

    void Awake()
    {
        bonusController = this;
    }

    /// <summary>
    /// Добавить бонус линии
    /// </summary>
    /// <param name="slot">Слот</param>
    public static void AddLineBonus(Slot slot)
    {
        if (slot.planet.GetDirectionOfMove() == TypeLineBonus.Horizontal)
            slot.planet.SetBonus(TypeOfBonus.HorizontalLine);
        else
            slot.planet.SetBonus(TypeOfBonus.VerticalLine);

        Instantiate(bonusController.lineEffectPrefab, slot.planet.transform).GetComponent<LineTraceEffect>().Move(slot.planet.GetDirectionOfMove());
    }

    /// <summary>
    /// Добавить первый бонус квадрат
    /// </summary>
    /// <param name="slot">Слот</param>
    public static void AddBoxFirstBonus(Slot slot)
    {
        slot.planet.SetBonus(TypeOfBonus.BoxFirst);

        foreach (BoxBonusSprite boxBonusSprite in bonusController.boxBonusSprites)
        {
            if (boxBonusSprite.typeOfPlanet == slot.planet.typeOfPlanet)
            {
                slot.planet.SetSprite(boxBonusSprite.sprite);
                break;
            }
        }
    }

    /// <summary>
    /// Добавить второй бонус квадрат
    /// </summary>
    /// <param name="slot">Слот</param>
    public static void AddBoxSecondBonus(Slot slot)
    {
        slot.planet.SetBonus(TypeOfBonus.BoxSecond);

        foreach (BoxBonusSprite boxBonusSprite in bonusController.boxBonusSprites)
        {
            if (boxBonusSprite.typeOfPlanet == slot.planet.typeOfPlanet)
            {
                slot.planet.SetSprite(boxBonusSprite.sprite);
                break;
            }
        }
    }

    /// <summary>
    /// Активация бонуса
    /// </summary>
    /// <param name="x">Положение X</param>
    /// <param name="y">Положение Y</param>
    /// <param name="typeOfBonus">Тип бонуса</param>
    public static void ActivateBonus(int x, int y, TypeOfPlanet typeOfPlanet,TypeOfBonus typeOfBonus)
    {
        switch (typeOfBonus)
        {
            case TypeOfBonus.HorizontalLine:
                {
                    HorizontalLineActivate(x, y);
                    break;
                }
            case TypeOfBonus.VerticalLine:
                {
                    VerticalLineActivate(x, y);
                    break;
                }
            case TypeOfBonus.BoxFirst:
                {
                    BoxFirstBonusActivate(x, y, typeOfPlanet);
                    break;
                }
            case TypeOfBonus.BoxSecond:
                {
                    BoxSecondActivate(x, y);
                    break;
                }
        }
    }

    /// <summary>
    /// Активация горизонтального бонуса
    /// </summary>
    /// <param name="x">Положение X</param>
    /// <param name="y">Положение Y</param>
    public static void HorizontalLineActivate(int x, int y)
    {

        Transform destroyLine = Instantiate(bonusController.lineDestroyEffectPrefab, bonusController.slotController.slots[x, y].transform).transform;
        destroyLine.localPosition = Vector3.zero;

        bonusController.slotController.slots[x, y].DestroyPlanetInSlot();

        for (int i = 1; i < SlotController.sizeField; i++)
        {
            if (x - i >= 0)
            {
                bonusController.slotController.slots[x - i, y].DestroyPlanetInSlot();
            }

            if (x + i < SlotController.sizeField)
            {
                bonusController.slotController.slots[x + i, y].DestroyPlanetInSlot();
            }
        }
    }

    /// <summary>
    /// Активация вертикального бонуса
    /// </summary>
    /// <param name="x">Положение X</param>
    /// <param name="y">Положение Y</param>
    public static void VerticalLineActivate(int x, int y)
    {

        Transform destroyLine = Instantiate(bonusController.lineDestroyEffectPrefab, bonusController.slotController.slots[x, y].transform).transform;
        destroyLine.localEulerAngles = new Vector3(0, 0, 90);
        destroyLine.localPosition = Vector3.zero;

        bonusController.slotController.slots[x, y].DestroyPlanetInSlot();

        for (int i = 1; i < SlotController.sizeField; i++)
        {
            if (y - i >= 0)
            {
                bonusController.slotController.slots[x, y - i].DestroyPlanetInSlot();
            }

            if (y + i < SlotController.sizeField)
            {
                bonusController.slotController.slots[x, y + i].DestroyPlanetInSlot();
            }
        }
    }

    /// <summary>
    /// Активация первого бонуса квадрата
    /// </summary>
    /// <param name="x">Положение X</param>
    /// <param name="y">Положение Y</param>
    public static void BoxFirstBonusActivate(int x, int y, TypeOfPlanet typeOfPlanet)
    {

        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (i >= 0 && j >= 0 && i < SlotController.sizeField && j < SlotController.sizeField && (i != x || j != y))
                {
                    bonusController.slotController.slots[i,j].DestroyPlanetInSlot();
                }               
            }
        }

        //bonusController.slotController.GeneratePlanet(typeOfPlanet, bonusController.slotController.slots[x, y]);

        AddBoxSecondBonus(bonusController.slotController.slots[x, y]);

        SlotController.CanPlanetMove = false;

        bonusController.slotController.slots[x, y].planet.DestroyWhenCanMove();
    }

    public static void BoxSecondActivate(int x, int y)
    {
        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (i >= 0 && j >= 0 && i < SlotController.sizeField && j < SlotController.sizeField)
                {
                    bonusController.slotController.slots[i, j].DestroyPlanetInSlot();
                }
            }
        }
    }
}

public enum TypeOfBonus 
{
    HorizontalLine,
    VerticalLine,
    BoxFirst,
    BoxSecond,
    Star
}

[System.Serializable]
public struct BoxBonusSprite
{
    public TypeOfPlanet typeOfPlanet;
    public Sprite sprite;
}
