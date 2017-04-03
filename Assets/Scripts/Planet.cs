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

    private SpriteRenderer spritePlanet;

    private List<TypeOfBonus> bonuses = new List<TypeOfBonus>();

    private float speedMove = 0.2f;

    private TypeLineBonus directionOfMove;

    private Slot slotToMove;

	// Use this for initialization
	void Start () {
        //transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 360));
        spritePlanet = GetComponent<SpriteRenderer>();
	}

    public TypeLineBonus GetDirectionOfMove()
    {
        return directionOfMove;
    }

    public void SetBonus(TypeOfBonus bonus)
    {
        bonuses.Add(bonus);
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
                BonusController.ActivateBonus(slot.x, slot.y, typeOfPlanet, typeBonus);
                break;
            }
            else
            {
                if (slotToMove != null)
                {
                    TypeOfBonus typeBonus = bonus;
                    bonuses.Remove(bonus);
                    BonusController.ActivateBonus(slotToMove.x, slotToMove.y, typeOfPlanet, typeBonus);
                    break;
                }
            }
        }
    }

    void OnDestroy()
    { 

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

public enum TypeOfPlanet { Green, Blue, Red, White, Purple, Yellow }