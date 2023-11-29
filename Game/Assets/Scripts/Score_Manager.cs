using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score_Manager : MonoBehaviour
{
    int score = 0;
    int tasksRevived = 0;
    int tasksCompleted = 0;

    TMP_Text scoreLabel;
    DB_Manager db;

    // Start is called before the first frame update
    void Start()
    {
        db = GameObject.Find("Main Camera").GetComponent<DB_Manager>();
        scoreLabel = GameObject.Find("lblScoreValue").GetComponent<TMP_Text>();
        scoreLabel.text = score.ToString();
    }

    public void saveScore()
    {
        PlayerPrefs.SetInt("score",score);
        PlayerPrefs.SetInt("tasksRevived",tasksRevived);
        PlayerPrefs.SetInt("tasksCompleted",tasksCompleted);
        
        if(PlayerPrefs.GetInt("offline", 0) == 0)
        {
            StartCoroutine(db.AddUserScores(PlayerPrefs.GetInt("user_id")));
        }
    }

    public void resetScore()
    {
        score = 0;
        tasksRevived = 0;
        tasksCompleted = 0;

        PlayerPrefs.SetInt("score",score);
        PlayerPrefs.SetInt("tasksRevived",tasksRevived);
        PlayerPrefs.SetInt("tasksCompleted",tasksCompleted);
        
        if(PlayerPrefs.GetInt("offline", 0) == 0)
        {
            StartCoroutine(db.AddUserScores(PlayerPrefs.GetInt("user_id")));
        }
    }

    void Update()
    {
        scoreLabel.text = score.ToString();
    }

    public void addToScore(int points)
    {
        score += points;
    }

    public void addToTasksRevived(int num)
    {
        tasksRevived += num;
    }

    public void addToTasksCompleted(int num)
    {
        tasksCompleted += num;
    }

    public int getScore()
    {
        return score;
    }

    public int getTasksRevived()
    {
        return tasksRevived;
    }

    public int getTasksCompleted()
    {
        return tasksCompleted;
    }

    public void deductScore(int points)
    {
        score -= points;
    }
}
