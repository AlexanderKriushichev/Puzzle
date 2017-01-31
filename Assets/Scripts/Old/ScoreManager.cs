using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class ScoreManager : MonoBehaviour {

    public static int score;

    public string levelName;
    public int levelID;

    public Text textScore;

    public Text textTarget;

    public Text textCountMove;

    public ProgressScore progressScore;

    public RectTransform endPanel;
    public Text endPanelText;

    public RectTransform menu;

    public float targetScore;

    public  int m_targetDeathStar;

    private static int _targetDeathStar;
    public static int targetDeathStar
    {
        get
        {
            return _targetDeathStar;
        }
        set 
        {
            if (value >= 0)
                _targetDeathStar = value;
            else
                _targetDeathStar = 0;
        }
    }

    [SerializeField]
    private Field field;

    [SerializeField]
    private int _countMove;

    public static int countMove;

    public SpaceMan spaceMan;

    static List<int> listScore = new List<int>();

    float timer = 0.1f;

    bool endShowPanel = false;

    void Start()
    {
        targetDeathStar = m_targetDeathStar;
        countMove = _countMove;
        score = 0;
        textTarget.text = "Цель: " + targetScore.ToString();
        textCountMove.text = "Ходы: " + countMove.ToString();
    }

    static public void AddScore(int _score)
    {
        listScore.Add(_score);
    }

    static public void MakeMove()
    {
        countMove--;
    }

    static public void AddDeathStar()
    {
        Debug.Log(targetDeathStar);
        targetDeathStar--;
    }

    void Update()
    {
        if (listScore.Count != 0)
        {
            if (timer >= 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                progressScore.SetScore((score / targetScore) * 0.325f);
                score += listScore[0];
                listScore.RemoveAt(0);
                timer = 0.1f;
            }
        }
        textScore.text = "Счёт: " + score;
        textCountMove.text = "Ходы: " + countMove.ToString();

        if (countMove == 0 && !endShowPanel && field.CheckMove() && !field.inRotate && field.moveCrystals.Count == 0 && field.combinations.Count == 0)
        {
            endShowPanel = true;
            ShowEndPanel();
        }
    }

    void ShowEndPanel()
    {
        endPanel.DOScale(Vector3.one, 1).SetEase(Ease.Flash).OnComplete(delegate { endPanel.DOScale(Vector3.zero, 0.5f).OnComplete(()=>menu.gameObject.SetActive(true));  });

        if (score >= targetScore&& targetDeathStar == 0)
        {
            endPanelText.text = "Молодец!";
            spaceMan.SetSuccessedText();
        }
        else
        {
            endPanelText.text = "Провал!";
            spaceMan.SetUnsuccessedText();

        }
    }

}
