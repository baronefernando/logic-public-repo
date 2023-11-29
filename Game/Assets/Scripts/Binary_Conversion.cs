using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Binary_Conversion : MonoBehaviour
{
    Score_Manager sm;
    Game_Manager gm;
    Localiser l = new Localiser();
    
    GameObject[] binaryButtons = new GameObject[8];
    TextMeshProUGUI[] binaryValues = new TextMeshProUGUI[8];
    int[] powerValues = {1,2,4,8,16,32,64,128};
    
    TMP_Text descMessage;
    TMP_Text pointsLabel;
    TMP_Text timeLabel;

    int points = 100;
    int expectedResult;
    int callingTask;

    GameObject helpPanel;
    TMP_Text helpResult;
    bool helpActive;

    void Start()
    {
        sm = GameObject.Find("Main Camera").GetComponent<Score_Manager>();
        gm = GameObject.Find("Main Camera").GetComponent<Game_Manager>();

        for (int i = 0; i < binaryButtons.Length; i++)
        {
            binaryButtons[i] = GameObject.Find("binary_convert/buttons_panel/btn_binary_" + i);
            binaryValues[i] = binaryButtons[i].GetComponentInChildren<TextMeshProUGUI>();
        }
        
        pointsLabel = GameObject.Find("lbl_points").GetComponent<TMP_Text>();
        helpPanel = GameObject.Find("help_panel");
        descMessage = GameObject.Find("lblDescription").GetComponent<TMP_Text>();
        timeLabel = GameObject.Find("lbl_time_task").GetComponent<TMP_Text>();
        helpResult = GameObject.Find("lblSumResult").GetComponent<TMP_Text>();
        callingTask = 0;
        this.gameObject.SetActive(false);
    }
    
    public void startMiniGame(int cTask)
    {
        callingTask = cTask;
        points = 100;
        
        for (int i = 0; i < binaryButtons.Length; i++)
        {
            binaryButtons[i].GetComponent<Image>().color = new Color32(212,26,0,255);
            binaryButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = "0";
        }

        helpActive = false;
        helpPanel.SetActive(false);
        helpResult.text = "";

        expectedResult = UnityEngine.Random.Range(1,256);
        descMessage.text = l.GetLocalisedString("convert") + " " + expectedResult + " " + l.GetLocalisedString("into_binary");

        this.gameObject.SetActive(true);
    }

    public void switchBinary(GameObject b)
    {
        TextMeshProUGUI btnText = b.GetComponentInChildren<TextMeshProUGUI>();
        Image btnImg = b.GetComponent<Image>();
        
        if(btnText.text.Equals("0"))
        {
            btnText.text = "1";
            btnImg.color = new Color32(0,144,57,255);
        }
        else
        {
            btnText.text = "0";
            btnImg.color = new Color32(212,26,0,255);
        }
    }
    
    int getBinaryResult()
    {
        int sum = 0;
        for (int i = 0; i < binaryValues.Length; i++)
        {
            if(binaryValues[i].text.Equals("1"))
            {
                sum += powerValues[i];
            }
        }
        return sum;
    }

    public void help()
    {
        if(helpActive == false)
        {
            points -= 50;
            helpPanel.SetActive(true);
            helpActive = true;
        }
    }

    public void check()
    {
        if(getBinaryResult() == expectedResult)
        {
            sm.addToScore(points);
            sm.saveScore();
            gm.setTaskCompleted(callingTask);
        }
        else
        {
            StartCoroutine(showWrongAnswerLabel());
        }
    }

    IEnumerator showWrongAnswerLabel()
    {
        descMessage.text = l.GetLocalisedString("wrong_answer");
        yield return new WaitForSeconds(1);
        descMessage.text = l.GetLocalisedString("convert") + " " + expectedResult + " " + l.GetLocalisedString("into_binary");
    }

    public void closeMiniGame(int cTask)
    {
        if(cTask == callingTask)
        {
            this.gameObject.SetActive(false);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if(helpActive)
        {
            helpResult.text = "= " + getBinaryResult();
        }

        pointsLabel.text = l.GetLocalisedString("points") + points.ToString();
        timeLabel.text = l.GetLocalisedString("time") + "\n" + gm.getTaskTime(callingTask);
    }
}
