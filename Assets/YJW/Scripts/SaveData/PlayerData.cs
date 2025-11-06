using UnityEngine;
using System.IO;
using NUnit.Framework;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int combo;
}

public class PlayerDataList
{
    public List<PlayerData> players = new List<PlayerData>();
}