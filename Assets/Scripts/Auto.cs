using UnityEngine;
using System.Collections;

public class Auto : MonoBehaviour {

    public Field field;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (field.findComplite && field.CheckMove() && field.combinations.Count == 0)
        {
            foreach (Cell cell in field.cells)
            {
                Cell target = field.FindPossibleCombination(cell);
                if (target != null)
                {
                    target.ExchangeCrystal(cell);
                    break;
                }
            }
        }
	}
}
