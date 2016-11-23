using UnityEngine;
using System.Collections;

public class BonusManager : MonoBehaviour {

    public GameObject lizerPrefab;
    public Field field;
    public static BonusManager bonusManager;

	// Use this for initialization
	void Start () {
        bonusManager = this;
	}

    public static GameObject GetLizerPrefab()
    {
        return bonusManager.lizerPrefab;
    }

    public static Field GetField()
    {
        return bonusManager.field;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
