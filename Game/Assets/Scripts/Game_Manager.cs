using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Game_Manager : MonoBehaviour
{
    Localiser l = new Localiser();
    DB_Manager db;
    Score_Manager sm;

    GameObject[] miniGames = new GameObject[3];
    GameObject[] miniTasksObjects = new GameObject[3];
    GameObject[] miniTasksButtons = new GameObject[3];
    TMP_Text[] miniTasksTimerLabel = new TMP_Text[3];
    TMP_Text getReadyMessage;
    GameObject getReadyWarning;
    public Sprite[] miniTasksSprite = new Sprite[3];
    bool[] isTaskActive = new bool[3];
    bool[] isTaskBroken = new bool[3];
    bool[] isTaskCompleted = new bool[3];
    int[] taskTime = new int[3];
    int[] taskGame = new int[3];

    int spawnTask;
    int gameSecondsLeft = 601;
    TMP_Text gameTimeLabel;

    // Start is called before the first frame update
    void Start()
    {
        gameTimeLabel = GameObject.Find("lblTime").GetComponent<TMP_Text>();
        db = GameObject.Find("Main Camera").GetComponent<DB_Manager>();
        sm = GameObject.Find("Main Camera").GetComponent<Score_Manager>();

        for (int i = 0; i < miniTasksObjects.Length; i++)
        {
            miniTasksObjects[i] = GameObject.Find("mini_task_" + i);
            miniTasksButtons[i] = GameObject.Find("btn_task_" + i);
            miniTasksButtons[i].SetActive(false);
            miniTasksTimerLabel[i] = GameObject.Find("mini_task_timer_" + i).GetComponent<TMP_Text>();
            miniTasksTimerLabel[i].text = "";
        }

        miniGames[0] = GameObject.Find("binary_convert");
        miniGames[1] = GameObject.Find("logic_mini_game");
        miniGames[1].SetActive(false);
        getReadyWarning = GameObject.Find("get_ready_warning");
        //Debug.Log(PlayerPrefs.GetInt("offline"));
        StartCoroutine(startGame());
    }

    // Update is called once per frame
    void Update()
    {
        checkGameOver();

        for (int i = 0; i < isTaskBroken.Length; i++)
        {
            if(isTaskBroken[i] == true && sm.getScore() >= 80)
            {
                miniTasksButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = l.GetLocalisedString("revive") +"\n-80 PTS";
                miniTasksButtons[i].SetActive(true);
            }
            else if(isTaskBroken[i] == true && sm.getScore() < 80)
            {
                miniTasksButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = l.GetLocalisedString("revive") +"\n-80 PTS";
                miniTasksButtons[i].SetActive(false);
            }
            else
            {
                miniTasksButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = l.GetLocalisedString("fix_me");
            }
        }
    }

    IEnumerator startGame()
    {
        getReadyMessage = getReadyWarning.GetComponentInChildren<TMP_Text>();
        getReadyMessage.text = l.GetLocalisedString("get_ready");
        yield return new WaitForSeconds(1);

        for (int i = 5; i > 0; i--)
        {
            getReadyMessage.text = i.ToString();
            yield return new WaitForSeconds(1);
        }

        getReadyMessage.text = l.GetLocalisedString("go");
        yield return new WaitForSeconds(1);
        getReadyWarning.SetActive(false);

        StartCoroutine(gameTimer());
        StartCoroutine(spawnTasks());
    }

    IEnumerator gameOver()
    {
        getReadyWarning.SetActive(true);
        getReadyMessage = getReadyWarning.GetComponentInChildren<TMP_Text>();
        getReadyMessage.text = l.GetLocalisedString("game_over");
        yield return new WaitForSeconds(2);

        if(PlayerPrefs.GetInt("offline") == 0)
        {
            sm.saveScore();
            SceneManager.LoadScene("LobbyStats");
        }
        else if(PlayerPrefs.GetInt("offline") == 1)
        {
            sm.saveScore();
            SceneManager.LoadScene("GameOver");
        }
    }

    IEnumerator gameTimer()
    {
        gameSecondsLeft = gameSecondsLeft - 1;
        System.TimeSpan timeLeft = System.TimeSpan.FromSeconds(gameSecondsLeft);

        gameTimeLabel.text = timeLeft.ToString(@"m\:ss");
        yield return new WaitForSeconds(1);
        StartCoroutine(gameTimer());
    }

    IEnumerator taskTimer(int t)
    {
        yield return new WaitForSeconds(1);
        taskTime[t] = System.Int32.Parse(miniTasksTimerLabel[t].text) - 1;

        if(isTaskCompleted[t] == true && isTaskActive[t] == true)
        {
            resetTask(t);
            miniGames[0].GetComponent<Binary_Conversion>().closeMiniGame(t);
            miniGames[1].GetComponent<Logic_Game>().closeMiniGame(t);
            sm.addToTasksCompleted(1);
            sm.saveScore();
        }
        else if(taskTime[t] > 0 && isTaskActive[t] == true)
        {
            miniTasksTimerLabel[t].text = taskTime[t].ToString();
            StartCoroutine(taskTimer(t));
        }
        else
        {
            miniGames[0].GetComponent<Binary_Conversion>().closeMiniGame(t);
            miniGames[1].GetComponent<Logic_Game>().closeMiniGame(t);
            isTaskActive[t] = false;
            isTaskBroken[t] = true;
            miniTasksButtons[t].SetActive(false);
            miniTasksTimerLabel[t].text = "";
            miniTasksObjects[t].GetComponent<Image>().sprite = miniTasksSprite[2];
        }
    }

    IEnumerator spawnTasks()
    {
        yield return new WaitForSeconds(1);
        spawnTask = UnityEngine.Random.Range(1,3);

        if(spawnTask == 2)
        {
            int taskNum = UnityEngine.Random.Range(0,3);
            int gameNum = UnityEngine.Random.Range(1,3);

            if(isTaskActive[taskNum] == false && isTaskBroken[taskNum] == false && gameSecondsLeft > 30)
            {
                taskGame[taskNum] = gameNum;
                isTaskActive[taskNum] = true;
                miniTasksButtons[taskNum].SetActive(true);
                miniTasksObjects[taskNum].GetComponent<Image>().sprite = miniTasksSprite[0];
                miniTasksTimerLabel[taskNum].text = "30";
                taskTime[taskNum] = 30;
                StartCoroutine(taskTimer(taskNum));
            }
            
        }

        yield return new WaitForSeconds(4);
        StartCoroutine(spawnTasks());
    }

    void resetTask(int t)
    {
        isTaskBroken[t] = false;
        isTaskActive[t] = false;
        isTaskCompleted[t] = false;
        miniTasksButtons[t].SetActive(false);
        miniTasksTimerLabel[t].text = "";
        miniTasksObjects[t].GetComponent<Image>().sprite = miniTasksSprite[1];
        miniTasksButtons[t].GetComponentInChildren<TextMeshProUGUI>().text = l.GetLocalisedString("fix_me");
    }

    public void miniTaskButton(int btn)
    {
        if(isTaskBroken[btn] == true)
        {
            sm.deductScore(80);
            resetTask(btn);
            sm.addToTasksRevived(1);
            sm.saveScore();
        }
        else if(taskGame[btn] == 1)
        {
            miniGames[0].SetActive(true);
            miniGames[0].GetComponent<Binary_Conversion>().startMiniGame(btn);
        }
        else if(taskGame[btn] == 2)
        {
            miniGames[1].SetActive(true);
            miniGames[1].GetComponent<Logic_Game>().startMiniGame(btn,1);
        }
        
    }

    public void setTaskCompleted(int t)
    {
        isTaskCompleted[t] = true;
    }

    public string getTaskTime(int t)
    {
        return miniTasksTimerLabel[t].text;
    }

    void checkGameOver()
    {
        int brokenTasks = 0;
        
        for (int i = 0; i < isTaskBroken.Length; i++)
        {
            if(isTaskBroken[i] == true)
            {
                brokenTasks++;
            }
        }

        if(brokenTasks == isTaskBroken.Length)
        {
            StartCoroutine(gameOver());
        }
    }
}
