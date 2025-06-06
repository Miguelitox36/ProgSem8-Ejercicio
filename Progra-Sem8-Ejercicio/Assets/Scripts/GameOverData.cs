using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameOverData
{
    public static Stack<string> powerUpHistory = new Stack<string>();
    public static Dictionary<string, bool> trophies = new Dictionary<string, bool>(); 

    public static void Save(int kills, float time, int level)
    {
        try
        {
            PlayerPrefs.SetInt("Kills", kills);
            PlayerPrefs.SetFloat("Time", time);
            PlayerPrefs.SetInt("Level", level);

            
            foreach (var entry in trophies)
            {
                PlayerPrefs.SetInt("Trophy_" + entry.Key, entry.Value ? 1 : 0);
            }
            PlayerPrefs.Save();
            Debug.Log("Datos de juego guardados: Kills=" + kills + ", Time=" + time.ToString("F2") + ", Level=" + level);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al guardar datos de juego: " + ex.Message);
        }
    }

    public static (int kills, float time, int level) Load()
    {
        int kills = PlayerPrefs.GetInt("Kills", 0);
        float time = PlayerPrefs.GetFloat("Time", 0f);
        int level = PlayerPrefs.GetInt("Level", 0);
        Debug.Log("Datos de juego cargados: Kills=" + kills + ", Time=" + time.ToString("F2") + ", Level=" + level);                
        return (kills, time, level);
    }
    
    public static void UnlockTrophy(string trophyName)
    {
        if (trophies.ContainsKey(trophyName))
        {
            trophies[trophyName] = true;
        }
        else
        {
            trophies.Add(trophyName, true);
        }
        PlayerPrefs.SetInt("Trophy_" + trophyName, 1); 
        PlayerPrefs.Save();
        Debug.Log($"Trofeo '{trophyName}' desbloqueado localmente.");
    }
    
    public static bool IsTrophyUnlocked(string trophyName)
    {
        return trophies.ContainsKey(trophyName) && trophies[trophyName];
    }
}