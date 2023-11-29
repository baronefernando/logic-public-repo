using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using System.Linq;

public class DB_Manager : MonoBehaviour
{
    //  Server related variable
    string server_ip;
    string server_api_key = "";

    // User related variables
    int user_id;
    int isPlayingOffline;
    string username;
    int rand;

    // Room related variables
    int room_id;
    int[] roomSettings;
    bool isLobbyUpdating = false;
    bool isUserJoining = true;
    GameObject lobbyPrefab,lobby_buttons_panel,lobby_buttons_start,lobby_buttons_edit;
    GameObject[] lobbyStatsPrefabs = new GameObject[4];
    Transform lobbyContainerTransform;
    UserRoot lobby;

    // Other variables
    TMP_Text txtUserID, txtRoomID;
    TMP_InputField txtInputUser;
    Popups popup;
    Localiser l = new Localiser();

    // Start is called before the first frame update
    void Start()
    {
        // Get variables from device's memory if exists
        user_id = PlayerPrefs.GetInt("user_id", 0);
        room_id = PlayerPrefs.GetInt("room_id", 0);
        username = PlayerPrefs.GetString("username", "unknown");
        isPlayingOffline = PlayerPrefs.GetInt("offline", 0);
        server_ip = PlayerPrefs.GetString("server_ip","");
        
        if(SceneManager.GetActiveScene().name != "Game")
        {
            // Gets Popups component
            popup = GameObject.Find("popup").GetComponent<Popups>();
        }
        
        // Updates according to scene being shown
        if(SceneManager.GetActiveScene().name.Equals("PlayChoice"))
        {
            PlayerPrefs.SetInt("offline",0);
            
            txtUserID = GameObject.Find("user_id").GetComponent<TMP_Text>();
            if(user_id == 0) // No saved user
            {
                rand = UnityEngine.Random.Range(1,Int32.MaxValue);
                StartCoroutine(AddUser(rand));
            }
            else
            {
                StartCoroutine(AddUser(user_id));
            }

            if(PlayerPrefs.GetInt("offline", 0) == 0 && room_id == 0)
            {
                PlayerPrefs.SetInt("score",0);
                PlayerPrefs.SetInt("tasksRevived",0);
                PlayerPrefs.SetInt("tasksCompleted",0);
                StartCoroutine(AddUserScores(PlayerPrefs.GetInt("user_id")));
            }
        }

        // Updates according to scene being shown
        else if(SceneManager.GetActiveScene().name.Equals("Lobby"))
        {
            txtRoomID = GameObject.Find("room_id").GetComponent<TMP_Text>();
            txtRoomID.text = "";
            lobbyPrefab = GameObject.Find("Scroll Area/Scroll/Container/one_user (1)");
            lobbyContainerTransform = GameObject.Find("Scroll Area/Scroll/Container").transform;

            lobby_buttons_panel = GameObject.Find("buttons_panel");
            lobby_buttons_start = GameObject.Find("buttons_panel/btn_start_room");
            lobby_buttons_edit = GameObject.Find("buttons_panel/btn_edit_room");
            
            lobby_buttons_edit.SetActive(false);
            lobby_buttons_start.SetActive(false);
            lobby_buttons_panel.SetActive(false);
        }

        else if(SceneManager.GetActiveScene().name.Equals("LobbyStats"))
        {
            txtRoomID = GameObject.Find("room_id").GetComponent<TMP_Text>();
            txtRoomID.text = "";

            for (int i = 0; i < lobbyStatsPrefabs.Length; i++)
            {
                lobbyStatsPrefabs[i] = GameObject.Find("Scroll Area/Scroll/Container/one_user ("+ i +")");
            }

            lobbyContainerTransform = GameObject.Find("Scroll Area/Scroll/Container").transform;
        }

        // Updates according to scene being shown
        else if(SceneManager.GetActiveScene().name.Equals("CreateRoom"))
        {
            StartCoroutine(GetUsernameById(user_id));
            txtInputUser = GameObject.Find("username").GetComponent<TMP_InputField>();
        }

        // Updates according to scene being shown
        else if(SceneManager.GetActiveScene().name.Equals("JoinRoom"))
        {
            StartCoroutine(GetUsernameById(user_id));
            txtInputUser = GameObject.Find("username").GetComponent<TMP_InputField>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Updates according to scene being shown
        if(SceneManager.GetActiveScene().name.Equals("PlayChoice"))
        {
            if(isPlayingOffline == 1)
            {
                txtUserID.text = "";
            }
            else
            {
                string player_id = l.GetLocalisedString("player_id").ToUpper();
                txtUserID.text = "ID: "  + user_id;
            }
        }

        // Updates according to scene being shown
        else if(SceneManager.GetActiveScene().name.Equals("Lobby"))
        {
            if(isUserJoining == true)
            {
                string title = l.GetLocalisedString("joining");
                string body = l.GetLocalisedString("loading_room") + "...";
                popup.CreatePopup(title,body,"NOBUTTON");
            }
            if(isLobbyUpdating == false && room_id != 0)
            {
                StartCoroutine(RefreshRoom(room_id));
            }
        }

         // Updates according to scene being shown
        else if(SceneManager.GetActiveScene().name.Equals("LobbyStats"))
        {
            if(isLobbyUpdating == false && room_id != 0)
            {
                StartCoroutine(RefreshRoom(room_id));
            }
        }
    }

    #region USER PHP_API METHODS

    IEnumerator AddUser(int id)
    {
        string url = server_ip + "/game-backend/add_user.php";
        WWWForm form = new WWWForm();
        form.AddField("server_api_key", server_api_key);
        form.AddField("user_id", id);

        using(UnityWebRequest server = UnityWebRequest.Post(url,form))
        {
            yield return server.SendWebRequest();
            if(server.isNetworkError || server.isHttpError)
            {
                PlayerPrefs.SetInt("offline", 1);
                isPlayingOffline = PlayerPrefs.GetInt("offline");
                print(server.error);
            }
            else
            {   
                PlayerPrefs.SetInt("offline", 0);
                isPlayingOffline = PlayerPrefs.GetInt("offline");

                if(server.downloadHandler.text.Equals("ID ALREADY EXISTS"))
                {
                    if(user_id == 0)
                    {
                        rand = UnityEngine.Random.Range(1,Int32.MaxValue);
                        StartCoroutine(AddUser(rand));
                    }
                }
                else
                {
                    PlayerPrefs.SetInt("offline", 0);
                    isPlayingOffline = PlayerPrefs.GetInt("offline");
                    print(server.downloadHandler.text);
                    user_id = System.Int32.Parse(server.downloadHandler.text);
                    PlayerPrefs.SetInt("user_id", id);
                }
            }
        }
    }

    IEnumerator GetUsernameById(int id)
    {
        string url = server_ip + "/game-backend/get_user_username_by_id.php";
        WWWForm form = new WWWForm();
        form.AddField("server_api_key", server_api_key);
        form.AddField("user_id", id);

        using(UnityWebRequest server = UnityWebRequest.Post(url,form))
        {
            yield return server.SendWebRequest();
            if(server.isNetworkError || server.isHttpError)
            {
                PlayerPrefs.SetInt("offline", 1);
                isPlayingOffline = PlayerPrefs.GetInt("offline");
                print(server.error);
            }
            else
            {
                PlayerPrefs.SetInt("offline", 0);
                isPlayingOffline = PlayerPrefs.GetInt("offline");

                username = server.downloadHandler.text;
                PlayerPrefs.SetString("username", username);
                txtInputUser.text = username;
            }
        }
    }

    public IEnumerator resetUserScores()
    {
        string url = server_ip + "/game-backend/update_user_details.php";
        WWWForm form = new WWWForm();
        form.AddField("server_api_key", server_api_key);
        form.AddField("user_id", user_id);
        form.AddField("room_id", room_id);
        form.AddField("columns_to_update",8);
        form.AddField("score", 0);
        form.AddField("tasksRevived", 0);
        form.AddField("tasksCompleted", 0);

        using(UnityWebRequest server = UnityWebRequest.Post(url,form))
        {
            yield return server.SendWebRequest();
            if(server.isNetworkError || server.isHttpError)
            {
                PlayerPrefs.SetInt("offline", 1);
                isPlayingOffline = PlayerPrefs.GetInt("offline");
                print(server.error);
            }
            else
            {
                PlayerPrefs.SetInt("offline", 0);
                isPlayingOffline = PlayerPrefs.GetInt("offline");

                string result = server.downloadHandler.text;
                if(result.Equals("Updated"))
                {
                    Debug.Log(" UPDATED IN DB");
                    PlayerPrefs.SetInt("room_gamestarted", 1);
                    StartCoroutine(UpdateRoomSettings());
                }
            }
        }
    }

    public IEnumerator AddUserScores(int id)
    {
        string url = server_ip + "/game-backend/update_user_details.php";
        WWWForm form = new WWWForm();
        form.AddField("server_api_key", server_api_key);
        form.AddField("user_id", id);
        form.AddField("columns_to_update",7);
        form.AddField("score", PlayerPrefs.GetInt("score",0));
        form.AddField("tasksRevived", PlayerPrefs.GetInt("tasksRevived",0));
        form.AddField("tasksCompleted", PlayerPrefs.GetInt("tasksCompleted",0));

        using(UnityWebRequest server = UnityWebRequest.Post(url,form))
        {
            yield return server.SendWebRequest();
            if(server.isNetworkError || server.isHttpError)
            {
                PlayerPrefs.SetInt("offline", 1);
                isPlayingOffline = PlayerPrefs.GetInt("offline");
                print(server.error);
            }
            else
            {
                PlayerPrefs.SetInt("offline", 0);
                isPlayingOffline = PlayerPrefs.GetInt("offline");

                string result = server.downloadHandler.text;
                if(result.Equals("Updated"))
                {
                    Debug.Log(" UPDATED IN DB");
                }
            }
        }
    }
    #endregion

    #region ROOM PHP_API METHODS
    
    // Will add a new room
    public IEnumerator AddRoom(int r_id)
    {
        string url = server_ip + "/game-backend/add_room.php";
        WWWForm form = new WWWForm();
        form.AddField("server_api_key", server_api_key);
        form.AddField("room_id", r_id);
        form.AddField("owner_id", user_id);
        form.AddField("room_locked", PlayerPrefs.GetInt("room_locked",0));
        form.AddField("room_pin", PlayerPrefs.GetInt("room_pin",0));
        form.AddField("room_capacity", PlayerPrefs.GetInt("room_capacity",10));

        using(UnityWebRequest server = UnityWebRequest.Post(url,form))
        {
            yield return server.SendWebRequest();
            if(server.isNetworkError || server.isHttpError)
            {
                PlayerPrefs.SetInt("offline", 1);
                isPlayingOffline = PlayerPrefs.GetInt("offline");
                print(server.error);
            }
            else
            {
                PlayerPrefs.SetInt("offline", 0);
                isPlayingOffline = PlayerPrefs.GetInt("offline");

                if(server.downloadHandler.text.Equals("ROOM ALREADY EXISTS"))
                {
                    rand = UnityEngine.Random.Range(1000,99999);
                    StartCoroutine(AddRoom(rand));
                }
                else
                {    
                    room_id = r_id;
                    yield return StartCoroutine(JoinRoom(r_id,PlayerPrefs.GetInt("room_pin",0)));
                }
            }
        }

        PlayerPrefs.SetInt("room_pin", 0);
        PlayerPrefs.SetInt("room_locked", 0);
    }

    // Show edit room popup
    public void EditRoom()
    {
        popup.CreateEditRoomPopup(roomSettings);
    }

    // Will check the username and room code/pin
    public IEnumerator JoinRoom(int r_id,int r_pin)
    {
        string url = server_ip + "/game-backend/update_user_details.php";
        WWWForm form = new WWWForm();
        form.AddField("server_api_key", server_api_key);
        form.AddField("user_id", user_id);
        form.AddField("username", PlayerPrefs.GetString("username"));
        form.AddField("columns_to_update", 3);

        using(UnityWebRequest server = UnityWebRequest.Post(url,form))
        {
            yield return server.SendWebRequest();
            if(server.isNetworkError || server.isHttpError)
            {
                PlayerPrefs.SetInt("offline", 1);
                isPlayingOffline = PlayerPrefs.GetInt("offline");
                print(server.error);
            }
            else
            {
                PlayerPrefs.SetInt("offline", 0);
                isPlayingOffline = PlayerPrefs.GetInt("offline");

                string result = server.downloadHandler.text;

                if(result.Equals("USERNAME TAKEN"))
                {
                    string title = l.GetLocalisedString("username_taken_error_header");
                    string body = l.GetLocalisedString("username_taken_error_message");
                    popup.CreatePopup(title,body,"OK");
                }
                else
                {
                    username = PlayerPrefs.GetString("username");
                    StartCoroutine(UpdateUserRoom(r_id,r_pin));
                }
            }
        }
    }

    // Get most up-to-date room settings
    IEnumerator GetRoomSettings(int room_id)
    {
        string url = server_ip + "/game-backend/get_room_settings.php";
        WWWForm form = new WWWForm();
        form.AddField("server_api_key", server_api_key);
        form.AddField("room_id", room_id);

        using(UnityWebRequest server = UnityWebRequest.Post(url,form))
        {
            yield return server.SendWebRequest();
            if(server.isNetworkError || server.isHttpError)
            {
                PlayerPrefs.SetInt("offline", 1);
                isPlayingOffline = PlayerPrefs.GetInt("offline");
                print(server.error);
            }
            else
            {
                PlayerPrefs.SetInt("offline", 0);
                isPlayingOffline = PlayerPrefs.GetInt("offline");

                string result = server.downloadHandler.text;
                string[] t = result.Split(",");
                roomSettings = new int[t.Length];

                for (int i = 0; i < roomSettings.Length; i++)
                {
                    roomSettings[i] = Int32.Parse(t[i]);
                }

                //GAME STARTED
                if(SceneManager.GetActiveScene().name.Equals("Lobby"))
                {
                    if(roomSettings[roomSettings.Length - 1] == 1 && user_id != roomSettings[1])
                    {
                        SceneManager.LoadScene("Game");
                    }
                    else if(roomSettings[roomSettings.Length - 1] == 1 && user_id == roomSettings[1])
                    {
                        SceneManager.LoadScene("LobbyStats");
                    }
                }
            }
        }
    }

    // Updates room settings
    public IEnumerator UpdateRoomSettings()
    {
        string url = server_ip + "/game-backend/update_room_settings.php";
        WWWForm form = new WWWForm();

        form.AddField("server_api_key", server_api_key);
        form.AddField("room_id", room_id);

        if(PlayerPrefs.GetInt("room_gamestarted", 0) == 0)
        {
            form.AddField("room_locked", PlayerPrefs.GetInt("room_locked",0));
            form.AddField("room_pin", PlayerPrefs.GetInt("room_pin",0));
            form.AddField("room_capacity", PlayerPrefs.GetInt("room_capacity",10));
            form.AddField("game_started", PlayerPrefs.GetInt("room_gamestarted",0));
            form.AddField("columns_to_update", 6);
        }
        else if(PlayerPrefs.GetInt("room_gamestarted",0) == 1)
        {
            form.AddField("room_id", room_id);
            form.AddField("game_started", PlayerPrefs.GetInt("room_gamestarted",0));
            form.AddField("columns_to_update", 7);
        }

        using(UnityWebRequest server = UnityWebRequest.Post(url,form))
        {
            yield return server.SendWebRequest();
            if(server.isNetworkError || server.isHttpError)
            {
                PlayerPrefs.SetInt("offline", 1);
                isPlayingOffline = PlayerPrefs.GetInt("offline");
                print(server.error);
            }
            else
            {
                PlayerPrefs.SetInt("offline", 0);
                isPlayingOffline = PlayerPrefs.GetInt("offline");

                string result = server.downloadHandler.text.ToString();
                if(result.Equals("Updated"))
                {
                    popup.Close();
                }
            }
        }
    }

    // Will check if room exists and validate pin/code and update user room
    IEnumerator UpdateUserRoom(int r_id,int r_pin)
    {
        string url = server_ip + "/game-backend/update_user_details.php";
        WWWForm form = new WWWForm();
        form.AddField("server_api_key", server_api_key);
        form.AddField("user_id", user_id);
        form.AddField("room_id", r_id);
        form.AddField("room_pin", r_pin);
        form.AddField("old_room_id", PlayerPrefs.GetInt("room_id"));
        form.AddField("columns_to_update", 1);

        using(UnityWebRequest server = UnityWebRequest.Post(url,form))
        {
            yield return server.SendWebRequest();
            if(server.isNetworkError || server.isHttpError)
            {
                PlayerPrefs.SetInt("offline", 1);
                isPlayingOffline = PlayerPrefs.GetInt("offline");
                print(server.error);
            }
            else
            {
                PlayerPrefs.SetInt("offline", 0);
                isPlayingOffline = PlayerPrefs.GetInt("offline");

                string result = server.downloadHandler.text.ToString();

                // Success
                if(result.Equals("Updated"))
                {
                    PlayerPrefs.SetInt("room_id", r_id);
                    if(r_id != 0)
                    {
                        print("JOINED ROOM #" + r_id);
                    }
                    else
                    {
                        print("LEFT ROOM");
                    }
                    
                    // User in lobby
                    if(SceneManager.GetActiveScene().name.Equals("Lobby"))
                    {
                        SceneManager.LoadScene("PlayChoice");
                    }
                    else
                    {
                        SceneManager.LoadScene("Lobby");
                    }
                }
                // Room is locked and no pin was given
                else if(result.Equals("Locked"))
                {
                    PlayerPrefs.SetInt("room_id_join",r_id);

                    string title = l.GetLocalisedString("room_locked_header");
                    string body = l.GetLocalisedString("room_locked_message");
                    popup.CreatePopup(title,body,"OKINPUT");
                }
                // Room is locked and pin was wrong
                else if(result.Equals("Wrong"))
                {
                    string title = l.GetLocalisedString("room_locked_header");
                    string body = l.GetLocalisedString("room_locked_wrong_pin");
                    popup.CreatePopup(title,body,"OKINPUT");
                }
                // Room does not exist
                else if(result.Equals("ROOM DOES NOT EXIST"))
                {
                    string title = l.GetLocalisedString("room_not_found_error_header");
                    string body = l.GetLocalisedString("room_not_found_error_message");
                    popup.CreatePopup(title,body,"OK");
                }
            }
        }
    }
    
    // Refreshes room settings
    IEnumerator RefreshRoom(int room_id)
    {
        lobby = null;
        isLobbyUpdating = true;
        string url = server_ip + "/game-backend/get_lobby.php";
        WWWForm form = new WWWForm();
        form.AddField("server_api_key", server_api_key);
        form.AddField("room_id", room_id);

        using(UnityWebRequest server = UnityWebRequest.Post(url,form))
        {
            yield return server.SendWebRequest();
            if(server.isNetworkError || server.isHttpError)
            {
                PlayerPrefs.SetInt("offline", 1);
                isPlayingOffline = PlayerPrefs.GetInt("offline");
                print(server.error);
            }
            else
            {
                PlayerPrefs.SetInt("offline", 0);
                isPlayingOffline = PlayerPrefs.GetInt("offline");
                
                string jsonValue = server.downloadHandler.text;
               
                // Format PHP generated JSON to match serializable array object
                jsonValue = jsonValue.Replace("[","{\"user\":[");
                jsonValue = jsonValue.Replace("]","]}");
                lobby = JsonUtility.FromJson<UserRoot>(jsonValue);

                StartCoroutine(GetRoomSettings(room_id));

                // Wait 1 seconds to update lobby
                yield return new WaitForSeconds(1);
                if(SceneManager.GetActiveScene().name.Equals("Lobby"))
                {
                    RefreshUsersInLobby();

                    if(isUserJoining == true)
                    {
                        isUserJoining = false;
                        popup.Close();
                    }

                    //Updates room labels
                    if(roomSettings[2] == 1)
                    {
                        txtRoomID.text = "#" + roomSettings[0] + " PIN:" + roomSettings[3];
                    }
                    else
                    {
                        txtRoomID.text = "#" + roomSettings[0];
                    }

                    // If user is the room owner
                    lobby_buttons_panel.SetActive(true);
                    if(roomSettings[1] == user_id)
                    {
                        lobby_buttons_start.SetActive(true);
                        lobby_buttons_edit.SetActive(true);
                    }
                    // If user is not the room owner
                    else
                    {
                        lobby_buttons_start.SetActive(false);
                        lobby_buttons_edit.SetActive(false);
                    }
                }
                else if(SceneManager.GetActiveScene().name.Equals("LobbyStats"))
                {
                    RefreshUsersScores();
                    //Updates room labels
                    if(roomSettings[2] == 1)
                    {
                        txtRoomID.text = "#" + roomSettings[0] + " PIN:" + roomSettings[3] + " - " + l.GetLocalisedString("results");
                    }
                    else
                    {
                        txtRoomID.text = "#" + roomSettings[0] + " - " + l.GetLocalisedString("results");
                    }
                }

                isLobbyUpdating = false;
            }
        }
    }

    void RefreshUsersScores()
    {
        // Destroy prefab slots
        for (int i = lobby.user.Length - 1; i < roomSettings[4]; i++)
        {
            if(GameObject.Find("Scroll Area/Scroll/Container/one_user ("+i+")") != null)
            {
                if(i > 3)
                {
                    Destroy(GameObject.Find("Scroll Area/Scroll/Container/one_user ("+i+")"));
                }
                lobbyStatsPrefabs[i].SetActive(false);
            }
        }
        
        //No players
        if((lobby.user.Length - 1) == 0)
        {
            lobbyStatsPrefabs[0].SetActive(true);
            GameObject.Find("Scroll Area/Scroll/Container/one_user (0)/user_id").GetComponent<TMP_Text>().text = l.GetLocalisedString("oh_no") + "\n\n" + l.GetLocalisedString("no_stats_available");
            GameObject.Find("Scroll Area/Scroll/Container/one_user (0)/place").GetComponent<TMP_Text>().text = ":(";
        }

        //One player - GOLD
        if((lobby.user.Length - 1) == 1)
        {
            if(GameObject.Find("Scroll Area/Scroll/Container/one_user (0)") != null)
            {
                lobbyStatsPrefabs[0].SetActive(true);
            }
        }

        //Two players - GOLD & SILVER
        if((lobby.user.Length - 1) == 2)
        {
            if(GameObject.Find("Scroll Area/Scroll/Container/one_user (1)") != null)
            {
                lobbyStatsPrefabs[1].SetActive(true);
            }
        }
        
        //Three players - GOLD & SILVER & BRONZE
        if((lobby.user.Length - 1) == 3)
        {
            if(GameObject.Find("Scroll Area/Scroll/Container/one_user (2)") != null)
            {
                lobbyStatsPrefabs[1].SetActive(true);
                lobbyStatsPrefabs[2].SetActive(true);
            }
        }

        //Four player - GOLD & SILVER & BRONZE & 4TH PLACE
        if((lobby.user.Length - 1) == 4)
        {
            lobbyStatsPrefabs[3].SetActive(true);
        }

        //More than four players - create more slots
        if(lobby.user.Length - 1 > 4)
        {
            // Update container to capacity size
            for (int i = 4; i < lobby.user.Length - 1; i++)
            {
                if(GameObject.Find("Scroll Area/Scroll/Container/one_user ("+i+")") == null)
                {
                    lobbyStatsPrefabs[3].SetActive(true);
                    var clone = Instantiate(lobbyStatsPrefabs[3],lobbyContainerTransform);
                    clone.name = "one_user ("+i+")";
                    GameObject.Find("Scroll Area/Scroll/Container/one_user ("+i+")").SetActive(true);
                }
            }
        }
        
        //Resize container
        RectTransform rtContainer = GameObject.Find("Scroll Area/Scroll/Container").GetComponent<RectTransform>();
        RectTransform rtPrefab = lobbyStatsPrefabs[3].GetComponent<RectTransform>();
        rtContainer.sizeDelta = new Vector2(rtContainer.sizeDelta.x, (rtPrefab.sizeDelta.y * (lobby.user.Length - 1)));
        
        //Reset slots before updating
        for (int i = 0; i < lobby.user.Length - 1; i++)
        {
            GameObject.Find("Scroll Area/Scroll/Container/one_user ("+i+")/user_id").GetComponent<TMP_Text>().text = l.GetLocalisedString("empty_slot");
            GameObject.Find("Scroll Area/Scroll/Container/one_user ("+i+")/place").GetComponent<TMP_Text>().text = (i+1) + "#";
            if(i > 3 && ((i % 2) == 0)) //Even
            {
                GameObject.Find("Scroll Area/Scroll/Container/one_user (" + i + ")").GetComponent<Image>().color = new Color32(198,197,197,255);
            }
        }

        //Array is not zero
        if(lobby.user.Length > 0)
        {
            //Sends owner user to end of array
            for (int i = 0; i < lobby.user.Length; i++)
            {
                if(lobby.user[i].user_id == roomSettings[1])
                {
                    var temp = lobby.user[lobby.user.Length - 1];
                    lobby.user[lobby.user.Length - 1] = lobby.user[i];
                    lobby.user[i] = temp;
                }
            }

            //Bubble sort through user array by score ignoring owner ID
            for (int i = 0; i < lobby.user.Length - 1; i++)
            {
                for (int j = 0; j < lobby.user.Length - 1; j++)
                {
                    if(lobby.user[i].score > lobby.user[j].score)
                    {
                        var temp = lobby.user[j];
                        lobby.user[j] = lobby.user[i];
                        lobby.user[i] = temp;
                    }
                }
            }

            //Update slots
            TMP_Text slotText;
            for (int i = 0; i < lobby.user.Length - 1; i++)
            {
                //Debug.Log(lobby.user[i].username + " " + lobby.user[i].score);
                if(lobby.user[i].user_id != roomSettings[1])
                {
                    slotText = GameObject.Find("Scroll Area/Scroll/Container/one_user ("+i+")/user_id").GetComponent<TMP_Text>();
                    slotText.text = lobby.user[i].username + " " + l.GetLocalisedString("score") + lobby.user[i].score + "\n\n" + l.GetLocalisedString("tasks_completed") + lobby.user[i].tasksCompleted + " " + l.GetLocalisedString("tasks_repaired") + lobby.user[i].tasksRevived;
                    GameObject.Find("Scroll Area/Scroll/Container/one_user ("+i+")/place").GetComponent<TMP_Text>().text = (i+1) + "#" ;
                }
            }
        }
    }

    // Refreshes users inside room
    void RefreshUsersInLobby()
    {
        // Update container to capacity size
        RectTransform rtContainer = GameObject.Find("Scroll Area/Scroll/Container").GetComponent<RectTransform>();
        RectTransform rtPrefab = lobbyPrefab.GetComponent<RectTransform>();
        if(roomSettings[4] % 2 == 0) //Even
        {
            rtContainer.sizeDelta = new Vector2(rtContainer.sizeDelta.x, (rtPrefab.sizeDelta.y + (rtPrefab.sizeDelta.y * (roomSettings[4]/2))));
        }
        else //Odd
        {
            rtContainer.sizeDelta = new Vector2(rtContainer.sizeDelta.x, (rtPrefab.sizeDelta.y + (rtPrefab.sizeDelta.y * ((roomSettings[4]/2)+1))));
        }   
        
        // Destroy slots that are over room capacity
        for (int i = roomSettings[4]; i < 50; i++)
        {
            Destroy(GameObject.Find("Scroll Area/Scroll/Container/one_user ("+i+")"));
        }
        
        // Create new slots if current size is under room capacity
        for (int i = 2; i < roomSettings[4]; i++)
        {
            if(GameObject.Find("Scroll Area/Scroll/Container/one_user ("+i+")") == null)
            {
                GameObject.Find("Scroll Area/Scroll/Container/one_user (1)/user_id").GetComponent<TMP_Text>().text = l.GetLocalisedString("empty_slot");
                var clone = Instantiate(lobbyPrefab,lobbyContainerTransform);
                clone.name = "one_user ("+i+")";
            }
        }

        // Reset slots before updating
        for (int i = 0; i < roomSettings[4]; i++)
        {
            GameObject.Find("Scroll Area/Scroll/Container/one_user (1)").GetComponent<Image>().color = new Color32(123,0,183,255);
            GameObject.Find("Scroll Area/Scroll/Container/one_user ("+i+")/user_id").GetComponent<TMP_Text>().text = l.GetLocalisedString("empty_slot");
        }

        if(lobby.user.Length > 0)
        {
            for (int i = 0; i < lobby.user.Length; i++)
            {
                // Set first slot to room owner's id
                if(lobby.user[i].user_id == roomSettings[1])
                {
                    var temp = lobby.user[0];
                    lobby.user[0] = lobby.user[i];
                    lobby.user[i] = temp;
                }

                // Set user to second slot if it's not the room's owner
                if(lobby.user[i].user_id == user_id && user_id != roomSettings[1])
                {
                    var temp = lobby.user[1];
                    lobby.user[1] = lobby.user[i];
                    lobby.user[i] = temp;
                }
            }
            
            // Update rest of slots
            for(int i = 0; i < lobby.user.Length; i++)
            {
                GameObject.Find("Scroll Area/Scroll/Container/one_user ("+i+")/user_id").GetComponent<TMP_Text>().text = lobby.user[i].username;
            }
        }
    }
    #endregion
}