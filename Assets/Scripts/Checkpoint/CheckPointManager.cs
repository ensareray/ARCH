﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    public List<GameObject> checkPointsPrefabList = new List<GameObject>();
    private float radius;
    public int checkPointCount,passedCheckpointCount=0;
    public List<CheckpointController> checkPoints = new List<CheckpointController>();
    SurvivalGameManager survivalManager;
    SurvivalGameUI survivalUI;
    [SerializeField] int checkPointWithGlass_StartLevel;
    [SerializeField] Transform checkpointParent;
    List<GameObject> checkPointsToCreate = new List<GameObject>();

    void Start()
    {
        survivalManager = FindObjectOfType<SurvivalGameManager>();
        survivalUI = FindObjectOfType<SurvivalGameUI>();
    }

    public void CreateCheckPoints()
    {
        radius = survivalManager.gameRadius-2;

        ChoosecheckPointCount();
        CheckpointsToCreate();
        for (int i = 0; i < checkPointCount; i++)
        {
            CheckpointController check = Instantiate( checkPointsToCreate[ChooseCheckpointType()], ChooseRandomLocation(), Quaternion.identity ).GetComponent<CheckpointController>();
            check.transform.SetParent(checkpointParent.transform);
            Color c =  Random.ColorHSV(0,1,1,1,1,1);

            check.up.material.SetColor("_EmissionColor", c);
            check.down.material.SetColor("_EmissionColor", c);

            if(check.rightParticle != null)
            {
                var col2 = check.rightParticle.colorOverLifetime;
                col2.enabled = true;
                Gradient grad = new Gradient();
                grad.SetKeys( new GradientColorKey[] { 
                    new GradientColorKey(c, 0.0f), new GradientColorKey(c, 1.0f) 
                    },
                    new GradientAlphaKey[] { 
                        new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) 
                        } );
                col2.color = grad;

                if(check.leftParticle != null)
                {
                    var col = check.leftParticle.colorOverLifetime;
                    col.enabled = true;
                    col.color = grad;
                }
            }
            checkPoints.Add(check);
        }
    }

    void CheckpointsToCreate()
    {

        checkPointsToCreate.Add(checkPointsPrefabList[0]);

        if(survivalManager.waveIndex >= checkPointWithGlass_StartLevel)
        {
            checkPointsToCreate.Add(checkPointsPrefabList[1]);
        }
    }
    int ChooseCheckpointType()
    {
        if(survivalManager.waveIndex < checkPointWithGlass_StartLevel)
            return 0;
        else
        {
            float val = Random.Range(0f,1f);
            if(val <= .65f)
            {
                return 0;
            }
            else if( val > .65f && val <= 1f)
            {
                return 1; 
            }
            else{
                return 0;
            }
        }
    }
    
    public void ChoosecheckPointCount()
    {
        if( survivalManager.waveIndex < 5 && radius <= 55 )
        {
            checkPointCount = UnityEngine.Random.Range(2,6);
        }
        else if( survivalManager.waveIndex < 5 && radius > 55 )
        {
            checkPointCount = UnityEngine.Random.Range(3,7);
        }
        else if( survivalManager.waveIndex > 5 && survivalManager.waveIndex <10 && radius <= 55 )
        {
            checkPointCount = UnityEngine.Random.Range(3,7);
        }
        else if( survivalManager.waveIndex > 5 && survivalManager.waveIndex <10 && radius > 55 )
        {
            checkPointCount = UnityEngine.Random.Range(5,9);
        }
        else if( survivalManager.waveIndex > 15 && radius <= 55 )
        {
            checkPointCount = UnityEngine.Random.Range(8,12);
        }
        else if( survivalManager.waveIndex > 15 && radius > 55 )
        {
            checkPointCount = UnityEngine.Random.Range(10,15);
        }
    }
    Vector2 ChooseRandomLocation()
    {
        var vector2 = Random.insideUnitCircle * radius;
        return new Vector2(transform.position.x + vector2.x,transform.position.y + vector2.y);
    }
    public void RemovePointFromlist(CheckpointController point)
    {
        passedCheckpointCount++;

        checkPoints.Remove(point);
        survivalManager.GetEnemyScore();
        if(checkPoints.Count == 0)
        {
            Debug.Log("All checkPoint Passed");
            survivalManager.CleanGame();
            survivalManager.SetRoom();
            survivalManager.CalculateScore();
            passedCheckpointCount = 0;
        }
        else
        {
            AddRingToAllCheckpoints();
        }
    }
    void AddRingToAllCheckpoints()
    {
        foreach (CheckpointController item in checkPoints)
        {
            item.AddRing(passedCheckpointCount);
        }
    }
}
