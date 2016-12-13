﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class GameMenu : MonoBehaviour {

    public List<GameObject> disactiveObjects= new List<GameObject>();
    public GameObject gameMenu;
    public ScoreManager scoreManager;

    public void Pause()
    {
        gameMenu.SetActive(true);
        Time.timeScale = 0;

    }

    public void Continue()
    {
        Time.timeScale = 1;
        foreach (GameObject disactiveObject in disactiveObjects)
        {
            disactiveObject.SetActive(false);
        }
    }

    public void Reload()
    {
        Time.timeScale = 1;

        SceneManager.LoadScene(scoreManager.levelName);
    }

    public void Exit()
    {
        Time.timeScale = 1;

        SceneManager.LoadScene("Menu");
    }

}
