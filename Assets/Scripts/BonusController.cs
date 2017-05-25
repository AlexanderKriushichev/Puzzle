using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BonusController : MonoBehaviour {

    public static BonusController bonusController;

    public SlotController slotController;

    [Header("LineBonus")]
    public GameObject lineEffectPrefab;
    public GameObject lineDestroyEffectPrefab;
    [Header("BoxBonus")]
    public List<BoxBonusSprite> boxBonusSprites = new List<BoxBonusSprite>();
    [Header("Star")]
    public Sprite starSprite;
    public GameObject starLinePrefab;
    [Header("Area")]
    public GameObject area;
    public List<AreaColor> areaColors = new List<AreaColor>();

    [SerializeField]
    private List<Planet> planetsForDestruction = new List<Planet>();

    void Awake()
    {
        bonusController = this;
    }

    void Update()
    {
        if (SlotController.CanPlanetMove && planetsForDestruction.Count!=0)
        {
            if (planetsForDestruction[0] != null)
            {
                planetsForDestruction[0].slot.DestroyPlanetInSlot();
                return;
            }
            else
            {
                planetsForDestruction.RemoveAt(0);
                return;
            }
        }
    }

    public static void InitArea(Planet planet)
    {
        if (planet.haveArea)
            return;

        GameObject initAreaTr = Instantiate(bonusController.area, planet.transform);
        initAreaTr.transform.localPosition = Vector3.zero;

        SpriteRenderer initAreaSR = initAreaTr.GetComponent<SpriteRenderer>();
        
        initAreaSR.color = bonusController.areaColors.Find(x => x.typeOfPlanet == planet.typeOfPlanet).color;
        initAreaSR.sortingOrder = planet.spritePlanet.sortingOrder - 1;

        planet.haveArea = true;
    }

    /// <summary>
    /// Добавить бонус линии
    /// </summary>
    /// <param name="slot">Слот</param>
    public static void AddLineBonus(Slot slot)
    {
        InitArea(slot.planet);
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
        InitArea(slot.planet);

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

        //Костыль
        if (slot.planet.GetComponentInChildren<TrailRenderer>() != null)
        {
            Destroy(slot.planet.GetComponentInChildren<TrailRenderer>().gameObject);
        }

        foreach (BoxBonusSprite boxBonusSprite in bonusController.boxBonusSprites)
        {
            if (boxBonusSprite.typeOfPlanet == slot.planet.typeOfPlanet)
            {
                slot.planet.SetSprite(boxBonusSprite.sprite);
                break;
            }
        }
    }

    public static void AddStarBonus(Slot slot)
    {
        InitArea(slot.planet);

        slot.planet.SetBonus(TypeOfBonus.Star);
        slot.planet.typeOfPlanet = TypeOfPlanet.Star;
        slot.planet.SetSprite(bonusController.starSprite);

    }

    /// <summary>
    /// Активация бонуса
    /// </summary>
    /// <param name="x">Положение X</param>
    /// <param name="y">Положение Y</param>
    /// <param name="typeOfBonus">Тип бонуса</param>
    public static void ActivateBonus(int x, int y, TypeOfPlanet typeOfPlanet, TypeOfBonus typeOfBonus, TypeOfBonus typeOfExPlanetBonus = TypeOfBonus.None)
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
            case TypeOfBonus.Star:
                {
                    StarBonusActivate(x, y, typeOfPlanet, typeOfExPlanetBonus);
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
        bonusController.slotController.slots[x, y].planet.dontDestroy = true;


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

        bonusController.slotController.slots[x, y].planet.gameObject.AddComponent<FlickeringSprite>().SetFlickering(2, Ease.OutSine, Ease.InSine);

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

    public static void StarBonusActivate(int x,int y,TypeOfPlanet typeOfPlanet,TypeOfBonus typeOfExPlanetBonus)
    {
        for (int i = 0; i < SlotController.sizeField; i++)
        {
            for (int j = 0; j < SlotController.sizeField; j++)
            {
                if (bonusController.slotController.slots[i,j].planet!=null && bonusController.slotController.slots[i,j].planet.typeOfPlanet == typeOfPlanet)
                {
                    Slot destroySlot = bonusController.slotController.slots[i, j];
                    GameObject initLine = Instantiate(bonusController.starLinePrefab, bonusController.slotController.slots[x, y].transform.position, bonusController.slotController.slots[x, y].transform.rotation);
                    initLine.GetComponent<TrailRenderer>().sortingOrder = 1;
                    initLine.transform.DOMove(bonusController.slotController.slots[i, j].transform.position, 0.5f).OnComplete(delegate 
                    {
                        Destroy(initLine);
                        if (typeOfExPlanetBonus == TypeOfBonus.None)
                        {
                            destroySlot.DestroyPlanetInSlot();
                        }
                        else
                        {
                            if (destroySlot == bonusController.slotController.slots[x, y])
                            {
                                destroySlot.DestroyPlanetInSlot();
                            }
                            else
                            {
                                if (!destroySlot.planet.HaveLineBonus())
                                {//рандомное значение: вертикальный или горизонтальный бонус
                                    destroySlot.planet.SetDirectionOfMove();
                                    AddLineBonus(destroySlot);
                                    destroySlot.planet.SetBonus(typeOfExPlanetBonus);
                                }
                                bonusController.planetsForDestruction.Add(destroySlot.planet);
                            }
                        }
                    });
                }
            }
        }
        SlotController.CanPlanetMove = true;
    }
}


public enum TypeOfBonus 
{
    None,
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

[System.Serializable]
public struct AreaColor
{
    public TypeOfPlanet typeOfPlanet;
    public Color color;
}