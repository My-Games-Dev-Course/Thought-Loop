//using System;
//using System.Collections.Generic;
//using UnityEngine;

///**
// * Stores all player data that will be saved to Unity Cloud
// * Includes level progress, name, and statistics
// */
//[Serializable]
//public class PlayerData
//{
//    public string playerName = "";
//    public int currentLevelIndex = 2;        // Last level player was on (2 = Tutorial1)
//    public int highestLevelReached = 2;      // Highest level ever reached
//    public Dictionary<int, int> failsPerLevel = new Dictionary<int, int>(); // Level index → fail count
//    public int totalDeaths = 0;              // Total deaths across all levels
//    //public float totalPlayTime = 0f;         // Total play time in seconds
//    public string lastPlayed = "";           // Last time player played

//    /**
//     * Constructor - creates new player data with default values
//     */
//    public PlayerData(string name = "Player")
//    {
//        playerName = name;
//        currentLevelIndex = 2;  // Tutorial1 is index 2
//        highestLevelReached = 2;
//        failsPerLevel = new Dictionary<int, int>();
//        totalDeaths = 0;
//        //totalPlayTime = 0f;
//        lastPlayed = DateTime.Now.ToString("o");
//    }

//    /**
//     * Record a death/fail in a specific level
//     */
//    public void RecordFail(int levelIndex)
//    {
//        if (failsPerLevel.ContainsKey(levelIndex))
//        {
//            failsPerLevel[levelIndex]++;
//        }
//        else
//        {
//            failsPerLevel[levelIndex] = 1;
//        }
//        totalDeaths++;
//        Debug.Log($"[PlayerData] Recorded fail in level {levelIndex}. Total fails this level: {failsPerLevel[levelIndex]}");
//    }

//    /**
//     * Get number of fails for a specific level
//     */
//    public int GetFailsForLevel(int levelIndex)
//    {
//        if (failsPerLevel.ContainsKey(levelIndex))
//        {
//            return failsPerLevel[levelIndex];
//        }
//        return 0;
//    }

//    /**
//     * Update current level (and highest reached if needed)
//     */
//    public void UpdateLevel(int newLevelIndex)
//    {
//        currentLevelIndex = newLevelIndex;
//        if (newLevelIndex > highestLevelReached)
//        {
//            highestLevelReached = newLevelIndex;
//            Debug.Log($"[PlayerData] New highest level reached: {highestLevelReached}");
//        }
//        lastPlayed = DateTime.Now.ToString("o");
//    }

//    /**
//     * Convert to JSON string for saving
//     */
//    public string ToJson()
//    {
//        // Create a serializable wrapper
//        var wrapper = new PlayerDataWrapper
//        {
//            playerName = this.playerName,
//            currentLevelIndex = this.currentLevelIndex,
//            highestLevelReached = this.highestLevelReached,
//            totalDeaths = this.totalDeaths,
//            //totalPlayTime = this.totalPlayTime,
//            lastPlayed = this.lastPlayed,
//            failsPerLevel = new List<FailEntry>()
//        };

//        // Convert dictionary to list for JSON serialization
//        foreach (var kvp in failsPerLevel)
//        {
//            wrapper.failsPerLevel.Add(new FailEntry { levelIndex = kvp.Key, fails = kvp.Value });
//        }

//        return JsonUtility.ToJson(wrapper);
//    }

//    /**
//     * Create PlayerData from JSON string (loaded from cloud)
//     */
//    public static PlayerData FromJson(string json)
//    {
//        var wrapper = JsonUtility.FromJson<PlayerDataWrapper>(json);
//        var playerData = new PlayerData
//        {
//            playerName = wrapper.playerName,
//            currentLevelIndex = wrapper.currentLevelIndex,
//            highestLevelReached = wrapper.highestLevelReached,
//            totalDeaths = wrapper.totalDeaths,
//            //totalPlayTime = wrapper.totalPlayTime,
//            lastPlayed = wrapper.lastPlayed,
//            failsPerLevel = new Dictionary<int, int>()
//        };

//        // Convert list back to dictionary
//        foreach (var entry in wrapper.failsPerLevel)
//        {
//            playerData.failsPerLevel[entry.levelIndex] = entry.fails;
//        }

//        return playerData;
//    }

//    /**
//     * Print summary for debugging
//     */
//    public void PrintSummary()
//    {
//        Debug.Log("========================================");
//        Debug.Log($"[PlayerData] Player: {playerName}");
//        Debug.Log($"[PlayerData] Current Level: {currentLevelIndex}");
//        Debug.Log($"[PlayerData] Highest Level: {highestLevelReached}");
//        Debug.Log($"[PlayerData] Total Deaths: {totalDeaths}");
//        //Debug.Log($"[PlayerData] Play Time: {totalPlayTime:F1}s");
//        Debug.Log($"[PlayerData] Fails per level:");
//        foreach (var kvp in failsPerLevel)
//        {
//            Debug.Log($"  Level {kvp.Key}: {kvp.Value} fails");
//        }
//        Debug.Log("========================================");
//    }
//}

///**
// * Wrapper class for JSON serialization (JsonUtility doesn't support Dictionary)
// */
//[Serializable]
//public class PlayerDataWrapper
//{
//    public string playerName;
//    public int currentLevelIndex;
//    public int highestLevelReached;
//    public int totalDeaths;
//    //public float totalPlayTime;
//    public string lastPlayed;
//    public List<FailEntry> failsPerLevel = new List<FailEntry>();
//}

///**
// * Entry for fails per level
// */
//[Serializable]
//public class FailEntry
//{
//    public int levelIndex;
//    public int fails;
//}

using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * Stores all player data that will be saved to Unity Cloud
 * Includes level progress, name, and statistics
 */
[Serializable]
public class PlayerData
{
    public string playerName = "";
    public int currentLevelIndex = 2;        // Last level player was on (2 = Tutorial1)
    public int highestLevelReached = 2;      // Highest level ever reached
    public Dictionary<int, int> failsPerLevel = new Dictionary<int, int>(); // Level index → fail count
    public int totalDeaths = 0;              // Deaths in CURRENT playthrough
    public int bestScore = -1;               // BEST (lowest) deaths from completed game (-1 = never completed)
    public int totalCompletions = 0;         // How many times game was completed
    //public float totalPlayTime = 0f;         // Total play time in seconds
    public string lastPlayed = "";           // Last time player played

    /**
     * Constructor - creates new player data with default values
     */
    public PlayerData(string name = "Player")
    {
        playerName = name;
        currentLevelIndex = 2;  // Tutorial1 is index 2
        highestLevelReached = 2;
        failsPerLevel = new Dictionary<int, int>();
        totalDeaths = 0;
        bestScore = -1;  // -1 means never completed the game
        totalCompletions = 0;
        //totalPlayTime = 0f;
        lastPlayed = DateTime.Now.ToString("o");
    }

    /**
     * Record a death/fail in a specific level
     */
    public void RecordFail(int levelIndex)
    {
        if (failsPerLevel.ContainsKey(levelIndex))
        {
            failsPerLevel[levelIndex]++;
        }
        else
        {
            failsPerLevel[levelIndex] = 1;
        }
        totalDeaths++;
        Debug.Log($"[PlayerData] Recorded fail in level {levelIndex}. Total fails this level: {failsPerLevel[levelIndex]}");
    }

    /**
     * Get number of fails for a specific level
     */
    public int GetFailsForLevel(int levelIndex)
    {
        if (failsPerLevel.ContainsKey(levelIndex))
        {
            return failsPerLevel[levelIndex];
        }
        return 0;
    }

    /**
     * Update current level (and highest reached if needed)
     */
    public void UpdateLevel(int newLevelIndex)
    {
        currentLevelIndex = newLevelIndex;
        if (newLevelIndex > highestLevelReached)
        {
            highestLevelReached = newLevelIndex;
            Debug.Log($"[PlayerData] New highest level reached: {highestLevelReached}");
        }
        lastPlayed = DateTime.Now.ToString("o");
    }

    /**
     * Complete the game - Update best score
     */
    public void CompleteGame()
    {
        totalCompletions++;

        // Update best score if this is first completion or new record
        if (bestScore == -1 || totalDeaths < bestScore)
        {
            int oldBest = bestScore;
            bestScore = totalDeaths;
            Debug.Log($"[PlayerData] 🏆 NEW BEST SCORE! Deaths: {bestScore} (Previous: {(oldBest == -1 ? "Never completed" : oldBest.ToString())})");
        }
        else
        {
            Debug.Log($"[PlayerData] Game completed! Deaths: {totalDeaths} (Best: {bestScore})");
        }

        Debug.Log($"[PlayerData] Total completions: {totalCompletions}");
        lastPlayed = DateTime.Now.ToString("o");
    }

    /**
     * Start a new game - Reset current playthrough stats
     */
    public void StartNewGame()
    {
        Debug.Log("[PlayerData] ========================================");
        Debug.Log("[PlayerData] STARTING NEW GAME");
        Debug.Log($"[PlayerData] Previous run - Deaths: {totalDeaths}, Best: {(bestScore == -1 ? "N/A" : bestScore.ToString())}");

        // Reset current playthrough
        currentLevelIndex = 2; // Back to Tutorial1
        totalDeaths = 0;
        failsPerLevel.Clear();

        Debug.Log("[PlayerData] Current game stats reset!");
        Debug.Log("[PlayerData] ========================================");
        lastPlayed = DateTime.Now.ToString("o");
    }

    /**
     * Convert to JSON string for saving
     */
    public string ToJson()
    {
        // Create a serializable wrapper
        var wrapper = new PlayerDataWrapper
        {
            playerName = this.playerName,
            currentLevelIndex = this.currentLevelIndex,
            highestLevelReached = this.highestLevelReached,
            totalDeaths = this.totalDeaths,
            bestScore = this.bestScore,
            totalCompletions = this.totalCompletions,
            //totalPlayTime = this.totalPlayTime,
            lastPlayed = this.lastPlayed,
            failsPerLevel = new List<FailEntry>()
        };

        // Convert dictionary to list for JSON serialization
        foreach (var kvp in failsPerLevel)
        {
            wrapper.failsPerLevel.Add(new FailEntry { levelIndex = kvp.Key, fails = kvp.Value });
        }

        return JsonUtility.ToJson(wrapper);
    }

    /**
     * Create PlayerData from JSON string (loaded from cloud)
     */
    public static PlayerData FromJson(string json)
    {
        var wrapper = JsonUtility.FromJson<PlayerDataWrapper>(json);
        var playerData = new PlayerData
        {
            playerName = wrapper.playerName,
            currentLevelIndex = wrapper.currentLevelIndex,
            highestLevelReached = wrapper.highestLevelReached,
            totalDeaths = wrapper.totalDeaths,
            bestScore = wrapper.bestScore,
            totalCompletions = wrapper.totalCompletions,
            //totalPlayTime = wrapper.totalPlayTime,
            lastPlayed = wrapper.lastPlayed,
            failsPerLevel = new Dictionary<int, int>()
        };

        // Convert list back to dictionary
        foreach (var entry in wrapper.failsPerLevel)
        {
            playerData.failsPerLevel[entry.levelIndex] = entry.fails;
        }

        return playerData;
    }

    /**
     * Print summary for debugging
     */
    public void PrintSummary()
    {
        Debug.Log("========================================");
        Debug.Log($"[PlayerData] Player: {playerName}");
        Debug.Log($"[PlayerData] Current Level: {currentLevelIndex}");
        Debug.Log($"[PlayerData] Highest Level: {highestLevelReached}");
        Debug.Log($"[PlayerData] Current Game Deaths: {totalDeaths}");
        Debug.Log($"[PlayerData] Best Score: {(bestScore == -1 ? "Never completed" : bestScore.ToString())}");
        Debug.Log($"[PlayerData] Total Completions: {totalCompletions}");
        //Debug.Log($"[PlayerData] Play Time: {totalPlayTime:F1}s");
        Debug.Log($"[PlayerData] Fails per level:");
        foreach (var kvp in failsPerLevel)
        {
            Debug.Log($"  Level {kvp.Key}: {kvp.Value} fails");
        }
        Debug.Log("========================================");
    }
}

/**
 * Wrapper class for JSON serialization (JsonUtility doesn't support Dictionary)
 */
[Serializable]
public class PlayerDataWrapper
{
    public string playerName;
    public int currentLevelIndex;
    public int highestLevelReached;
    public int totalDeaths;
    public int bestScore;
    public int totalCompletions;
    //public float totalPlayTime;
    public string lastPlayed;
    public List<FailEntry> failsPerLevel = new List<FailEntry>();
}

/**
 * Entry for fails per level
 */
[Serializable]
public class FailEntry
{
    public int levelIndex;
    public int fails;
}