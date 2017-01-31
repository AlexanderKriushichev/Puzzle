using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour {

    [Header("Position")]
    public int x;
    public int y;
    [Header("Type of slot")]
    public TypeOfSlot typeOfSlot = TypeOfSlot.None;
    [Header("Planet")]
    public Planet planet;
    [HideInInspector]
    public SlotController slotController;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

public enum TypeOfSlot { None, Barrier, Generator}