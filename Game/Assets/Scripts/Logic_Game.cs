using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class Logic_Game : MonoBehaviour
{
    Localiser l = new Localiser();
    Score_Manager sm;
    Game_Manager gm;
    TMP_Text pointsLabel;
    TMP_Text timeLabel;

    public Sprite[] gateSprite;
    public Sprite[] taskSprite;
    TextMeshProUGUI[] inputText;
    GameObject[] nodes;
    Image[] lines;
    int[] b;
    bool[] isNodeTrue;

    int level = 0;
    int points = 100;
    int callingTask;

    // Start is called before the first frame update
    void Start()
    {
        sm = GameObject.Find("Main Camera").GetComponent<Score_Manager>();
        gm = GameObject.Find("Main Camera").GetComponent<Game_Manager>();
        pointsLabel = GameObject.Find("logic_mini_game/lbl_points").GetComponent<TMP_Text>();
        timeLabel = GameObject.Find("logic_mini_game/lbl_time_task").GetComponent<TMP_Text>();
        callingTask = 0;
    }

    public void startMiniGame(int cTask, int l)
    {
        level = l;
        callingTask = cTask;

        //Setup Level 1
        if(level == 1)
        {
            lines = new Image[12];
            inputText = new TextMeshProUGUI[3];
            nodes = new GameObject[3];
            b = new int[3];
            isNodeTrue = new bool[3];

            //Get nodes
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i] = GameObject.Find("logic_level_1/node_" + i);
            }

            //Get buttons
            for (int i = 0; i < inputText.Length; i++)
            {
                inputText[i] = GameObject.Find("logic_level_1/buttons_panel/btn_binary_" + i).GetComponentInChildren<TextMeshProUGUI>();
            }

            //Get lines
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = GameObject.Find("logic_level_1/line_" + i).GetComponent<Image>();
            }
        }
        resetGame();
    }
    // Update is called once per frame
    void Update()
    {
        if(level > 0)
        {
            updateLogicPath();
            getLogicResult();
        }

        pointsLabel.text = l.GetLocalisedString("points") + points.ToString();
        timeLabel.text = l.GetLocalisedString("time") + "\n" + gm.getTaskTime(callingTask);
    }

    void updateLogicPath()
    {
        //CONVERT INPUTS TO INT
        for (int i = 0; i < b.Length; i++)
        {
            b[i] = System.Int32.Parse(inputText[i].text);
        }

        if(level == 1)
        {
            //INPUT 0
            if(b[0] == 1)
            {
                lines[4].color = new Color32(203,255,194,255);
            }
            else
            {
                lines[4].color = new Color32(255,194,194,255);
            }

            //INPUT 1
            if(b[1] == 1)
            {
                for (int i = 1; i < 4; i++)
                {
                    lines[i].color = new Color32(203,255,194,255);
                }
            }
            else
            {
                for (int i = 1; i < 4; i++)
                {
                    lines[i].color = new Color32(255,194,194,255);
                }
            }
   

            //INPUT 2
            if(b[2] == 1)
            {
                lines[0].color = new Color32(203,255,194,255);
            }
            else
            {
                lines[0].color = new Color32(255,194,194,255);
            }

            //NODE 0 - NOT - FALSE
            if(b[0] == 0 && nodes[0].transform.Find("not") == true)
            {
                for (int i = 5; i < 8; i++)
                {
                    lines[i].color = new Color32(203,255,194,255);
                }
                isNodeTrue[0] = true;
            }
            //NODE 0 - NOT - TRUE
            else if(b[0] == 1 && nodes[0].transform.Find("not") == true)
            {
                for (int i = 5; i < 8; i++)
                {
                    lines[i].color = new Color32(255,194,194,255);
                }  
                isNodeTrue[0] = false;  
            }
            //NODE 0 - NO GATE
            else
            {
                for (int i = 5; i < 8; i++)
                {
                    lines[i].color = new Color32(255,194,194,255);
                }
                isNodeTrue[0] = false;    
            }

            //NODE 1 - AND
            if(b[1] == 1 && b[2] == 1 && nodes[1].transform.Find("and") == true)
            {
                for (int i = 8; i < 11; i++)
                {
                    lines[i].color = new Color32(203,255,194,255);
                }
                isNodeTrue[1] = true;    
            }
            //NODE 1 - OR
            else if(((b[1] == 1 && b[2] == 0) || (b[1] == 0 && b[2] == 1)) && nodes[1].transform.Find("or") == true)
            {
                for (int i = 8; i < 11; i++)
                {
                    lines[i].color = new Color32(203,255,194,255);
                }
                isNodeTrue[1] = true;    
            }
            //NODE 1 - NO GATE
            else
            {
                for (int i = 8; i < 11; i++)
                {
                    lines[i].color = new Color32(255,194,194,255);
                }
                isNodeTrue[1] = false;    
            }

             //NODE 2 - AND
            if((isNodeTrue[0] && isNodeTrue[1]) && nodes[2].transform.Find("and") == true)
            {
                for (int i = 8; i < 11; i++)
                {
                    lines[i].color = new Color32(203,255,194,255);
                }    
                isNodeTrue[2] = true;
            }
            //NODE 2 - OR
            else if((isNodeTrue[0] && isNodeTrue[1] == false) || (isNodeTrue[0] == false && isNodeTrue[1]) && nodes[2].transform.Find("or") == true)
            {
                if(nodes[0].transform.Find("not") == true && nodes[1].transform.Find("and") == true) //Check if and gate exists
                {
                    lines[11].color = new Color32(203,255,194,255);
                    isNodeTrue[2] = true;  
                }  
            }
            //NODE 2 - NO GATE
            else
            {
                lines[11].color = new Color32(255,194,194,255);
                isNodeTrue[2] = false;    
            }
        }
    }
    
    bool getLogicResult()
    {
        if(level == 1)
        {
            if((isNodeTrue[0] && isNodeTrue[1] && isNodeTrue[2]) || (isNodeTrue[0] == false && isNodeTrue[1] && isNodeTrue[2]))
            {
                GameObject.Find("logic_level_1/node_result").GetComponent<Image>().sprite = taskSprite[1];
                return true;
            }
            else
            {
                GameObject.Find("logic_level_1/node_result").GetComponent<Image>().sprite = taskSprite[0];
                return false;
            }  
        }
        else
        {
            return false;
        }
    }

    public void checkResult()
    {
        if(getLogicResult() == true)
        {
            sm.addToScore(points);
            sm.saveScore();
            gm.setTaskCompleted(callingTask);
        }
    }

    public void closeMiniGame(int cTask)
    {
        if(cTask == callingTask)
        {
            this.gameObject.SetActive(false);
        }
    }

    void resetGame()
    {
        points = 100;
        //Resets nodes and gates
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].GetComponent<Image>().sprite = null;
            if(nodes[i].transform.childCount > 0)
            {  
                Transform child = nodes[i].transform.GetChild(0);
                child.SetParent(GameObject.Find("logic_mini_game/toolbox").transform);
                if(child.name.Equals("not"))
                {
                    child.SetAsFirstSibling();
                }
                else if (child.name.Equals("or"))
                {
                    child.SetAsLastSibling();
                }
                else if(child.name.Equals("and"))
                {
                    child.SetSiblingIndex(1);
                }
            }
        }

        //Resets input buttons
        for (int i = 0; i < inputText.Length; i++)
        {
            inputText[i].text = "0";
            GameObject.Find("logic_level_1/buttons_panel/btn_binary_" + i).GetComponent<Image>().color = new Color32(212,26,0,255);;
         
        }
    }

    public void switchBinary(GameObject b)
    {
        TextMeshProUGUI inputText = b.GetComponentInChildren<TextMeshProUGUI>();
        Image btnImg = b.GetComponent<Image>();
        
        if(inputText.text.Equals("0"))
        {
            inputText.text = "1";
            btnImg.color = new Color32(0,144,57,255);
        }
        else
        {
            inputText.text = "0";
            btnImg.color = new Color32(212,26,0,255);
        }
    }
}