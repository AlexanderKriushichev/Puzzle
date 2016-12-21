using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelLoad : MonoBehaviour
{

    public static int levelIDRun;
    public static int scoreLevel;

    public string levelName;
    public int levelID;
    public float score;
    public float targetScore;

    public GameObject firstStar;
    public GameObject secondStar;
    public GameObject thirdStar;

    private int countStar = 0;

    public LevelLoad prevLevel;

    public ElementLevelRotate elements;
    public LineRenderer line;
    public TextMesh textMesh;

    public bool Active { get; set; }

    public static Dictionary<string, int> levelsScore = new Dictionary<string, int>();

    void Start()
    {
        GetCountStars();

        if (prevLevel != null)
        {
            if (prevLevel.GetCountStars() != 0)
            {
                Active = true;
            }
            else
            {
                Active = false;
            }
        }
        else
        {
            Active = true;
        }

        ActiveSprite();
    }

    void OnMouseDown()
    {
        if (Active)
        {
            SceneManager.LoadScene(levelName);
            levelIDRun = levelID;
        }
    }

    public int GetCountStars()
    {
        int count = 0;
        if (levelsScore.ContainsKey(levelName))
            score = levelsScore[levelName];
        else
            score = 0;

        float coef = (score / targetScore) * 0.325f;
        if (coef >= 0.325f)
        {
            firstStar.SetActive(true);
            count++;
        }
        else
        {
            firstStar.SetActive(false);
        }

        if (coef >= 0.665)
        {
            secondStar.SetActive(true);
            count++;
        }
        else
        {
            secondStar.SetActive(false);
        }

        if (coef >= 1)
        {
            thirdStar.SetActive(true);
            count++;
        }
        else
        {
            thirdStar.SetActive(false);
        }

        return count;
    }

    void ActiveSprite()
    {
        elements.SetActive(Active);
        if (Active)
        {
            Color color = new Color(1, 1, 1, 1);
            if (line != null)
                line.SetColors(color, color);
            textMesh.color = color;
        }
        else
        {
            Color color = new Color(1, 1, 1, 0.3f);
            if (line != null)
                line.SetColors(color, color);
            textMesh.color = color;
        }
    }
}
