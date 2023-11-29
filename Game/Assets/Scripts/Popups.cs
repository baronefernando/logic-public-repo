using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class Popups : MonoBehaviour
{
    GameObject[] PopupProperties = new GameObject[5];
    GameObject Popup,PopupEditRoom;
    Image button_1_image, button_2_image;
    TextMeshProUGUI button_1_text,button_2_text;
    DB_Manager db;
    Localiser l = new();

    private int result;
    private string picked_option;
    // Start is called before the first frame update
    void Start()
    {
        db = GameObject.Find("Main Camera").GetComponent<DB_Manager>();
        PopupProperties[0] = GameObject.Find("popup/lblHeader");
        PopupProperties[1] = GameObject.Find("popup/lblBody");
        PopupProperties[2] = GameObject.Find("popup/buttons_panel/btn_option_1");
        PopupProperties[3] = GameObject.Find("popup/buttons_panel/btn_option_2");
        PopupProperties[4] = GameObject.Find("popup/user_input");

        button_1_image = PopupProperties[2].GetComponent<Image>();
        button_2_image = PopupProperties[3].GetComponent<Image>();
        button_1_text = PopupProperties[2].GetComponentInChildren<TextMeshProUGUI>();
        button_2_text = PopupProperties[3].GetComponentInChildren<TextMeshProUGUI>();

        Popup = GameObject.Find("popup");
        Popup.SetActive(false);
        if(SceneManager.GetActiveScene().name.Equals("Lobby"))
        {
            PopupEditRoom = GameObject.Find("popup_edit_room");
            PopupEditRoom.SetActive(false);
        }
    }

    public void CreatePopup(string label,string body,string options)
    {
        result = 0;
        picked_option = options;
        Popup.SetActive(true);
        PopupProperties[0].GetComponent<TMP_Text>().text = label;
        PopupProperties[1].GetComponent<TMP_Text>().text = body;
        
        if(options.Equals("YESNO"))
        {
            PopupProperties[2].SetActive(true);
            PopupProperties[3].SetActive(true);
            PopupProperties[4].SetActive(false);
            
            button_1_image.color = new Color32(0,144,57,255);
            button_2_image.color = new Color32(212,26,0,255);

            button_1_text.text = l.GetLocalisedString("yes");
            button_2_text.text = l.GetLocalisedString("no");
        }
        else if(options.Equals("OK"))
        {
            PopupProperties[2].SetActive(true);
            PopupProperties[3].SetActive(false);
            PopupProperties[4].SetActive(false);

            button_1_image.color = new Color32(0,144,57,255);
            button_1_text.text = "OK";
        }
        else if(options.Equals("OKINPUT"))
        {
            PopupProperties[2].SetActive(true);
            PopupProperties[3].SetActive(false);
            PopupProperties[4].SetActive(true);

            button_1_image.color = new Color32(0,144,57,255);
            button_1_text.text = l.GetLocalisedString("enter");
            
            if(SceneManager.GetActiveScene().name.Equals("JoinRoom"))
            {
                PopupProperties[4].GetComponent<TMP_InputField>().placeholder.GetComponent<TMP_Text>().text = "PIN";
            }
            else if(SceneManager.GetActiveScene().name.Equals("Lobby"))
            {
                PopupProperties[1].GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Left;
            }
        }
        else if(options.Equals("NOBUTTON"))
        {
            PopupProperties[2].SetActive(false);
            PopupProperties[3].SetActive(false);
            PopupProperties[4].SetActive(false);

            if(SceneManager.GetActiveScene().name.Equals("Lobby"))
            {
                PopupProperties[1].GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;
            }
        }
    }

    public void CreateEditRoomPopup(int[] roomSettings)
    {
        picked_option = "EDITROOM";
        PopupEditRoom.SetActive(true);
        if(roomSettings[2] == 1)
        {
            GameObject.Find("popup_edit_room/pin").GetComponent<TMP_InputField>().text = roomSettings[3].ToString();
            GameObject button = GameObject.Find("btn_locked_state");
            Image btnImg = button.GetComponent<Image>();
            TextMeshProUGUI btnText = button.GetComponentInChildren<TextMeshProUGUI>();
            btnImg.color = new Color32(212,26,0,255);
            btnText.text = l.GetLocalisedString("locked");
        }
        else
        {
            GameObject.Find("popup_edit_room/pin").GetComponent<TMP_InputField>().text = "";
            GameObject button = GameObject.Find("btn_locked_state");
            Image btnImg = button.GetComponent<Image>();
            TextMeshProUGUI btnText = button.GetComponentInChildren<TextMeshProUGUI>();
            btnImg.color = new Color32(0,144,57,255);
            btnText.text = l.GetLocalisedString("unlocked");
        }

        GameObject.Find("popup_edit_room/size").GetComponent<TMP_InputField>().text = roomSettings[4].ToString();
    }

    public void OptionClicked()
    {
        string name = EventSystem.current.currentSelectedGameObject.name;
        if(name.Equals("btn_option_1"))
        {
            result = 1;
        }
        else if(name.Equals("btn_option_2"))
        {
            result = 2;
        }

        if(SceneManager.GetActiveScene().name.Equals("PlayChoice"))
        {
            if(result == 1)
            {
                Close();
            }
        }
        else if(SceneManager.GetActiveScene().name.Equals("JoinRoom"))
        {
            if(result == 1)
            {
                if(picked_option.Equals("OK"))
                {
                    Close();
                }
                else if(picked_option.Equals("OKINPUT"))
                {
                    string data = PopupProperties[4].GetComponent<TMP_InputField>().text;
                    if(data.Length > 0)
                    {
                        int input = System.Int32.Parse(data);

                        if(input == 0)
                        {
                            PopupProperties[1].GetComponent<TMP_Text>().text = l.GetLocalisedString("room_locked_wrong_pin");
                            return;
                        }

                        PlayerPrefs.SetInt("room_pin",input);
                        int room_id = PlayerPrefs.GetInt("room_id_join");
                        StartCoroutine(db.JoinRoom(room_id,input));
                    }
                    else
                    {
                        PopupProperties[1].GetComponent<TMP_Text>().text = l.GetLocalisedString("enter_a_pin");
                    }
                }   
            }
        }
        else if(SceneManager.GetActiveScene().name.Equals("CreateRoom"))
        {
            if(picked_option.Equals("OK"))
            {
                if(result == 1)
                {
                    Close();
                }
            }
        }
        else if(SceneManager.GetActiveScene().name.Equals("Lobby"))
        {
            if(picked_option.Equals("YESNO"))
            {
                if(result == 1)
                {
                    StartCoroutine(db.JoinRoom(0,0));
                }
                else
                {
                    Close();
                }
            }
            else if(picked_option.Equals("OK"))
            {
                Close();
            }
            else if(picked_option.Equals("EDITROOM"))
            {
                this.gameObject.GetComponent<Scene_Manager>().CreateRoom();
            }
        }
    }
    
    public int GetResult()
    {
        return result;
    }

    public void Close()
    {
        Popup.SetActive(false);
        if(SceneManager.GetActiveScene().name.Equals("Lobby"))
        {
            if(picked_option.Equals("EDITROOM"))
            {
                PopupEditRoom.SetActive(false);
            }

            if(PopupEditRoom.activeSelf)
            {
                picked_option = "EDITROOM";
            }
        }
    }
}
