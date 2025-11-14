using System.Linq;
using UnityEngine;

public class ScoreBoardUI : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject scoreItemPrefab;

    private void Start()
    {
        RefreshScoreboard();
    }

    public void RefreshScoreboard()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);

        Datamanager.instance.LoadFromJson();

        var sorted = Datamanager.instance.allData.players
            .OrderByDescending(p => p.combo)
            .ToList();

        // UI 생성
        for (int i = 0; i < sorted.Count; i++)
        {
            CreateScoreItem(i + 1, sorted[i].playerName, sorted[i].combo);
        }
    }

    void CreateScoreItem(int rank, string name, int score)
    {
        GameObject obj = Instantiate(scoreItemPrefab, content);
        ScoreItem item = obj.GetComponent<ScoreItem>();  // ScoreItem 스크립트 필요

        item.rankText.text = rank.ToString();
        item.nameText.text = name + " Jackson";
        item.scoreText.text = score.ToString();
    }
}

