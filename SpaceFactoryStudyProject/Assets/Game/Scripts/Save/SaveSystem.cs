using System;
using UnityEngine;

public class SaveSystem
{
    private const string SaveKey = "FactoryIdleSave_v1";

    public bool HasSave()
    {
        return PlayerPrefs.HasKey(SaveKey);
    }

    public void Save(SaveData data)
    {
        if (data == null) return;
        data.lastExitTicks = DateTime.UtcNow.Ticks;
        string json = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
        Debug.Log("[SaveSystem] Игра сохранена");
    }

    public SaveData Load()
    {
        if (!HasSave())
        {
            Debug.Log("[SaveSystem] Сейва нет — будет создан новый");
            return null;
        }
        string json = PlayerPrefs.GetString(SaveKey);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        Debug.Log("[SaveSystem] Сейв загружен");
        return data;
    }

    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
        Debug.Log("[SaveSystem] Сейв удалён");
    }
}