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

    private SpriteRenderer spritePlanet;

    public AnimationCurve curveMove;

    private float speedMove = 0.2f;

	// Use this for initialization
	void Start () {
        spritePlanet = GetComponent<SpriteRenderer>();
	}

    public void SetSprite(Sprite sprite)
    {
        if (spritePlanet == null)
            spritePlanet = GetComponent<SpriteRenderer>();
        spritePlanet.sprite = sprite;
    }

    public void MoveToSlot(Slot slotToMove)
    {
        if (slot != null)
            slot.planet = null;
        slot = null;
        slotToMove.planet = this;
        transform.DOMove(slotToMove.transform.position, speedMove).SetEase(Ease.Linear).OnComplete(() => OnCompliteMove(slotToMove));
    }

    public void ExchangePlanet(Slot slotToMove)
    {
        transform.DOMove(slotToMove.transform.position, speedMove).SetEase(Ease.Linear).OnComplete(() => transform.DOMove(slot.transform.position, speedMove).SetEase(Ease.Linear));
    }

    public void MoveToSlot(Slot slotToMove, bool lastSlot)
    {
        slot.planet = null;
        slot = null;
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

    public void OnCompliteMove(Slot slotToMove)
    {
        transform.SetParent(slotToMove.transform);
        slotToMove.planet = this;
        slot = slotToMove;
        slot.slotController.CheckFieldForCombination();
    }

}

public enum TypeOfPlanet { Green, Blue, Red, White, Purple, Yellow }