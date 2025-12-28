//using UnityEngine;
//using Unity.Services.Core;
//using Unity.Services.Authentication;
//using Unity.Services.CloudSave;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System;

///**
// * Cloud Save Manager - Saves complete PlayerData to Unity Cloud
// * Includes name, level progress, fails, and stats
// */
//public class CloudSaveManager : MonoBehaviour
//{
//    private const string CONSTANT_PASSWORD = "Pass1234!";
//    private const string PLAYER_DATA_KEY = "PlayerData";

//    public static CloudSaveManager Instance { get; private set; }

//    private bool isInitialized = false;
//    public PlayerData CurrentPlayerData { get; private set; }

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);
//            Debug.Log("========================================");
//            Debug.Log("[CloudSave] Manager created - READY");
//            Debug.Log("========================================");
//        }
//        else
//        {
//            Debug.Log("[CloudSave] Duplicate found, destroying");
//            Destroy(gameObject);
//        }
//    }

//    async void Start()
//    {
//        await InitializeUnityServices();
//    }

//    /**
//     * Initialize Unity Services
//     */
//    private async Task InitializeUnityServices()
//    {
//        if (isInitialized)
//        {
//            Debug.Log("[CloudSave] Already initialized");
//            return;
//        }

//        try
//        {
//            Debug.Log("[CloudSave] Initializing Unity Services...");
//            await UnityServices.InitializeAsync();
//            isInitialized = true;
//            Debug.Log("[CloudSave] ✓✓✓ Unity Services initialized SUCCESS ✓✓✓");
//        }
//        catch (Exception e)
//        {
//            Debug.LogError($"[CloudSave] ✗ Init failed: {e.Message}");
//            isInitialized = false;
//        }
//    }

//    /**
//     * REGISTER - Create NEW account
//     */
//    public async Task<string> RegisterWithUsername(string username)
//    {
//        Debug.Log("========================================");
//        Debug.Log($"[CloudSave] REGISTER REQUEST for: '{username}'");
//        Debug.Log("========================================");

//        try
//        {
//            if (!isInitialized)
//            {
//                await InitializeUnityServices();
//            }

//            Debug.Log($"[CloudSave] Creating account...");

//            // Create account
//            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, CONSTANT_PASSWORD);

//            Debug.Log("========================================");
//            Debug.Log($"[CloudSave] ✓✓✓ REGISTER SUCCESS ✓✓✓");
//            Debug.Log($"[CloudSave] Player ID: {AuthenticationService.Instance.PlayerId}");
//            Debug.Log("========================================");

//            // Create new player data with the username
//            CurrentPlayerData = new PlayerData(username);
//            CurrentPlayerData.playerName = username; // Make sure name is set!
//            Debug.Log($"[CloudSave] Created PlayerData for: {username}");
//            await SavePlayerData();

//            return $"Register successful! Player ID: {AuthenticationService.Instance.PlayerId}";
//        }
//        catch (AuthenticationException ex)
//        {
//            Debug.LogError($"[CloudSave] ✗ Register failed: {ex.Message}");
//            return $"Register failed: {ex.Message}";
//        }
//        catch (Exception ex)
//        {
//            Debug.LogError($"[CloudSave] ✗ Unexpected error: {ex.Message}");
//            return $"Unexpected error: {ex.Message}";
//        }
//    }

//    /**
//     * LOGIN - Sign into EXISTING account
//     */
//    public async Task<string> LoginWithUsername(string username)
//    {
//        Debug.Log("========================================");
//        Debug.Log($"[CloudSave] LOGIN REQUEST for: '{username}'");
//        Debug.Log("========================================");

//        try
//        {
//            if (!isInitialized)
//            {
//                await InitializeUnityServices();
//            }

//            Debug.Log($"[CloudSave] Signing in...");

//            // Sign in
//            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, CONSTANT_PASSWORD);

//            Debug.Log("========================================");
//            Debug.Log($"[CloudSave] ✓✓✓ LOGIN SUCCESS ✓✓✓");
//            Debug.Log($"[CloudSave] Player ID: {AuthenticationService.Instance.PlayerId}");
//            Debug.Log("========================================");

//            // Load player data from cloud
//            await LoadPlayerData();

//            return $"Login successful! Player ID: {AuthenticationService.Instance.PlayerId}";
//        }
//        catch (AuthenticationException ex)
//        {
//            Debug.LogError($"[CloudSave] ✗ Login failed: {ex.Message}");
//            return $"Login failed: {ex.Message}";
//        }
//        catch (Exception ex)
//        {
//            Debug.LogError($"[CloudSave] ✗ Unexpected error: {ex.Message}");
//            return $"Unexpected error: {ex.Message}";
//        }
//    }

//    /**
//     * Save complete player data to cloud
//     */
//    public async Task SavePlayerData()
//    {
//        if (!AuthenticationService.Instance.IsSignedIn)
//        {
//            Debug.LogWarning("[CloudSave] Cannot save: Not signed in");
//            return;
//        }

//        if (CurrentPlayerData == null)
//        {
//            Debug.LogWarning("[CloudSave] Cannot save: PlayerData is null");
//            return;
//        }

//        try
//        {
//            Debug.Log("[CloudSave] Saving player data to cloud...");

//            string json = CurrentPlayerData.ToJson();
//            var data = new Dictionary<string, object> { { PLAYER_DATA_KEY, json } };

//            await CloudSaveService.Instance.Data.Player.SaveAsync(data);

//            Debug.Log("[CloudSave] ✓ Player data saved to cloud");
//            CurrentPlayerData.PrintSummary();
//        }
//        catch (Exception ex)
//        {
//            Debug.LogError($"[CloudSave] ✗ Save failed: {ex.Message}");
//        }
//    }

//    /**
//     * Load player data from cloud
//     */
//    public async Task LoadPlayerData()
//    {
//        if (!AuthenticationService.Instance.IsSignedIn)
//        {
//            Debug.LogWarning("[CloudSave] Cannot load: Not signed in");
//            return;
//        }

//        try
//        {
//            Debug.Log("[CloudSave] Loading player data from cloud...");

//            var data = await CloudSaveService.Instance.Data.Player.LoadAsync(
//                new HashSet<string> { PLAYER_DATA_KEY }
//            );

//            if (data.TryGetValue(PLAYER_DATA_KEY, out var playerDataItem))
//            {
//                string json = playerDataItem.Value.GetAsString();
//                CurrentPlayerData = PlayerData.FromJson(json);

//                // Fix player name if it's missing or default
//                if (string.IsNullOrEmpty(CurrentPlayerData.playerName) || CurrentPlayerData.playerName == "Player")
//                {
//                    // Try to get from Authentication service
//                    if (!string.IsNullOrEmpty(AuthenticationService.Instance.PlayerName))
//                    {
//                        CurrentPlayerData.playerName = AuthenticationService.Instance.PlayerName;
//                    }
//                }

//                Debug.Log("[CloudSave] ✓ Player data loaded from cloud");
//                CurrentPlayerData.PrintSummary();
//            }
//            else
//            {
//                Debug.Log("[CloudSave] No saved data found, creating new PlayerData");
//                string username = AuthenticationService.Instance.PlayerName ?? "Player";
//                CurrentPlayerData = new PlayerData(username);
//                CurrentPlayerData.playerName = username;
//            }
//        }
//        catch (Exception ex)
//        {
//            Debug.LogError($"[CloudSave] ✗ Load failed: {ex.Message}");
//            // Create default player data as fallback
//            CurrentPlayerData = new PlayerData("Player");
//        }
//    }

//    /**
//     * Update current level and save to cloud
//     */
//    public async Task UpdateCurrentLevel(int levelIndex)
//    {
//        if (CurrentPlayerData == null)
//        {
//            Debug.LogWarning("[CloudSave] Cannot update level: PlayerData is null");
//            return;
//        }

//        Debug.Log($"[CloudSave] Updating current level to {levelIndex}");
//        CurrentPlayerData.UpdateLevel(levelIndex);
//        await SavePlayerData();
//    }

//    /**
//     * Record a fail/death and save to cloud
//     */
//    public async Task RecordFail(int levelIndex)
//    {
//        if (CurrentPlayerData == null)
//        {
//            Debug.LogWarning("[CloudSave] Cannot record fail: PlayerData is null");
//            return;
//        }

//        Debug.Log($"[CloudSave] Recording fail in level {levelIndex}");
//        CurrentPlayerData.RecordFail(levelIndex);
//        await SavePlayerData();
//    }

//    /**
//     * Get current level index
//     */
//    public int GetCurrentLevel()
//    {
//        if (CurrentPlayerData != null)
//        {
//            return CurrentPlayerData.currentLevelIndex;
//        }
//        return 2; // Default: Tutorial1
//    }

//    /**
//     * Get highest level reached
//     */
//    public int GetHighestLevel()
//    {
//        if (CurrentPlayerData != null)
//        {
//            return CurrentPlayerData.highestLevelReached;
//        }
//        return 2;
//    }

//    /**
//     * Get fails for a specific level
//     */
//    public int GetFailsForLevel(int levelIndex)
//    {
//        if (CurrentPlayerData != null)
//        {
//            return CurrentPlayerData.GetFailsForLevel(levelIndex);
//        }
//        return 0;
//    }

//    /**
//     * Update play time
//     */
//    public void UpdatePlayTime(float timeToAdd)
//    {
//        if (CurrentPlayerData != null)
//        {
//            //CurrentPlayerData.totalPlayTime += timeToAdd;
//            //Debug.Log($"[CloudSave] Play time updated: +{timeToAdd:F1}s (Total: {CurrentPlayerData.totalPlayTime:F1}s)");
//        }
//    }

//    /**
//     * Save play time to cloud
//     */
//    public async System.Threading.Tasks.Task SavePlayTime()
//    {
//        await SavePlayerData();
//    }

//    public void SignOut()
//    {
//        if (AuthenticationService.Instance.IsSignedIn)
//        {
//            AuthenticationService.Instance.SignOut();
//            CurrentPlayerData = null;
//            Debug.Log("[CloudSave] Signed out");
//        }
//    }

//    public bool IsSignedIn()
//    {
//        return AuthenticationService.Instance != null && AuthenticationService.Instance.IsSignedIn;
//    }

//    public string GetPlayerName()
//    {
//        if (CurrentPlayerData != null)
//        {
//            return CurrentPlayerData.playerName;
//        }
//        return "";
//    }
//}

using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

/**
 * Cloud Save Manager - Saves complete PlayerData to Unity Cloud
 * Includes name, level progress, fails, and stats
 */
public class CloudSaveManager : MonoBehaviour
{
    private const string CONSTANT_PASSWORD = "Pass1234!";
    private const string PLAYER_DATA_KEY = "PlayerData";

    public static CloudSaveManager Instance { get; private set; }

    private bool isInitialized = false;
    public PlayerData CurrentPlayerData { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("========================================");
            Debug.Log("[CloudSave] Manager created - READY");
            Debug.Log("========================================");
        }
        else
        {
            Debug.Log("[CloudSave] Duplicate found, destroying");
            Destroy(gameObject);
        }
    }

    async void Start()
    {
        await InitializeUnityServices();
    }

    /**
     * Initialize Unity Services
     */
    private async Task InitializeUnityServices()
    {
        if (isInitialized)
        {
            Debug.Log("[CloudSave] Already initialized");
            return;
        }

        try
        {
            Debug.Log("[CloudSave] Initializing Unity Services...");
            await UnityServices.InitializeAsync();
            isInitialized = true;
            Debug.Log("[CloudSave] ✓✓✓ Unity Services initialized SUCCESS ✓✓✓");
        }
        catch (Exception e)
        {
            Debug.LogError($"[CloudSave] ✗ Init failed: {e.Message}");
            isInitialized = false;
        }
    }

    /**
     * REGISTER - Create NEW account
     */
    public async Task<string> RegisterWithUsername(string username)
    {
        Debug.Log("========================================");
        Debug.Log($"[CloudSave] REGISTER REQUEST for: '{username}'");
        Debug.Log("========================================");

        try
        {
            if (!isInitialized)
            {
                await InitializeUnityServices();
            }

            Debug.Log($"[CloudSave] Creating account...");

            // Create account
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, CONSTANT_PASSWORD);

            Debug.Log("========================================");
            Debug.Log($"[CloudSave] ✓✓✓ REGISTER SUCCESS ✓✓✓");
            Debug.Log($"[CloudSave] Player ID: {AuthenticationService.Instance.PlayerId}");
            Debug.Log("========================================");

            // Create new player data with the username
            CurrentPlayerData = new PlayerData(username);
            CurrentPlayerData.playerName = username; // Make sure name is set!
            Debug.Log($"[CloudSave] Created PlayerData for: {username}");
            await SavePlayerData();

            return $"Register successful! Player ID: {AuthenticationService.Instance.PlayerId}";
        }
        catch (AuthenticationException ex)
        {
            Debug.LogError($"[CloudSave] ✗ Register failed: {ex.Message}");
            return $"Register failed: {ex.Message}";
        }
        catch (Exception ex)
        {
            Debug.LogError($"[CloudSave] ✗ Unexpected error: {ex.Message}");
            return $"Unexpected error: {ex.Message}";
        }
    }

    /**
     * LOGIN - Sign into EXISTING account
     */
    public async Task<string> LoginWithUsername(string username)
    {
        Debug.Log("========================================");
        Debug.Log($"[CloudSave] LOGIN REQUEST for: '{username}'");
        Debug.Log("========================================");

        try
        {
            if (!isInitialized)
            {
                await InitializeUnityServices();
            }

            Debug.Log($"[CloudSave] Signing in...");

            // Sign in
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, CONSTANT_PASSWORD);

            Debug.Log("========================================");
            Debug.Log($"[CloudSave] ✓✓✓ LOGIN SUCCESS ✓✓✓");
            Debug.Log($"[CloudSave] Player ID: {AuthenticationService.Instance.PlayerId}");
            Debug.Log("========================================");

            // Load player data from cloud
            await LoadPlayerData();

            return $"Login successful! Player ID: {AuthenticationService.Instance.PlayerId}";
        }
        catch (AuthenticationException ex)
        {
            Debug.LogError($"[CloudSave] ✗ Login failed: {ex.Message}");
            return $"Login failed: {ex.Message}";
        }
        catch (Exception ex)
        {
            Debug.LogError($"[CloudSave] ✗ Unexpected error: {ex.Message}");
            return $"Unexpected error: {ex.Message}";
        }
    }

    /**
     * Save complete player data to cloud
     */
    public async Task SavePlayerData()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.LogWarning("[CloudSave] Cannot save: Not signed in");
            return;
        }

        if (CurrentPlayerData == null)
        {
            Debug.LogWarning("[CloudSave] Cannot save: PlayerData is null");
            return;
        }

        try
        {
            Debug.Log("[CloudSave] Saving player data to cloud...");

            string json = CurrentPlayerData.ToJson();
            var data = new Dictionary<string, object> { { PLAYER_DATA_KEY, json } };

            await CloudSaveService.Instance.Data.Player.SaveAsync(data);

            Debug.Log("[CloudSave] ✓ Player data saved to cloud");
            CurrentPlayerData.PrintSummary();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[CloudSave] ✗ Save failed: {ex.Message}");
        }
    }

    /**
     * Load player data from cloud
     */
    public async Task LoadPlayerData()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.LogWarning("[CloudSave] Cannot load: Not signed in");
            return;
        }

        try
        {
            Debug.Log("[CloudSave] Loading player data from cloud...");

            var data = await CloudSaveService.Instance.Data.Player.LoadAsync(
                new HashSet<string> { PLAYER_DATA_KEY }
            );

            if (data.TryGetValue(PLAYER_DATA_KEY, out var playerDataItem))
            {
                string json = playerDataItem.Value.GetAsString();
                CurrentPlayerData = PlayerData.FromJson(json);

                // Fix player name if it's missing or default
                if (string.IsNullOrEmpty(CurrentPlayerData.playerName) || CurrentPlayerData.playerName == "Player")
                {
                    // Try to get from Authentication service
                    if (!string.IsNullOrEmpty(AuthenticationService.Instance.PlayerName))
                    {
                        CurrentPlayerData.playerName = AuthenticationService.Instance.PlayerName;
                    }
                }

                Debug.Log("[CloudSave] ✓ Player data loaded from cloud");
                CurrentPlayerData.PrintSummary();
            }
            else
            {
                Debug.Log("[CloudSave] No saved data found, creating new PlayerData");
                string username = AuthenticationService.Instance.PlayerName ?? "Player";
                CurrentPlayerData = new PlayerData(username);
                CurrentPlayerData.playerName = username;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[CloudSave] ✗ Load failed: {ex.Message}");
            // Create default player data as fallback
            CurrentPlayerData = new PlayerData("Player");
        }
    }

    /**
     * Update current level and save to cloud
     */
    public async Task UpdateCurrentLevel(int levelIndex)
    {
        if (CurrentPlayerData == null)
        {
            Debug.LogWarning("[CloudSave] Cannot update level: PlayerData is null");
            return;
        }

        Debug.Log($"[CloudSave] Updating current level to {levelIndex}");
        CurrentPlayerData.UpdateLevel(levelIndex);
        await SavePlayerData();
    }

    /**
     * Record a fail/death and save to cloud
     */
    public async Task RecordFail(int levelIndex)
    {
        if (CurrentPlayerData == null)
        {
            Debug.LogWarning("[CloudSave] Cannot record fail: PlayerData is null");
            return;
        }

        Debug.Log($"[CloudSave] Recording fail in level {levelIndex}");
        CurrentPlayerData.RecordFail(levelIndex);
        await SavePlayerData();
    }

    /**
     * Get current level index
     */
    public int GetCurrentLevel()
    {
        if (CurrentPlayerData != null)
        {
            return CurrentPlayerData.currentLevelIndex;
        }
        return 2; // Default: Tutorial1
    }

    /**
     * Get highest level reached
     */
    public int GetHighestLevel()
    {
        if (CurrentPlayerData != null)
        {
            return CurrentPlayerData.highestLevelReached;
        }
        return 2;
    }

    /**
     * Get fails for a specific level
     */
    public int GetFailsForLevel(int levelIndex)
    {
        if (CurrentPlayerData != null)
        {
            return CurrentPlayerData.GetFailsForLevel(levelIndex);
        }
        return 0;
    }

    /**
     * Update play time
     */
    public void UpdatePlayTime(float timeToAdd)
    {
        if (CurrentPlayerData != null)
        {
            //CurrentPlayerData.totalPlayTime += timeToAdd;
            //Debug.Log($"[CloudSave] Play time updated: +{timeToAdd:F1}s (Total: {CurrentPlayerData.totalPlayTime:F1}s)");
        }
    }

    /**
     * Save play time to cloud
     */
    public async System.Threading.Tasks.Task SavePlayTime()
    {
        await SavePlayerData();
    }

    /**
     * Complete the game - Update best score
     * Call this when player finishes the LAST level
     */
    public async System.Threading.Tasks.Task CompleteGame()
    {
        if (CurrentPlayerData == null)
        {
            Debug.LogWarning("[CloudSave] Cannot complete game: PlayerData is null");
            return;
        }

        Debug.Log("[CloudSave] 🏆 GAME COMPLETED! 🏆");
        CurrentPlayerData.CompleteGame();
        await SavePlayerData();
    }

    /**
     * Start a new game - Reset current playthrough
     * Call this when player clicks "New Game" or restarts
     */
    public async System.Threading.Tasks.Task StartNewGame()
    {
        if (CurrentPlayerData == null)
        {
            Debug.LogWarning("[CloudSave] Cannot start new game: PlayerData is null");
            return;
        }

        Debug.Log("[CloudSave] Starting new game...");
        CurrentPlayerData.StartNewGame();
        await SavePlayerData();
    }

    /**
     * Get best score (lowest deaths from completed game)
     */
    public int GetBestScore()
    {
        if (CurrentPlayerData != null)
        {
            return CurrentPlayerData.bestScore;
        }
        return -1;
    }

    /**
     * Get total completions
     */
    public int GetTotalCompletions()
    {
        if (CurrentPlayerData != null)
        {
            return CurrentPlayerData.totalCompletions;
        }
        return 0;
    }

    /**
     * Get current game total deaths
     */
    public int GetCurrentGameDeaths()
    {
        if (CurrentPlayerData != null)
        {
            return CurrentPlayerData.totalDeaths;
        }
        return 0;
    }

    public void SignOut()
    {
        if (AuthenticationService.Instance.IsSignedIn)
        {
            AuthenticationService.Instance.SignOut();
            CurrentPlayerData = null;
            Debug.Log("[CloudSave] Signed out");
        }
    }

    public bool IsSignedIn()
    {
        return AuthenticationService.Instance != null && AuthenticationService.Instance.IsSignedIn;
    }

    public string GetPlayerName()
    {
        if (CurrentPlayerData != null)
        {
            return CurrentPlayerData.playerName;
        }
        return "";
    }
}