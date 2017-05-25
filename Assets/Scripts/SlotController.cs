using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class SlotController : MonoBehaviour
{

    private static int _sizeField = 8;
    /// <summary>
    /// Размер поля
    /// </summary>
    public static int sizeField
    {
        get
        {
            return _sizeField;
        }
    }

    [SerializeField]
    private List<Slot> slotsList = new List<Slot>();
    [SerializeField]
    private List<TypeOfPlanet> planetGenerateList = new List<TypeOfPlanet>();

    public List<PlanetPrefab> planetPrefabs = new List<PlanetPrefab>();

    public Slot[,] slots = new Slot[sizeField, sizeField];

    private List<PlanetPosition>[,] slotPlanetsList = new List<PlanetPosition>[sizeField, sizeField];
    private bool[,] slotHavePlanet = new bool[sizeField, sizeField];

    private List<List<Slot>> combination = new List<List<Slot>>();

    private bool[,] planetsInCombination = new bool[sizeField, sizeField];
    private bool[,] planetsAddInCombination = new bool[sizeField, sizeField];

    private int[,] planetsMoveCounter = new int[sizeField, sizeField];
    private int moveCounter = 0;


    [HideInInspector]
    public Slot firstSelectSlotToExchange;

    private static bool _canPlanetMove = true;
    public static bool CanPlanetMove
    {
        get
        {
            return _canPlanetMove;
        }
        set
        {
            _canPlanetMove = value;
        }
    }

    private bool inverse = false;

    private bool combinationFound = true;

    public bool moveComplite = true;

    private bool boxBonus = false;

    #region VariablesMovePlanets

    bool planetDontMoveMovePlanets = true;
    Planet selectPlanetMovePlanets;
    int countMovePlanets;

    #endregion


    // Use this for initialization
    void Start()
    {

        foreach (Slot slot in slotsList)
        {
            slots[slot.x, slot.y] = slot;
        }

        for (int i = 0; i < sizeField; i++)
        {
            for (int j = 0; j < sizeField; j++)
            {
                slotPlanetsList[i, j] = new List<PlanetPosition>();
            }
        }

        CreatePlanets();
        CanPlanetMove = true;
    }


    /// <summary>
    /// Создание поля заполненого планетами
    /// </summary>
    void CreatePlanets()
    {
        for (int i = 0; i < sizeField; i++)
        {
            for (int j = 0; j < sizeField; j++)
            {
                if ((slots[i, j].typeOfSlot == TypeOfSlot.None || slots[i, j].typeOfSlot == TypeOfSlot.Generator) && slots[i, j].planet == null)
                {
                    GeneratePlanet(GenerateTypePlanet(i, j), slots[i, j]);
                }
                else
                {
                    slots[i, j].slotController = this;
                }
            }
        }
    }

    /// <summary>
    /// Генерирует тип планеты
    /// </summary>
    /// <param name="x">X</param>
    /// <param name="y">Y</param>
    /// <returns>Тип планеты</returns>
    private TypeOfPlanet GenerateTypePlanet(int x, int y)
    {
        List<TypeOfPlanet> _planetGenerateList = new List<TypeOfPlanet>();
        foreach (TypeOfPlanet planetType in planetGenerateList)
        {
            _planetGenerateList.Add(planetType);
        }

        if (x > 1 && slots[x - 1, y].typeOfSlot == TypeOfSlot.None && slots[x - 2, y].typeOfSlot == TypeOfSlot.None)
        {
            if (slots[x - 1, y].planet != null && slots[x - 2, y].planet != null)
            {
                if (slots[x - 1, y].planet.typeOfPlanet == slots[x - 2, y].planet.typeOfPlanet)
                {
                    _planetGenerateList.Remove(slots[x - 1, y].planet.typeOfPlanet);
                }
            }
        }
        if (y > 1 && slots[x, y - 1].typeOfSlot == TypeOfSlot.None && slots[x, y - 2].typeOfSlot == TypeOfSlot.None)
        {
            if (slots[x, y - 1].planet != null && slots[x, y - 2].planet != null)
            {
                if (slots[x, y - 1].planet.typeOfPlanet == slots[x, y - 2].planet.typeOfPlanet)
                {
                    _planetGenerateList.Remove(slots[x, y - 1].planet.typeOfPlanet);
                }
            }
        }
        if (x < sizeField - 2 && slots[x + 1, y].typeOfSlot == TypeOfSlot.None && slots[x + 2, y].typeOfSlot == TypeOfSlot.None)
        {
            if (slots[x + 1, y].planet != null && slots[x + 2, y].planet != null)
            {
                if (slots[x + 1, y].planet.typeOfPlanet == slots[x + 2, y].planet.typeOfPlanet)
                {
                    _planetGenerateList.Remove(slots[x + 1, y].planet.typeOfPlanet);
                }
            }
        }
        if (y < sizeField - 2 && slots[x, y + 1].typeOfSlot == TypeOfSlot.None && slots[x, y + 2].typeOfSlot == TypeOfSlot.None)
        {
            if (slots[x, y + 1].planet != null && slots[x, y + 2].planet != null)
            {
                if (slots[x, y + 1].planet.typeOfPlanet == slots[x, y + 2].planet.typeOfPlanet)
                {
                    _planetGenerateList.Remove(slots[x, y + 1].planet.typeOfPlanet);
                }
            }
        }
        int newColor = 0;
        if (_planetGenerateList.Count != 0)
        {
            newColor = Random.Range(0, _planetGenerateList.Count);
            return _planetGenerateList[newColor];
        }
        else
        {
            newColor = Random.Range(0, planetGenerateList.Count);
            return planetGenerateList[newColor];
        }
    }

    /// <summary>
    /// Генерирует планету в слоте
    /// </summary>
    /// <param name="typeOfPlanet">Тип планеты</param>
    /// <param name="slot">Слот</param>
    /// <returns>Планета</returns>
    public Planet GeneratePlanet(TypeOfPlanet typeOfPlanet, Slot slot)
    {
        PlanetPrefab initPlanetPrefab = planetPrefabs.Find(x => x.typeOfPlanet == typeOfPlanet);
        Planet initPlanet = Instantiate(initPlanetPrefab.prefab, slot.transform).GetComponent<Planet>();
        initPlanet.transform.position = slot.transform.position;
        initPlanet.typeOfPlanet = initPlanetPrefab.typeOfPlanet;
        initPlanet.SetSprite(initPlanetPrefab.sprite);
        slot.planet = initPlanet;
        initPlanet.slot = slot;
        return initPlanet;
    }

    /// <summary>
    /// Поиск пути передвижения планет вниз
    /// </summary>
    private void FindWaysMovementPlanetsDown()
    {

        for (int j = sizeField - 2; j >= 0; j--)
        {
            for (int i = 0; i < sizeField; i++)
            {
                if (slots[i, j].typeOfSlot != TypeOfSlot.Barrier && slotHavePlanet[i, j])
                {
                    if (slots[i, j + 1].typeOfSlot != TypeOfSlot.Barrier && !slotHavePlanet[i, j + 1])
                    {
                        slotPlanetsList[i, j + 1].Add(new PlanetPosition(i, j));
                        slotHavePlanet[i, j] = false;
                        slotHavePlanet[i, j + 1] = true;
                        FindWaysMovementPlanetsDown();
                        return;
                    }

                }
                else
                {
                    if (slots[i, j].typeOfSlot == TypeOfSlot.Generator && !slotHavePlanet[i, j])
                    {
                        slotPlanetsList[i, j].Add(new PlanetPosition(i, j - 1));
                        slotHavePlanet[i, j] = true;
                        FindWaysMovementPlanetsDown();
                        return;
                    }
                }
            }
        }
        FindWaysMovementPlanetsSide();
    }

    /// <summary>
    /// Поиск пути передвижения планет в бок
    /// </summary>
    private void FindWaysMovementPlanetsSide()
    {
        for (int j = 0; j < sizeField; j++)
        {
            if (inverse)
            {
                for (int i = 0; i < sizeField; i++)
                {
                    if (slots[i, j].typeOfSlot != TypeOfSlot.Barrier && slotHavePlanet[i, j])
                    {
                        if (j < sizeField - 1)
                        {
                            if (i > 0)
                            {
                                if (slots[i - 1, j + 1].typeOfSlot != TypeOfSlot.Barrier && !slotHavePlanet[i - 1, j] && !slotHavePlanet[i - 1, j + 1])
                                {

                                    if (i < sizeField - 1)
                                    {
                                        if (slots[i + 1, j + 1].typeOfSlot != TypeOfSlot.Barrier && !slotHavePlanet[i + 1, j] && !slotHavePlanet[i + 1, j + 1])
                                        {
                                            if (slotPlanetsList[i + 1, j + 1].Count < slotPlanetsList[i - 1, j + 1].Count)
                                            {
                                                slotPlanetsList[i + 1, j + 1].Add(new PlanetPosition(i, j));
                                                slotHavePlanet[i, j] = false;
                                                slotHavePlanet[i + 1, j + 1] = true;
                                                inverse = !inverse;
                                                FindWaysMovementPlanetsDown();
                                                return;
                                            }
                                        }
                                    }
                                    slotPlanetsList[i - 1, j + 1].Add(new PlanetPosition(i, j));
                                    slotHavePlanet[i, j] = false;
                                    slotHavePlanet[i - 1, j + 1] = true;
                                    inverse = !inverse;
                                    FindWaysMovementPlanetsDown();
                                    return;
                                }
                            }

                            if (i < sizeField - 1)
                            {
                                if (slots[i + 1, j + 1].typeOfSlot != TypeOfSlot.Barrier && !slotHavePlanet[i + 1, j] && !slotHavePlanet[i + 1, j + 1])
                                {

                                    if (i > 0)
                                    {
                                        if (slots[i - 1, j + 1].typeOfSlot != TypeOfSlot.Barrier && !slotHavePlanet[i - 1, j] && !slotHavePlanet[i - 1, j + 1])
                                        {
                                            if (slotPlanetsList[i - 1, j + 1].Count < slotPlanetsList[i + 1, j + 1].Count)
                                            {
                                                slotPlanetsList[i - 1, j + 1].Add(new PlanetPosition(i, j));
                                                slotHavePlanet[i, j] = false;
                                                slotHavePlanet[i - 1, j + 1] = true;
                                                inverse = !inverse;
                                                FindWaysMovementPlanetsDown();
                                                return;
                                            }
                                        }
                                    }
                                    slotPlanetsList[i + 1, j + 1].Add(new PlanetPosition(i, j));
                                    slotHavePlanet[i, j] = false;
                                    slotHavePlanet[i + 1, j + 1] = true;
                                    inverse = !inverse;
                                    FindWaysMovementPlanetsDown();
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = sizeField-1; i >= 0; i--)
                {
                    if (slots[i, j].typeOfSlot != TypeOfSlot.Barrier && slotHavePlanet[i, j])
                    {
                        if (j < sizeField - 1)
                        {
                            if (i > 0)
                            {
                                if (slots[i - 1, j + 1].typeOfSlot != TypeOfSlot.Barrier && !slotHavePlanet[i - 1, j] && !slotHavePlanet[i - 1, j + 1])
                                {

                                    if (i < sizeField - 1)
                                    {
                                        if (slots[i + 1, j + 1].typeOfSlot != TypeOfSlot.Barrier && !slotHavePlanet[i + 1, j] && !slotHavePlanet[i + 1, j + 1])
                                        {
                                            if (slotPlanetsList[i + 1, j + 1].Count < slotPlanetsList[i - 1, j + 1].Count)
                                            {
                                                slotPlanetsList[i + 1, j + 1].Add(new PlanetPosition(i, j));
                                                slotHavePlanet[i, j] = false;
                                                slotHavePlanet[i + 1, j + 1] = true;
                                                inverse = !inverse;
                                                FindWaysMovementPlanetsDown();
                                                return;
                                            }
                                        }
                                    }
                                    slotPlanetsList[i - 1, j + 1].Add(new PlanetPosition(i, j));
                                    slotHavePlanet[i, j] = false;
                                    slotHavePlanet[i - 1, j + 1] = true;
                                    inverse = !inverse;
                                    FindWaysMovementPlanetsDown();
                                    return;
                                }
                            }

                            if (i < sizeField - 1)
                            {
                                if (slots[i + 1, j + 1].typeOfSlot != TypeOfSlot.Barrier && !slotHavePlanet[i + 1, j] && !slotHavePlanet[i + 1, j + 1])
                                {

                                    if (i > 0)
                                    {
                                        if (slots[i - 1, j + 1].typeOfSlot != TypeOfSlot.Barrier && !slotHavePlanet[i - 1, j] && !slotHavePlanet[i - 1, j + 1])
                                        {
                                            if (slotPlanetsList[i - 1, j + 1].Count < slotPlanetsList[i + 1, j + 1].Count)
                                            {
                                                slotPlanetsList[i - 1, j + 1].Add(new PlanetPosition(i, j));
                                                slotHavePlanet[i, j] = false;
                                                slotHavePlanet[i - 1, j + 1] = true;
                                                inverse = !inverse;
                                                FindWaysMovementPlanetsDown();
                                                return;
                                            }
                                        }
                                    }
                                    slotPlanetsList[i + 1, j + 1].Add(new PlanetPosition(i, j));
                                    slotHavePlanet[i, j] = false;
                                    slotHavePlanet[i + 1, j + 1] = true;
                                    inverse = !inverse;
                                    FindWaysMovementPlanetsDown();
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Выбрать слот для обмена
    /// </summary>
    /// <param name="slotSelect">Выбранный слот</param>
    public void SelectSlotToExchange(Slot slotSelect)
    {
        if (!CanPlanetMove)
            return;

        if (firstSelectSlotToExchange == null)
        {
            firstSelectSlotToExchange = slotSelect;
        }
        else
        {
            if (firstSelectSlotToExchange != slotSelect)
            {
                if ((firstSelectSlotToExchange.x == slotSelect.x && Mathf.Abs(firstSelectSlotToExchange.y - slotSelect.y) == 1) || (firstSelectSlotToExchange.y == slotSelect.y && Mathf.Abs(firstSelectSlotToExchange.x - slotSelect.x) == 1))
                {
                    ExchangePlanet(firstSelectSlotToExchange, slotSelect);
                }
                firstSelectSlotToExchange = null;
            }

            
        }
    }

    /// <summary>
    /// Обмен планетами
    /// </summary>
    /// <param name="firstSlot">Первый слот</param>
    /// <param name="secondSlot">Второй слот</param>
    private void ExchangePlanet(Slot firstSlot, Slot secondSlot)
    {
        if (firstSlot.planet != null && secondSlot.planet != null)
        {
            CanPlanetMove = false;
            Planet firstPlanet = firstSlot.planet;
            Planet secondPlanet = secondSlot.planet;
            if (CheckPossibleCombination(secondSlot, firstPlanet) || CheckPossibleCombination(firstSlot, secondPlanet))
            {
                moveCounter++;
                planetsMoveCounter[secondSlot.x, secondSlot.y] = moveCounter;
                firstPlanet.MoveToSlot(secondSlot, secondPlanet);
                moveCounter++;
                planetsMoveCounter[firstSlot.x, firstSlot.y] = moveCounter;
                secondPlanet.MoveToSlot(firstSlot, firstPlanet);
            }
            else
            {
                firstPlanet.ExchangePlanet(secondSlot);
                secondPlanet.ExchangePlanet(firstSlot);
            }
        }
    }

    /// <summary>
    /// Проверка на возможность комбинации
    /// </summary>
    /// <param name="slot">Слот</param>
    /// <param name="planet">Планета</param>
    /// <returns>Возвращает true если комбинация возможна, false если нет</returns>
    private bool CheckPossibleCombination(Slot slot, Planet planet)
    {
        if (slot == null)
            return false;
        if (slot == planet)
            return false;

        if (planet.CountBonus() != 0)
        {
            if (planet.HaveStarBonus())
            {
                return true;
            }
        }

        if (slot.x > 0 && slot.x < sizeField - 1)
        {
            if (CheckPossibleCombinationStep(slot, planet, -1, 1, 0, 0))
            {
                return true;
            }
        }

        if (slot.x > 1 && slot.x <= sizeField - 1)
        {
            if (CheckPossibleCombinationStep(slot, planet, -2, -1, 0, 0))
            {
                return true;
            }
        }

        if (slot.x >= 0 && slot.x < sizeField - 2)
        {
            if (CheckPossibleCombinationStep(slot, planet, 1, 2, 0, 0))
            {
                return true;
            }
        }

        if (slot.y > 0 && slot.y < sizeField - 1)
        {
            if (CheckPossibleCombinationStep(slot, planet, 0, 0, -1, 1))
            {
                return true;
            }
        }

        if (slot.y > 1 && slot.y <= sizeField - 1)
        {
            if (CheckPossibleCombinationStep(slot, planet, 0, 0, -2, -1))
            {
                return true;
            }
        }

        if (slot.y >= 0 && slot.y < sizeField - 2)
        {
            if (CheckPossibleCombinationStep(slot, planet, 0, 0, 1, 2))
            {
                return true;
            }
        }

        return false;
    }

    private bool CheckPossibleCombinationStep(Slot slot, Planet planet, int x1, int x2, int y1, int y2)
    {
        if (slots[slot.x + x1, slot.y + y1].planet != null && slots[slot.x + x2, slot.y + y2].planet != null)
        {
            if (slots[slot.x + x1, slot.y + y1].planet.typeOfPlanet == planet.typeOfPlanet
                && slots[slot.x + x2, slot.y + y2].planet.typeOfPlanet == planet.typeOfPlanet
                && slots[slot.x + x1, slot.y + y1].planet != planet
                && slots[slot.x + x2, slot.y + y2].planet != planet)
            {
                return true;
            }
        }

        return false;
    }

    private void DestroyCombination()
    {

        if (combination.Count == 0)
        {
            CanPlanetMove = true;
        }
        else
        {
            CanPlanetMove = false;
        }

        for (int i = 0; i < combination.Count; i++)
        {
            for (int j = 0; j < combination[i].Count; j++)
            {
                combination[i][j].DestroyPlanetInSlot();
            }
        }

        combination.Clear();
    }

    public void CheckFieldForCombination()
    {
        bool complite = true;
        for (int i = 0; i < sizeField; i++)
        {
            for (int j = 0; j < sizeField; j++)
            {
                if (slotPlanetsList[i, j].Count != 0)
                {
                    complite = false;
                }
            }
        }
        if (complite && moveComplite)
        {
            combinationFound = true;
            FindCombination();
        }
    }

    /// <summary>
    /// Поиск комбинаций
    /// </summary>
    private void FindCombination()
    {
        for (int i = 0; i < sizeField; i++)
        {
            for (int j = 0; j < sizeField; j++)
            {
                planetsInCombination[i, j] = false;
            }
        }

        for (int i = 0; i < sizeField; i++)
        {
            for (int j = 0; j < sizeField; j++)
            {
                planetsAddInCombination[i, j] = false;
            }
        }

        //combination.Clear();

        int minPlanetsMoveCount = 0;
        Slot bonusSlot = slots[0,0];

        for (int i = 0; i < sizeField; i++)
        {
            for (int j = 0; j < sizeField; j++)
            {
                List<Slot> combo = new List<Slot>();
                combo = FindCombinationFromSlot(slots[i, j]);
                if (combo.Count >= 1)
                {
                    switch (combo.Count)
                    {
                        case 1:
                            {
                                combination.Add(combo);
                                break;
                            }
                        case 2:
                            {
                                combination.Add(combo);
                                break;
                            }
                        case 3:
                            {
                                combination.Add(combo);
                                break;
                            }
                        case 4:
                            {
                                minPlanetsMoveCount = 0;
                                foreach (Slot slot in combo)
                                {
                                    if (planetsMoveCounter[slot.x, slot.y] >= minPlanetsMoveCount)
                                    {
                                        minPlanetsMoveCount = planetsMoveCounter[slot.x, slot.y];
                                        bonusSlot = slot;
                                    }
                                }
                                if (bonusSlot.planet.CountBonus() == 0)
                                    combo.Remove(bonusSlot);
                                BonusController.AddLineBonus(bonusSlot);
                                bonusSlot.DestroyEffectActivate();
                                combination.Add(combo);
                                break;
                            }
                        case 5:
                            {
                                minPlanetsMoveCount = 0;
                                boxBonus = false;

                                if ((combo[0].x == combo[1].x))
                                {
                                    foreach (Slot slot in combo)
                                    {
                                        if (planetsMoveCounter[slot.x, slot.y] >= minPlanetsMoveCount)
                                        {
                                            minPlanetsMoveCount = planetsMoveCounter[slot.x, slot.y];
                                            bonusSlot = slot;
                                        }
                                        if (combo[0].x != slot.x)
                                            boxBonus = true;
                                    }
                                }
                                else
                                {
                                    foreach (Slot slot in combo)
                                    {
                                        if (planetsMoveCounter[slot.x, slot.y] >= minPlanetsMoveCount)
                                        {
                                            minPlanetsMoveCount = planetsMoveCounter[slot.x, slot.y];
                                            bonusSlot = slot;
                                        }
                                        if (combo[0].y != slot.y)
                                            boxBonus = true;
                                    }
                                }
                                
                                if (bonusSlot.planet.CountBonus() == 0)
                                    combo.Remove(bonusSlot);

                                if (boxBonus)
                                    BonusController.AddBoxFirstBonus(bonusSlot);
                                else
                                    BonusController.AddStarBonus(bonusSlot);

                                bonusSlot.DestroyEffectActivate();
                                combination.Add(combo);
                                break;
                            }
                        default:
                            {
                                minPlanetsMoveCount = 0;
                                boxBonus = false;

                                if ((combo[0].x == combo[1].x))
                                {
                                    foreach (Slot slot in combo)
                                    {
                                        if (planetsMoveCounter[slot.x, slot.y] >= minPlanetsMoveCount)
                                        {
                                            minPlanetsMoveCount = planetsMoveCounter[slot.x, slot.y];
                                            bonusSlot = slot;
                                        }
                                        if (combo[0].x != slot.x)
                                            boxBonus = true;
                                    }
                                }
                                else
                                {
                                    foreach (Slot slot in combo)
                                    {
                                        if (planetsMoveCounter[slot.x, slot.y] >= minPlanetsMoveCount)
                                        {
                                            minPlanetsMoveCount = planetsMoveCounter[slot.x, slot.y];
                                            bonusSlot = slot;
                                        }
                                        if (combo[0].y != slot.y)
                                            boxBonus = true;
                                    }
                                }

                                if (bonusSlot.planet.CountBonus() == 0)
                                    combo.Remove(bonusSlot);

                                if (boxBonus)
                                    BonusController.AddBoxFirstBonus(bonusSlot);
                                else
                                    BonusController.AddStarBonus(bonusSlot);


                                bonusSlot.DestroyEffectActivate();
                                combination.Add(combo);
                                break;
                            }
                    }
                }
            }
        }

        DestroyCombination();

        //CanPlanetMove = true;

        moveCounter = 0;

        for (int i = 0; i < sizeField; i++)
        {
            for (int j = 0; j < sizeField; j++)
            {
                planetsMoveCounter[i,j] = 0;
            }
        }
    }

    List<Slot> FindCombinationFromSlot(Slot slot)
    {
        List<Slot> slotsInCombination = new List<Slot>();

        if (slot.planet != null && slot.planet.HaveStarBonus() && slot.planet.typeOfPlanet != TypeOfPlanet.Star)
        {
            planetsAddInCombination[slot.x, slot.y] = true;
            planetsInCombination[slot.x, slot.y] = true;
            slotsInCombination.Add(slot);
        }

        if (planetsInCombination[slot.x, slot.y] == false && slot.planet != null)
        {
            if (slot.x > 0 && slot.x < sizeField - 1)
            {
                slotsInCombination.AddRange(FindCombinationStep(slot, -1, 1, 0, 0));
            }

            if (slot.x > 1 && slot.x < sizeField)
            {
                slotsInCombination.AddRange(FindCombinationStep(slot, -2, -1, 0, 0));
            }

            if (slot.x >=0 && slot.x < sizeField-2)
            {
                slotsInCombination.AddRange(FindCombinationStep(slot, 1, 2, 0, 0));
            }

            if (slot.y > 0 && slot.y < sizeField - 1)
            {
                slotsInCombination.AddRange(FindCombinationStep(slot, 0, 0, -1, 1));
            }

            if (slot.y > 1 && slot.y < sizeField)
            {
                slotsInCombination.AddRange(FindCombinationStep(slot, 0, 0, -2, -1));
            }

            if (slot.y >= 0 && slot.y < sizeField - 2)
            {
                slotsInCombination.AddRange(FindCombinationStep(slot, 0, 0, 1, 2));
            }
        }
        return slotsInCombination;
    }

    List<Slot> FindCombinationStep(Slot slot, int x1, int x2, int y1, int y2)
    {
        List<Slot> slotsInCombination = new List<Slot>();
        if (slots[slot.x + x1, slot.y + y1].planet != null && slots[slot.x + x2, slot.y + y2].planet != null)
        {
            if (slots[slot.x + x1, slot.y + y1].planet.typeOfPlanet == slot.planet.typeOfPlanet && slots[slot.x + x2, slot.y + y2].planet.typeOfPlanet == slot.planet.typeOfPlanet)
            {
                if (!planetsAddInCombination[slot.x, slot.y])
                    slotsInCombination.Add(slot);

                if (!planetsAddInCombination[slot.x + x1, slot.y + y1])
                    slotsInCombination.Add(slots[slot.x + x1, slot.y + y1]);

                if (!planetsAddInCombination[slot.x + x2, slot.y + y2])
                    slotsInCombination.Add(slots[slot.x + x2, slot.y + y2]);

                planetsAddInCombination[slot.x, slot.y] = true;
                planetsAddInCombination[slot.x + x1, slot.y + y1] = true;
                planetsAddInCombination[slot.x + x2, slot.y + y2] = true;

                planetsInCombination[slot.x, slot.y] = true;

                slotsInCombination.AddRange(FindCombinationFromSlot(slots[slot.x + x1, slot.y + y1]));
                planetsInCombination[slot.x + x1, slot.y + y1] = true;

                slotsInCombination.AddRange(FindCombinationFromSlot(slots[slot.x + x2, slot.y + y2]));
                planetsInCombination[slot.x + x2, slot.y + y2] = true;

            }
        }
        return slotsInCombination;
    }

    [ContextMenu("Check")]
    public void CheckFieldForMove()
    {
        for (int i = 0; i < sizeField; i++)
        {
            for (int j = 0; j < sizeField; j++)
            {
                slotHavePlanet[i, j] = slots[i, j].planet != null;
                slotPlanetsList[i, j].Clear();
            }
        }

        FindWaysMovementPlanetsDown();
    }

    bool MovePlanets()
    {
        planetDontMoveMovePlanets = true;
        for (int j = sizeField - 1; j >= 0; j--)
        {
            for (int i = 0; i < sizeField; i++)
            {
                if (slotPlanetsList[i, j].Count != 0)
                {
                    planetDontMoveMovePlanets = false;
                    moveComplite = false;
                    combinationFound = false;
                    CanPlanetMove = false;
                    if (j - slotPlanetsList[i, j][0].y == 1)
                    {
                        if (i == slotPlanetsList[i, j][0].x || slotPlanetsList[slotPlanetsList[i, j][0].x, j].Count == 0 || (slotPlanetsList[slotPlanetsList[i, j][0].x, j].Count != 0 ? (slotPlanetsList[slotPlanetsList[i, j][0].x, j][0] != slotPlanetsList[i, j][0]) : true))
                        {
                            if (slots[i, j].typeOfSlot != TypeOfSlot.Generator)
                            {
                                if (slots[slotPlanetsList[i, j][0].x, slotPlanetsList[i, j][0].y].planet != null && slots[i, j].planet == null)
                                {
                                    if (slots[slotPlanetsList[i, j][0].x, slotPlanetsList[i, j][0].y].planet.slot != null)
                                    {
                                        selectPlanetMovePlanets = slots[slotPlanetsList[i, j][0].x, slotPlanetsList[i, j][0].y].planet;

                                        if (j + 1 < sizeField)
                                        {
                                            if (slotPlanetsList[i, j + 1].Count != 0)
                                            {
                                                if (slotPlanetsList[i, j + 1].Exists(x => x.x == i && x.y == j))
                                                {
                                                    moveCounter++;
                                                    planetsMoveCounter[i, j] = moveCounter;
                                                    selectPlanetMovePlanets.MoveToSlot(slots[i, j]);
                                                    slotPlanetsList[i, j].Remove(slotPlanetsList[i, j][0]);
                                                    return moveComplite;
                                                }
                                            }

                                            if (i - 1 >= 0)
                                            {
                                                if (slotPlanetsList[i - 1, j + 1].Count != 0)
                                                {
                                                    if (slotPlanetsList[i - 1, j + 1].Exists(x => x.x == i && x.y == j))
                                                    {
                                                        moveCounter++;
                                                        planetsMoveCounter[i, j] = moveCounter;
                                                        selectPlanetMovePlanets.MoveToSlot(slots[i, j]);
                                                        slotPlanetsList[i, j].Remove(slotPlanetsList[i, j][0]);
                                                        return moveComplite;
                                                    }
                                                }
                                            }

                                            if (i + 1 < sizeField)
                                            {
                                                if (slotPlanetsList[i + 1, j + 1].Count != 0)
                                                {
                                                    if (slotPlanetsList[i + 1, j + 1].Exists(x => x.x == i && x.y == j))
                                                    {
                                                        moveCounter++;
                                                        planetsMoveCounter[i, j] = moveCounter;
                                                        selectPlanetMovePlanets.MoveToSlot(slots[i, j]);
                                                        slotPlanetsList[i, j].Remove(slotPlanetsList[i, j][0]);
                                                        return moveComplite;
                                                    }
                                                }
                                            }

                                            moveCounter++;
                                            planetsMoveCounter[i, j] = moveCounter;
                                            selectPlanetMovePlanets.MoveToSlot(slots[i, j], true);
                                            slotPlanetsList[i, j].Remove(slotPlanetsList[i, j][0]);
                                            return moveComplite;
                                        }
                                        else
                                        {
                                            moveCounter++;
                                            planetsMoveCounter[i, j] = moveCounter;
                                            selectPlanetMovePlanets.MoveToSlot(slots[i, j]);
                                            slotPlanetsList[i, j].Remove(slotPlanetsList[i, j][0]);
                                            return moveComplite;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (slots[i, j].planet == null)
                                {
                                    countMovePlanets = Random.Range(0, planetGenerateList.Count);
                                    selectPlanetMovePlanets = GeneratePlanet(planetGenerateList[countMovePlanets], slots[i, j]);
                                    selectPlanetMovePlanets.transform.position += Vector3.up;
                                    if (j + 1 < sizeField)
                                    {
                                        if (slotPlanetsList[i, j].Count != 0)
                                        {
                                            if (slotPlanetsList[i, j].Exists(x => x.x == i && x.y == j - 1))
                                            {
                                                moveCounter++;
                                                planetsMoveCounter[i, j] = moveCounter;
                                                selectPlanetMovePlanets.MoveToSlot(slots[i, j]);
                                                slotPlanetsList[i, j].Remove(slotPlanetsList[i, j][0]);
                                                return moveComplite;
                                            }
                                        }

                                        if (i - 1 >= 0)
                                        {
                                            if (slotPlanetsList[i - 1, j + 1].Count != 0)
                                            {
                                                if (slotPlanetsList[i - 1, j + 1].Exists(x => x.x == i && x.y == j))
                                                {
                                                    moveCounter++;
                                                    planetsMoveCounter[i, j] = moveCounter;
                                                    selectPlanetMovePlanets.MoveToSlot(slots[i, j]);
                                                    slotPlanetsList[i, j].Remove(slotPlanetsList[i, j][0]);
                                                    return moveComplite;
                                                }
                                            }
                                        }

                                        if (i + 1 < sizeField)
                                        {
                                            if (slotPlanetsList[i + 1, j + 1].Count != 0)
                                            {
                                                if (slotPlanetsList[i + 1, j + 1].Exists(x => x.x == i && x.y == j))
                                                {
                                                    moveCounter++;
                                                    planetsMoveCounter[i, j] = moveCounter;
                                                    selectPlanetMovePlanets.MoveToSlot(slots[i, j]);
                                                    slotPlanetsList[i, j].Remove(slotPlanetsList[i, j][0]);
                                                    return moveComplite;
                                                }
                                            }
                                        }

                                        moveCounter++;
                                        planetsMoveCounter[i, j] = moveCounter;
                                        selectPlanetMovePlanets.MoveToSlot(slots[i, j], true);
                                        slotPlanetsList[i, j].Remove(slotPlanetsList[i, j][0]);
                                        return moveComplite;
                                    }
                                    else
                                    {
                                        moveCounter++;
                                        planetsMoveCounter[i, j] = moveCounter;
                                        selectPlanetMovePlanets.MoveToSlot(slots[i, j]);
                                        slotPlanetsList[i, j].Remove(slotPlanetsList[i, j][0]);
                                        return moveComplite;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (slots[i, j].planet != null)
                    {
                        if (slots[i, j].planet.slot != slots[i, j])
                        {
                            combinationFound = false;
                            planetDontMoveMovePlanets = false;
                        }
                    }
                }
            }
        }

        moveComplite = planetDontMoveMovePlanets;
        return moveComplite;
    }

    // Update is called once per frame
    void Update()
    {

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            for (int i = 0; i < planetPrefabs.Count; i++)
            {
                planetPrefabs[i] = new PlanetPrefab(planetPrefabs[i], planetPrefabs[i].typeOfPlanet.ToString());
            }
        }
#endif    

        MovePlanets();

        if (moveComplite && !combinationFound)
        {
            CanPlanetMove = true;
            CheckFieldForCombination();
        }
    }
}

[System.Serializable]
public struct PlanetPrefab
{
    [HideInInspector]
    public string name;
    public TypeOfPlanet typeOfPlanet;
    public Sprite sprite;
    public GameObject prefab;

    public PlanetPrefab(PlanetPrefab clone, string _name)
    {
        name = _name;
        typeOfPlanet = clone.typeOfPlanet;
        sprite = clone.sprite;
        prefab = clone.prefab;
    }
}

public struct PlanetPosition
{
    public int x;
    public int y;

    public PlanetPosition(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    public static bool operator ==(PlanetPosition lhs, PlanetPosition rhs)
    {
        return lhs.x == rhs.x && lhs.y == rhs.y;        
    }

    public static bool operator !=(PlanetPosition lhs, PlanetPosition rhs)
    {
        return !(lhs.x == rhs.x && lhs.y == rhs.y);
    }
}
