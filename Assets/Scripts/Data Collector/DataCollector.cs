using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using UnityEngine.UIElements;
using TMPro;

[Serializable]
public class PlayerData
{
    //Game Description Data
    public string game_id;
    public string theme="Jungle";
    public string visual="Light";
    public string completion_type="Endless";
    public string genre="Casual";
    public string subgenere="HyperCasual";
    public string type="Platformer";
    public string prompt= "Create a C# game set in a dark and light jungle with gradient visuals and a retro art style, players must navigate through endless levels with reflex and spatial awareness skills. Using a joystick for controls, a fixed camera focuses on the center, providing a 2D horizontal perspective. The gameplay offers a hypercasual experience.";
    public string skill_tagged="Reflex spatial awareness";
    public string perspective= "Fixed camera focused on the center";
    public string orientation="Horizontal";
    public string dimension="2D";
    public string playtime="5 mins average";
    public string cohort_id="cohort_id";
    public string mechanics="mechanics of the game";
    public string controls="Touch inputs";
    public string screen_movement="Horizontal";
    public string game_mode="Casual";
    public string game_type="Hypercasual";
    public string duration="2d";
    public string game_version="v1.1";
    public string art_style="Retro";
    public string skill="reflex";
    public string sub_skill="spatial awareness";
    public string gnere_desc="A mario type 2d platformer game in a jungle";

    //Game metrics data
    public int collectives_collected; //
    public int obstacles_destroyed;//
    public float distance_travelled;
    public int score;//
    public float time_played;
    public float obstacle_generation_chance;  //0-1 
    public float platform_generation_chance;  //0-1
    public int collectible_generation_chance_in_platform; //1-5 
    public int obstacle_generation_chance_in_platform; //1-5 
    public float playerSpeed;
    public int jumpCount;
    public int dashCount;
    public int no_of_lives_given = 3;
    public int no_of_attempts_provided_before_skip=5;

    //Average metrics data 
    public int avg_player_count;
    public int avg_powerup_used;
    public int avg_platform_generation_chance;
    public int avg_obstalce_generation_chance;
    public int avg_collectible_generation_chance_in_platform;
    public int avg_obstacle_generation_chance_in_platform;
    public float avg_distance_travelled;
    public int avg_time_spent;
    public float avg_player_speed;
    public int avg_collectibles_collected;
    public int avg_obstacles_destroyed;
    public int avg_score;
}
[Serializable]
public class Login
{
    public string username;
    public string password;
}

[Serializable]
public class Token
{
   public string access_token;
    public string token_type;
}
public class DataCollector : MonoBehaviour
{
    [Serializable]
    public class DataOnEnd
    {
        public string start_time;
        public string end_time;

        public string game_name;
        public string data;
    }

    #region singleton 

    private static DataCollector instance;

    // Public property to access the singleton instance
    public static DataCollector Instance
    {
        get
        {
            // If the instance is null, try to find an existing instance in the scene
            if (instance == null)
            {
                instance = FindObjectOfType<DataCollector>();

                // If no instance exists in the scene, create a new GameObject and attach the script to it
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(DataCollector).Name);
                    instance = singletonObject.AddComponent<DataCollector>();
                }

                // Ensure that the singleton instance persists across scene changes
                //DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }
    private void Awake()
    {
        // If an instance already exists in the scene and it's not the current instance, destroy it
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
    }

    #endregion

    [SerializeField] private PlayerData gameData = new PlayerData();
    private float timePlayed = 0;
    // Path to the JSON file
    private string filePath;
    public Player _player;
    private void OnEnable()
    {
        if (isDataEntered)
        {
            _inputFields.SetActive(false);
        }
    }
    private void Start()
    {
        // Set the file path to the persistent data path
        filePath = Application.dataPath;
        filePath = Path.Combine(filePath, "GameData.json");

  
    }

    private void Update()
    {
        timePlayed += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.B))
        {
            SaveGameData();
        }
    }

    // Function to save the game data to a JSON file
    public event Action SaveDataEvent;
    public async void SaveGameData()
    {
        if (SaveDataEvent != null) SaveDataEvent();

        await Task.Delay(1000);
        gameData.time_played = timePlayed;
        CalculateAverages();

        string Data = JsonUtility.ToJson(gameData);
        File.WriteAllText(filePath, Data);

        UploadData();

    }
    public void CalculateAverages()
    {
        //CALCULATE ALL THE AVERAGES OF THE DATA.
        gameData.avg_player_count = 1;
        gameData.avg_powerup_used = 1;
        gameData.avg_platform_generation_chance = 1;
        gameData.avg_obstalce_generation_chance = 1;
        gameData.avg_collectible_generation_chance_in_platform = 1;
        gameData.avg_obstacle_generation_chance_in_platform = 1;
        gameData.avg_distance_travelled = 1;
        gameData.avg_time_spent = 1;
        gameData.avg_player_speed = 1;
        gameData.avg_collectibles_collected = 1;
        gameData.avg_obstacles_destroyed = 1;
        gameData.avg_score = 1;
    }

    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public GameObject _inputFields;
    public void GetData()
    {
        username = usernameField.text;
        password = passwordField.text;
        isDataEntered = true;

    }
    public static bool isDataEntered;

    public string username="random123";
    public string password="pass123";
    public async void UploadData()
    {
        const string URL = "http://34.27.103.197";
         //username = "random123";
        // password = "pass123";
        UnityWebRequest auth = new UnityWebRequest($"{URL}/user/auth/login", "POST");
        Login l = new Login();
        l.username = username;
        l.password = password;
        string lb = JsonUtility.ToJson(l);
        byte[] b = Encoding.ASCII.GetBytes(lb);
        UploadHandlerRaw uHR = new UploadHandlerRaw(b);
        uHR.contentType= "application/json";
        auth.uploadHandler= uHR;    
        auth.SetRequestHeader("Content-Type", "application/json");
        auth.downloadHandler = new DownloadHandlerBuffer();

        var _operation = auth.SendWebRequest();
        Debug.Log(_operation);
        while (!_operation.isDone)
            await Task.Yield();

        Debug.Log("142");
        if (auth.result == UnityWebRequest.Result.Success)
            Debug.Log($"Success: {auth}");
        else
            Debug.Log($"Error: {auth.error}");

        Token token = JsonUtility.FromJson<Token>(auth.downloadHandler.text);
        string TOKEN = token.access_token;

        Debug.Log("148");
        Debug.Log($"Status Code: {auth.responseCode}");
        //byte[] results = req.downloadHandler.data;
        Debug.Log(auth.downloadHandler.text);

        const string ENDPOINT = "/user/game/data";
       // const string TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJyYW5kb20xMjMiLCJleHAiOjE2ODk5MzQxNjB9.-rL6nDQeu80JWzfRWZaKUDjg4yKIJZG-xdSrSRmiGCw";

        DataOnEnd doe = new DataOnEnd();
        doe.start_time = DateTime.UtcNow.ToUniversalTime().ToString("yyyy/MM/dd HH/MM/ss");
        doe.end_time = DateTime.UtcNow.ToUniversalTime().ToString("yyyy/MM/dd HH/MM/ss"); 
        doe.game_name = "mario";
        Debug.Log(doe.start_time);
        Debug.Log(doe.end_time);
        doe.data = JsonUtility.ToJson(gameData);
        string body = JsonUtility.ToJson(doe);
        Debug.Log(body);
        UnityWebRequest req = new UnityWebRequest($"{URL}{ENDPOINT}", "POST");
        byte[] bytes = Encoding.ASCII.GetBytes(body);
        UploadHandlerRaw uH = new UploadHandlerRaw(bytes);
        uH.contentType = "application/json";
        req.uploadHandler = uH;

        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("Authorization", $"Bearer {TOKEN}");
        req.downloadHandler = new DownloadHandlerBuffer();
        Debug.Log("137");
        var operation = req.SendWebRequest();
        Debug.Log(operation);

        while (!operation.isDone)
            await Task.Yield();

        Debug.Log(req.downloadHandler);
        if (req.result == UnityWebRequest.Result.Success)
            Debug.Log($"Success: {req}");
        else
            Debug.Log($"Error: {req.error}");

        Debug.Log("148");
        Debug.Log($"Status Code: {req.responseCode}");
        //byte[] results = req.downloadHandler.data;
        Debug.Log(req.downloadHandler.text);

    }

    // Data Saving functions


}  