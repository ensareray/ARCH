﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalGameUI : GameUI
{
    public TextMeshProUGUI timeText,scoreText,startText;
    public Button nextButton;
    SurvivalGameManager survivalManager;
    void Start()
    {
        survivalManager = FindObjectOfType<SurvivalGameManager>();
    }   
    void Update()
    {
        if(survivalManager.isGameStarted == false)
        {
            startText.gameObject.SetActive(true);
            if(Input.GetMouseButton(0))
            {
                survivalManager.StartOrGoNextGame();
                startText.gameObject.SetActive(false);
            }
        }
        if(!survivalManager.gameStopped && survivalManager.isGameStarted)
        {
            survivalManager.gameTime += Time.deltaTime; 
            timeText.text = survivalManager.gameTime.ToString("#.#");   
        }
    }
    public void UpdateScoreText(int score)
    {
        //scoreText.text = score.ToString();
        scoreText.color = UnityEngine.Random.ColorHSV(0,1,1,1,1,1);
        StartCoroutine( UpdateScoreEnumerator(score) );
    }
    public void SetUIOnGameEnded()
    {
        nextButton.gameObject.SetActive(true);
    }
    public void OnNextButtonClick()
    {
        survivalManager.CleanGame();
        survivalManager.SetRoom();
        survivalManager.isGameStarted = false;
        nextButton.gameObject.SetActive(false);
    }
    IEnumerator UpdateScoreEnumerator(int score)
    {
        int cScore = Convert.ToInt32(scoreText.text);
        while (cScore < score) {
            cScore += 2;
            scoreText.text = cScore.ToString();
            yield return null;
        }
    }
}