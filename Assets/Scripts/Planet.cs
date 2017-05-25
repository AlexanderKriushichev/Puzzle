using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Planet : MonoBehaviour {

    [Header("Type of planet")]
    public TypeOfPlanet typeOfPlanet;

    /// <summary>
    /// Слот в котором находится планета
    /// </summary>
    [Header("Slot")]   
    public Slot slot;

    public AnimationCurve curveMove;

    private SpriteRenderer _spritePlanet;
    public SpriteRenderer spritePlanet 
    {
        get
        {
            if (_spritePlanet == null)
                _spritePlanet = GetComponent<SpriteRenderer>();
            return _spritePlanet;
        }
        set
        {
            _spritePlanet = value;
        }
    }

    private bool _haveArea = false;
    public bool haveArea
    {
        get
        {
            return _haveArea;
        }
        set
        {
            _haveArea = value;
        }
    }

    public bool dontDestroy = false;

    private List<TypeOfBonus> bonuses = new List<TypeOfBonus>();

    private float speedMove = 0.2f;

    private TypeLineBonus directionOfMove;

    private Slot slotToMove;

    private TypeOfBonus typeOfExPlanetBonus = TypeOfBonus.None;

	// Use this for initialization
	void Start () {
        //transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 360));
        spritePlanet = GetComponent<SpriteRenderer>();
	}

    public TypeLineBonus GetDirectionOfMove()
    {
        return directionOfMove;
    }

    public void SetDirectionOfMove(TypeLineBonus typeDir)
    {
        directionOfMove = typeDir;
    }

    public void SetBonus(TypeOfBonus bonus)
    {
        foreach (TypeOfBonus bns in bonuses)
        {
            if (bns == bonus)
                return;
        }
        bonuses.Add(bonus);
    }

    public bool HaveStarBonus()
    {
        foreach (TypeOfBonus bonus in bonuses)
        {
            if (bonus == TypeOfBonus.Star)
            {
                return true;
            }
        }
        return false;
    }

    public bool HaveLineBonus()
    {
        foreach (TypeOfBonus bonus in bonuses)
        {
            if (bonus == TypeOfBonus.HorizontalLine || bonus == TypeOfBonus.VerticalLine)
            {
                return true;
            }
        }
        return false;
    }

    public int CountBonus()
    {
        return bonuses.Count;
    }

    public void SetSprite(Sprite sprite)
    {
        if (spritePlanet == null)
            spritePlanet = GetComponent<SpriteRenderer>();
        spritePlanet.sprite = sprite;
    }

    public void MoveToSlot(Slot slotToMove)
    {
        directionOfMove = TypeLineBonus.Verical;

        if (slot.x == slotToMove.x)
            directionOfMove = TypeLineBonus.Verical;
        if (slot.y == slotToMove.y)
            directionOfMove = TypeLineBonus.Horizontal;

        if (slot != null)
            slot.planet = null;
        slot = null;
        slotToMove.planet = this;
        transform.DOMove(slotToMove.transform.position, speedMove).SetEase(Ease.Linear).OnUpdate(
            delegate
            {
                if (slot!=null)
                {
                    SlotController.CanPlanetMove = false;
                }
            }).OnComplete(() => OnCompliteMove(slotToMove));
    }

    public void MoveToSlot(Slot slotToMove, Planet exPlanet)
    {
        directionOfMove = TypeLineBonus.Verical;

        if (slot.x == slotToMove.x)
            directionOfMove = TypeLineBonus.Verical;
        if (slot.y == slotToMove.y)
            directionOfMove = TypeLineBonus.Horizontal;

        if (slot != null)
            slot.planet = null;
        slot = null;
        slotToMove.planet = this;
        transform.DOMove(slotToMove.transform.position, speedMove).SetEase(Ease.Linear).OnUpdate(
            delegate
            {
                if (slot != null)
                {
                    SlotController.CanPlanetMove = false;
                }
            }).OnComplete(delegate
            {
                if (HaveStarBonus())
                {
                    typeOfPlanet = exPlanet.typeOfPlanet;
                    if (exPlanet.CountBonus()!=0)
                        typeOfExPlanetBonus = exPlanet.bonuses[0];
                }
                OnCompliteMove(slotToMove);
            });
    }

    public void ExchangePlanet(Slot slotToMove)
    {
        transform.DOMove(slotToMove.transform.position, speedMove).SetEase(Ease.Linear).OnUpdate(
            delegate
            {
                if (slot != null)
                {
                    SlotController.CanPlanetMove = false;
                }
            }).OnComplete(() => transform.DOMove(slot.transform.position, speedMove).SetEase(Ease.Linear).OnComplete(
                delegate
                {
                    if (slot != null)
                    {
                        SlotController.CanPlanetMove = true;
                    }
                }));
    }

    public void MoveToSlot(Slot _slotToMove, bool lastSlot)
    {
        slot.planet = null;
        slot = null;
        slotToMove = _slotToMove;
        slotToMove.planet = this;
        if (lastSlot)
        {
            transform.DOMove(slotToMove.transform.position, speedMove).SetEase(curveMove).OnComplete(() => OnCompliteMove(slotToMove));
        }
        else
        {
            transform.DOMove(slotToMove.transform.position, speedMove).SetEase(Ease.Linear).OnComplete(() => OnCompliteMove(slotToMove));
        }
    }

    public void ActivateBonus()
    {
        bonuses.Sort();
        foreach (TypeOfBonus bonus in bonuses)
        {
            if (slot != null)
            {
                TypeOfBonus typeBonus = bonus;
                bonuses.Remove(bonus);
                BonusController.ActivateBonus(slot.x, slot.y, typeOfPlanet, typeBonus, typeOfExPlanetBonus);
                break;
            }
            else
            {
                if (slotToMove != null)
                {
                    TypeOfBonus typeBonus = bonus;
                    bonuses.Remove(bonus);
                    BonusController.ActivateBonus(slotToMove.x, slotToMove.y, typeOfPlanet, typeBonus, typeOfExPlanetBonus);
                    break;
                }
            }
        }
    }

    void OnDestroy()
    {
        SlotController.CanPlanetMove = true;
        StopCoroutine(DestoyPlanetWhenCanMove());
    }

    public void DestroyWhenCanMove()
    {
        StartCoroutine(DestoyPlanetWhenCanMove());
    }

    IEnumerator DestoyPlanetWhenCanMove()
    {

        while (!SlotController.CanPlanetMove)
        {
            yield return null;
        }

        dontDestroy = false;
        Debug.Log(1);
        if (slot != null)
        {
            slot.DestroyPlanetInSlot();
        }
        else
        {
            slotToMove.DestroyPlanetInSlot();
        }
    }

    public void OnCompliteMove(Slot slotToMove)
    {
        transform.SetParent(slotToMove.transform);
        slotToMove.planet = this;
        slot = slotToMove;
        slot.slotController.CheckFieldForCombination();
    }

}

public enum TypeOfPlanet { Green, Blue, Red, White, Purple, Yellow, Star }