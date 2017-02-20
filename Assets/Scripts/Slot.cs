﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{

    [Header("Position")]
    public int x;
    public int y;
    [Header("Type of slot")]
    public TypeOfSlot typeOfSlot = TypeOfSlot.None;
    [Header("Planet")]
    public Planet planet;
    [HideInInspector]
    public SlotController slotController;

    private Vector2 startPositionOfMouse;
    private Vector2 endPositionOfMouse;
    private Vector2 moveVector;

    void OnMouseDown()
    {
        if (slotController.firstSelectSlotToExchange == null)
            slotController.SelectSlotToExchange(this);
        startPositionOfMouse = Input.mousePosition;
    }

    void OnMouseUp()
    {
        if (slotController.firstSelectSlotToExchange != null)
            slotController.SelectSlotToExchange(this);
    }

    void OnMouseExit()
    {
        if (slotController.firstSelectSlotToExchange == this && Input.GetMouseButton(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject == gameObject)
                    return;
            }

            endPositionOfMouse = Input.mousePosition;
            moveVector = endPositionOfMouse - startPositionOfMouse;

            if (Mathf.Abs(moveVector.x) > Mathf.Abs(moveVector.y))
            {
                if (moveVector.x > 0)
                {
                    if (x + 1 < SlotController.sizeField)
                    {
                        slotController.SelectSlotToExchange(slotController.slots[x + 1, y]);
                    }
                }
                else
                {
                    if (x - 1 >= 0)
                    {
                        slotController.SelectSlotToExchange(slotController.slots[x - 1, y]);
                    }
                }
            }
            else
            {
                if (moveVector.y > 0)
                {
                    if (y - 1 >= 0)
                    {
                        slotController.SelectSlotToExchange(slotController.slots[x, y - 1]);
                    }
                }
                else
                {
                    if (y + 1 < SlotController.sizeField)
                    {
                        slotController.SelectSlotToExchange(slotController.slots[x, y + 1]);
                    }
                }
            }
        }
    }

}

public enum TypeOfSlot { None, Barrier, Generator}