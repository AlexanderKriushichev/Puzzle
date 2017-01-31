using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

    public bool check = false;


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
                if (slots[i, j].typeOfSlot == TypeOfSlot.None && slots[i, j].planet == null)
                {
                    TypeOfPlanet initPlanetType = GenerateTypePlanet(i, j);
                    PlanetPrefab initPlanetPrefab = planetPrefabs.Find(x => x.typeOfPlanet == initPlanetType);

                    GameObject initPlanet = (GameObject)Instantiate(initPlanetPrefab.prefab, slots[i, j].transform);
                    initPlanet.transform.position = slots[i, j].transform.position;
                    slots[i, j].planet = initPlanet.GetComponent<Planet>();
                    slots[i, j].planet.typeOfPlanet = initPlanetPrefab.typeOfPlanet;
                    slots[i, j].planet.slot = slots[i, j];
                    if (initPlanetPrefab.sprite != null)
                        slots[i, j].planet.SetSprite(initPlanetPrefab.sprite);
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
    /// Поиск пути передвижения планет вниз
    /// </summary>
    private void FindWaysMovementPlanetsDown()
    {

        for (int j = sizeField - 1; j >= 0; j--)
        {
            for (int i = 0; i < sizeField; i++)
            {
                if (slots[i, j].typeOfSlot != TypeOfSlot.Barrier && slotHavePlanet[i, j])
                {
                    if (j < sizeField - 1)
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
                                            FindWaysMovementPlanetsDown();
                                            return;
                                        }
                                    }
                                }
                                slotPlanetsList[i - 1, j + 1].Add(new PlanetPosition(i, j));
                                slotHavePlanet[i, j] = false;
                                slotHavePlanet[i - 1, j + 1] = true;
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
                                            FindWaysMovementPlanetsDown();
                                            return;
                                        }
                                    }
                                }
                                slotPlanetsList[i + 1, j + 1].Add(new PlanetPosition(i, j));
                                slotHavePlanet[i, j] = false;
                                slotHavePlanet[i + 1, j + 1] = true;
                                FindWaysMovementPlanetsDown();
                                return;
                            }
                        }
                    }
                }
            }
        }
    }

    [ContextMenu("Check")]
    void CheckField()
    {
        for (int i = 0; i < sizeField; i++)
        {
            for (int j = 0; j < sizeField; j++)
            {
                slotHavePlanet[i, j] = slots[i, j].planet != null;
            }
        }
        FindWaysMovementPlanetsDown();
    }

    void MovePlanets()
    {
        for (int i = 0; i < sizeField; i++)
        {
            for (int j = 0; j < sizeField; j++)
            {
                if (slotPlanetsList[i, j].Count != 0)
                {
                    if (j - slotPlanetsList[i, j][0].y == 1)
                    {
                        if (slots[slotPlanetsList[i, j][0].x, slotPlanetsList[i, j][0].y].planet != null && slots[i, j].planet == null)
                        {
                            if (slots[slotPlanetsList[i, j][0].x, slotPlanetsList[i, j][0].y].planet.slot != null)
                            {
                                Planet selectPlanet = slots[slotPlanetsList[i, j][0].x, slotPlanetsList[i, j][0].y].planet;

                                if (j + 1 < sizeField)
                                {
                                    if (slotPlanetsList[i, j + 1].Count != 0)
                                    {
                                        if (slotPlanetsList[i, j + 1].Exists(x => x.x == i && x.y == j))
                                        {
                                            selectPlanet.MoveToSlot(slots[i, j]);
                                            slotPlanetsList[i, j].Remove(slotPlanetsList[i, j][0]);
                                            return;
                                        }
                                    }

                                    if (i - 1 >= 0)
                                    {
                                        if (slotPlanetsList[i - 1, j + 1].Count != 0)
                                        {
                                            if (slotPlanetsList[i - 1, j + 1].Exists(x => x.x == i && x.y == j))
                                            {
                                                selectPlanet.MoveToSlot(slots[i, j]);
                                                slotPlanetsList[i, j].Remove(slotPlanetsList[i, j][0]);
                                                return;
                                            }
                                        }
                                    }

                                    if (i + 1 < sizeField)
                                    {
                                        if (slotPlanetsList[i + 1, j + 1].Count != 0)
                                        {
                                            if (slotPlanetsList[i + 1, j + 1].Exists(x => x.x == i && x.y == j))
                                            {
                                                selectPlanet.MoveToSlot(slots[i, j]);
                                                slotPlanetsList[i, j].Remove(slotPlanetsList[i, j][0]);
                                                return;
                                            }
                                        }
                                    }

                                    selectPlanet.MoveToSlot(slots[i, j], true);
                                    slotPlanetsList[i, j].Remove(slotPlanetsList[i, j][0]);
                                    return;
                                }
                                else
                                {
                                    selectPlanet.MoveToSlot(slots[i, j]);
                                    slotPlanetsList[i, j].Remove(slotPlanetsList[i, j][0]);
                                    return;
                                }


                            }
                        }
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

#if UNITY_EDITOR
        for (int i = 0; i < planetPrefabs.Count; i++)
        {
            planetPrefabs[i] = new PlanetPrefab(planetPrefabs[i], planetPrefabs[i].typeOfPlanet.ToString());
        }
#endif    
          

        MovePlanets();


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
