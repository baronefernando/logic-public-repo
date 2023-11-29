using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Scene_Manager : MonoBehaviour
{
    private int user_id;
    Popups popup;
    DB_Manager db;
    Localiser l = new Localiser();

    //For game over
    int score;
    int tasksRevived;
    int tasksCompleted;

    void Start()
    {
        if(SceneManager.GetActiveScene().name.Equals("GameOver"))
        {
            score = PlayerPrefs.GetInt("score",0);
            tasksRevived = PlayerPrefs.GetInt("tasksRevived",0);
            tasksCompleted = PlayerPrefs.GetInt("tasksCompleted",0);
        }
        
        if(SceneManager.GetActiveScene().name.Equals("Game") == false && SceneManager.GetActiveScene().name.Equals("GameOver") == false)
        {
            popup = GameObject.Find("popup").GetComponent<Popups>();
        }

        if(SceneManager.GetActiveScene().name.Equals("GameOver") == false)
        {
            db = this.gameObject.GetComponent<DB_Manager>();
        }
        
    }

    void Update()
    {
        if(SceneManager.GetActiveScene().name.Equals("GameOver"))
        {
            TMP_Text stats = GameObject.Find("lblStats").GetComponent<TMP_Text>();
            stats.text = l.GetLocalisedString("score") + score + "\n\n" + l.GetLocalisedString("tasks_completed") + tasksCompleted + "\n\n" + l.GetLocalisedString("tasks_repaired") + tasksRevived;
        }   
    }

    public void CreateRoom()
    {
        if(PlayerPrefs.GetInt("offline",0) == 1)
        {
            string title = l.GetLocalisedString("connection_error_header");
            string body = l.GetLocalisedString("connection_error_message");
            popup.CreatePopup(title,body,"OK");
        }
        else
        {
            if(SceneManager.GetActiveScene().name.Equals("PlayChoice"))
            {
                PlayerPrefs.SetInt("room_id",0);
                SceneManager.LoadScene("CreateRoom");
            }
            else if(SceneManager.GetActiveScene().name.Equals("CreateRoom") || SceneManager.GetActiveScene().name.Equals("Lobby"))
            {
                string room_pin = GameObject.Find("pin").GetComponent<TMP_InputField>().text;
                string lock_state = GameObject.Find("btn_locked_state").GetComponentInChildren<TextMeshProUGUI>().text;
                TMP_InputField txtInputSize = GameObject.Find("size").GetComponent<TMP_InputField>();

                // Validate input size
                if(txtInputSize.text.Length > 0)
                {
                    if(System.Int32.Parse(txtInputSize.text) < 10)
                    {
                        string title = l.GetLocalisedString("too_small_room_error_header");
                        string body = l.GetLocalisedString("too_small_room_error_message");
                        popup.CreatePopup(title,body,"OK");
                        return;
                    }
                    else if(System.Int32.Parse(txtInputSize.text) > 50)
                    {
                        string title = l.GetLocalisedString("too_big_room_error_header");
                        string body = l.GetLocalisedString("too_big_room_error_message");
                        popup.CreatePopup(title,body,"OK");
                        return;
                    }
                    else
                    {
                        PlayerPrefs.SetInt("room_capacity",System.Int32.Parse(txtInputSize.text));
                    }
                }
                else
                {
                    string title = l.GetLocalisedString("how_many_players_header");
                    string body = l.GetLocalisedString("how_many_players_message");
                    popup.CreatePopup(title,body,"OK");
                    return;
                }

                // Validate lock & PIN
                if(lock_state.Equals(l.GetLocalisedString("locked")))
                {
                    PlayerPrefs.SetInt("room_locked", 1);
                    if(room_pin.Length > 0 && System.Int32.Parse(room_pin) != 0)
                    {
                        PlayerPrefs.SetInt("room_pin", System.Int32.Parse(room_pin));
                    }
                    else
                    {
                        string title = l.GetLocalisedString("faulty_lock_error_header");
                        string body = l.GetLocalisedString("faulty_lock_error_message");
                        popup.CreatePopup(title,body,"OK");
                        return;
                    }
                }
                else
                {
                    PlayerPrefs.SetInt("room_locked", 0);
                }
                
                // Edits room
                if(SceneManager.GetActiveScene().name.Equals("Lobby"))
                {
                    PlayerPrefs.SetInt("room_gamestarted", 0);
                    StartCoroutine(db.UpdateRoomSettings());
                    return;
                }

                // Validates username
                TMP_InputField txtInputUsername = GameObject.Find("username").GetComponent<TMP_InputField>();
                PlayerPrefs.SetString("username",txtInputUsername.text);

                // Adds new room
                int rand = UnityEngine.Random.Range(1000,99999);
                StartCoroutine(db.AddRoom(rand));
            }
        }
    }  

    // Changes button text/colour on click
    public void LockToggle()
    {
        GameObject button = GameObject.Find("btn_locked_state");
        Image btnImg = button.GetComponent<Image>();
        TextMeshProUGUI btnText = button.GetComponentInChildren<TextMeshProUGUI>();
        
        if(btnText.text.Equals(l.GetLocalisedString("unlocked")))
        {
            btnImg.color = new Color32(212,26,0,255);
            btnText.text = l.GetLocalisedString("locked");
        }
        else
        {
            btnImg.color = new Color32(0,144,57,255);
            btnText.text = l.GetLocalisedString("unlocked");
        }
    }

    public void PlusButton()
    {
        TMP_InputField txtInputSize = GameObject.Find("size").GetComponent<TMP_InputField>();
        if(txtInputSize.text.Length > 0)
        {
            int value = System.Int32.Parse(txtInputSize.text);
            int result = value + 1;
            if(result <= 50 && result >= 10)
            {
                txtInputSize.text = result.ToString();
            }
            else if(System.Int32.Parse(txtInputSize.text) > 50)
            {
                txtInputSize.text = "50";
            }
            else if(System.Int32.Parse(txtInputSize.text) < 10)
            {
                txtInputSize.text = "10";
            }
        }
        else
        {
            txtInputSize.text = "10";
        }
    }

    public void MinusButton()
    {
        TMP_InputField txtInputSize = GameObject.Find("size").GetComponent<TMP_InputField>();
        if(txtInputSize.text.Length > 0)
        {
            int value = System.Int32.Parse(txtInputSize.text);
            int result = value - 1;
            if(result <= 50 && result >= 10)
            {
                txtInputSize.text = result.ToString();
            }
            else if(System.Int32.Parse(txtInputSize.text) > 50)
            {
                txtInputSize.text = "50";
            }
            else if(System.Int32.Parse(txtInputSize.text) < 10)
            {
                txtInputSize.text = "10";
            }
        }
        else
        {
            txtInputSize.text = "10";
        }
    }

    public void JoinRoom()
    {
        if(SceneManager.GetActiveScene().name.Equals("PlayChoice"))
        {
            if(PlayerPrefs.GetInt("offline",0) == 1)
            {
                string title = l.GetLocalisedString("connection_error_header");
                string body = l.GetLocalisedString("connection_error_message");
                popup.CreatePopup(title,body,"OK");
            }
            else
            {
                SceneManager.LoadScene("JoinRoom");
            }
        }
        else if(SceneManager.GetActiveScene().name.Equals("JoinRoom"))
        {
            if(PlayerPrefs.GetInt("offline",0) == 1)
            {
                string title = l.GetLocalisedString("connection_error_header");
                string body = l.GetLocalisedString("connection_error_message");
                popup.CreatePopup(title,body,"OK");
            }
            else
            {
                TMP_InputField txtInputUser,txtInputRoomCode;
                txtInputRoomCode = GameObject.Find("room_code").GetComponent<TMP_InputField>();
                txtInputUser = GameObject.Find("username").GetComponent<TMP_InputField>();
                PlayerPrefs.SetString("username",txtInputUser.text);

                if(txtInputRoomCode.text.Length > 0)
                {
                    int room_id = System.Int32.Parse(txtInputRoomCode.text);
                    StartCoroutine(db.JoinRoom(room_id,0));
                }
                else
                {
                    string title = l.GetLocalisedString("room_not_found_error_header");
                    string body = l.GetLocalisedString("room_no_input");
                    popup.CreatePopup(title,body,"OK");
                }
            }
        }
    }

    public void StartRoomGame()
    {
        StartCoroutine(db.resetUserScores());
    }
    
    public void LeaveRoom()
    {
        string title = l.GetLocalisedString("leave_room_header");
        string body = l.GetLocalisedString("leave_room_message");
        popup.CreatePopup(title,body,"YESNO");
    }

    public void GoPlayChoice()
    {
        SceneManager.LoadScene("PlayChoice");
    }

    public void GoPlayChoiceFromGameOver()
    {
        PlayerPrefs.SetInt("score", 0);
        PlayerPrefs.SetInt("tasksRevived", 0);
        PlayerPrefs.SetInt("tasksCompleted", 0);
        PlayerPrefs.SetInt("offline",0);

        SceneManager.LoadScene("PlayChoice");
    }

    public void GoGame()
    {
        PlayerPrefs.SetInt("offline",1);
        SceneManager.LoadScene("Game");
    }

    public void GoBackToLobby()
    {
        PlayerPrefs.SetInt("room_gamestarted", 0);
        StartCoroutine(db.UpdateRoomSettings());
        SceneManager.LoadScene("Lobby");
    }
    
    public void GoMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

