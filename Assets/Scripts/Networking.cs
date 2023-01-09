using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using static LeaderBoard;

public class Response
{
    public int code;
    public string response;
    public Dictionary<string, object> msg;

    override public string ToString()
    {
        return $"Code:{code}\nMsg:{msg}";
    }
}

public class Networking : MonoBehaviour
{
    private LoadHandler _loadHandler;
    private SaveHandler _saveHandler;
    private PersistantData _persistantData;

    public UIControls UIControls;

    public string username;
    public string password;
    public string email;
    public string code;

    public bool loggedIn;

    private void Start()
    {
        _loadHandler = GameObject.Find("Scripts").GetComponent<LoadHandler>();
        _saveHandler = GameObject.Find("Scripts").GetComponent<SaveHandler>();
        _persistantData = GameObject.Find("PersistantData").GetComponent<PersistantData>();

        DontDestroyOnLoad(this);
    }

    public void UpdateUsername(TMP_InputField input)
    {
        username = input.text;
    }

    public void UpdatePassword(TMP_InputField input)
    {
        password = input.text;
    }

    public void UpdateEmail(TMP_InputField input)
    {
        email = input.text;
    }

    public void UpdateCode(TMP_InputField input)
    {
        code = input.text;
    }

    public void Login()
    {
        UIControls.SetResponseMessage("Logging in...");

        WWWForm form = new WWWForm();
        form.AddField("rUsername", username);
        form.AddField("rPassword", password);

        StartCoroutine(SendAPIRequest("http://tapzoo.ddns.net:32300/login", "POST", form, (string response) =>
        {
            if (response != null)
            {
                try
                {
                    Response res = JsonConvert.DeserializeObject<Response>(response);

                    print(res.msg["zooData"].ToString());

                    if (res.code == 0)
                    {
                        _loadHandler.Load(res.msg["playerData"].ToString(), res.msg["zooData"].ToString());
                        UIControls.ShowLayout("LoggedIn");
                    }
                    else
                    {
                        UIControls.SetResponseMessage(res.response);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error parsing JSON: " + ex.Message);
                    return;
                }
            }
        }));
    }

    public void SignUp()
    {
        UIControls.SetResponseMessage("Signing up...");

        WWWForm form = new WWWForm();
        form.AddField("rUsername", username);
        form.AddField("rPassword", password);
        form.AddField("rEmail", email);

        StartCoroutine(SendAPIRequest("http://tapzoo.ddns.net:32300/create", "POST", form, (string response) =>
        {
            if(response != null)
            {
                try
                {
                    // Parse the JSON and get the response code and message
                    Response res = JsonUtility.FromJson<Response>(response);
                    print(res);

                    // Do something with the code and message
                    if (res.code == 0)
                    {
                        loggedIn = true;
                        UIControls.ShowLayout("Verify");
                    }
                    else
                    {
                        UIControls.SetResponseMessage(res.response);
                        Debug.Log(res.response);
                    }
                }
                catch(Exception ex)
                {
                    Debug.LogError("Error parsing JSON: " + ex.Message);
                    return;
                }
            }
        }));
    }

    public void Verify()
    {
        WWWForm form = new WWWForm();
        form.AddField("verificationCode", code);

        StartCoroutine(SendAPIRequest("http://tapzoo.ddns.net:32300/verify", "POST", form, (string response) =>
        {
            if(response != null)
            {
                try
                {
                    // Parse the JSON and get the response code and message
                    Response res = JsonUtility.FromJson<Response>(response);

                    // Do something with the code and message
                    if (res.code == 0)
                    {
                        loggedIn = true;
                        UIControls.ShowLayout("Default");
                    }

                    UIControls.SetResponseMessage(res.response);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error parsing JSON: " + ex.Message);
                }
            }
        }));
    }

    public void Logout()
    {
        WWWForm form = new WWWForm();
        form.AddField("playerId", _persistantData.PlayerData._id);

        StartCoroutine(SendAPIRequest("http://tapzoo.ddns.net:32300/logout", "POST", form, (string response) =>
        {
            if(response != null)
            {
                try
                {
                    Response res = JsonConvert.DeserializeObject<Response>(response);

                    if(res.code == 0)
                    {
                        Debug.Log(res.response);
                        _persistantData.Clean();
                        UIControls.ShowLayout("Default");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error parsing JSON: " + ex.Message);
                    return;
                }
            }
        }));
    }

    public void GetLeaderboard(Action<List<LeaderBoardEntryData>> callback, string stat = "zooCoins")
    {
        List<LeaderBoardEntryData> LeaderboardData = new List<LeaderBoardEntryData>();

        StartCoroutine(SendAPIRequest("http://tapzoo.ddns.net:32300/leaderboard/" + stat, "GET", null, (string response) =>
        {
            if (response != null)
            {
                try
                {
                    Response res = JsonConvert.DeserializeObject<Response>(response);

                    string leaderboardJSON = res.msg["LeaderboardData"].ToString();

                    LeaderboardData = JsonConvert.DeserializeObject<List<LeaderBoardEntryData>>(leaderboardJSON);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error parsing JSON: " + ex.Message);
                    return;
                }
            }

            callback(LeaderboardData);
        }));
    }

    public void GetZoo(string id)
    {
        StartCoroutine(SendAPIRequest("http://tapzoo.ddns.net:32300/zoos/" + id, "GEt", null, (string response) =>
        {
            Debug.Log(response);
        }));
    }

    public void UpdatePlayer(string id, PlayerData playerData)
    {
        string json = JsonConvert.SerializeObject(playerData);

        WWWForm form = new WWWForm();
        form.AddField("data", json);

        StartCoroutine(SendAPIRequest("http://tapzoo.ddns.net:32300/users/" + id + "/update", "POST", form, (string response) =>
        {
            Debug.Log(response);
        }));
    }

    public void UpdateZoo(string id, ZooData zooData)
    {
        string json = JsonConvert.SerializeObject(zooData);

        WWWForm form = new WWWForm();
        form.AddField("data", json);

        StartCoroutine(SendAPIRequest("http://tapzoo.ddns.net:32300/zoos/" + id + "/update", "POST", form, (string response) =>
        {
            Debug.Log(response);
        }));
    }

    IEnumerator SendAPIRequest(string url, string method, WWWForm formData, Action<string> callback)
    {
        UnityWebRequest www;

        if (method == "POST") { www = UnityWebRequest.Post(url, formData); }
        else { www = UnityWebRequest.Get(url); }

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) 
        {
            Debug.Log(www.error);
            callback(null); 
        }
        else { callback(www.downloadHandler.text); }
    }
}
