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
        }


        path = Application.persistentDataPath;
    }

    public void SaveToJson()
    {
        // 1. 기존 데이터 불러오기
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            allData = JsonUtility.FromJson<PlayerDataList>(json);
        }
        else
        {
            allData = new PlayerDataList();
        }

        // 2. 새로운 플레이어 데이터 추가
        allData.players.Add(curPlayerData);

        // 3. JSON으로 저장 (덮어쓰기)
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


