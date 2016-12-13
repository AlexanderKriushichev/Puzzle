using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelLoad : MonoBehaviour {

    public static int levelIDRun;
    public static int scoreLevel;

    public string levelName;
    public int levelID;
    public float score;
    public float targetScore;

    public GameObject firstStar;
    public GameObject secondStar;
    public GameObject thirdStar;


    void Start()
    {
        float coef = (score / targetScore) * 0.325f;
        if (coef >= 0.325f)
        {
            firstStar.SetActive(true);
        }

        if (coef >= 0.665)
        {
            secondStar.SetActive(true);
        }

        if (coef >= 1)
        {
            thirdStar.SetActive(true);
        }
    }

    void OnMouseDown()
    {
        SceneManager.LoadScene(levelName);
        levelIDRun = levelID;
    }
}
