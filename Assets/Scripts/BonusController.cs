using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusController : MonoBehaviour {

    public static BonusController bonusController;

    public SlotController slotController;

    void Awake()
    {
        bonusController = this;
    }

    public static void HorizontalLineActivate(int x, int y)
    {
        
    }
}

public enum TypeOfBonus 
{
    HorizontalLine,
    VerticalLine,
    Box,
    Start
}
