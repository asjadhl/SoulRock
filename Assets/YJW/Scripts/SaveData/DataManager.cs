using System.IO;
using UnityEngine;

public class Datamanager : MonoBehaviour
{
    public static Datamanager instance;

    public PlayerData curPlayerData = new PlayerData();
    public PlayerDataList allData = new PlayerDataList();

    string path;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

#if UNITY_EDITOR
        string folder = Path.Combine(Application.dataPath, "Json");
#else
        string folder = Path.Combine(Application.dataPath, "../Json");
#endif

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        path = Path.Combine(folder, "PlayerData.json");
    }

    public void SaveToJson()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            allData = JsonUtility.FromJson<PlayerDataList>(json);
        }
        else
        {
            allData = new PlayerDataList();
        }

        allData.players.Add(curPlayerData);

        string newJson = JsonUtility.ToJson(allData, true);
        File.WriteAllText(path, newJson);

    }

    public void LoadFromJson()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            allData = JsonUtility.FromJson<PlayerDataList>(json);
        }
        else
        {
            allData = new PlayerDataList();
        }
    }
}


