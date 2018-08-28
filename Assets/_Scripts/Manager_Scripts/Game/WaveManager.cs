/*Alex Greff
19/01/2016
WaveManager
Manages the enemy waves
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType {
    FIGHTER, TURRET
}

public enum EnemyLevel {
    LEVEL_1 = 0, LEVEL_2 = 1, LEVEL_3 = 2, LEVEL_4 = 3, LEVEL_5 = 4
}

//Set to serializable so it is able to be edited in the inspector
[Serializable] public class EnemyWave {
    [SerializeField] private List<EnemyGroupInfo> enemyGroups;

    //Property
    public List<EnemyGroupInfo> EnemyGroups {
        get {
            return enemyGroups;
        }
    }
    public int EnemyAmount {
        get {
            int count = 0;

            foreach (EnemyGroupInfo e in enemyGroups)
                count += e.Ammount;

            return count;
        }
    }

    public void SpawnWave () { //Spawns a wave
        foreach (EnemyGroupInfo e in enemyGroups) {
            if (e.SpawnDelay <= 0) {
                /*SpawnLocation randomLocation = WaveManager.instance.getRandomSpawnPos(e.Type);
                if (randomLocation == null) {
                    Debug.LogWarning("Unable to spawn enemy because no spawn points were found");
                    return;
                } */
                //SpawnEnemy(e, randomLocation.Use()); //Spawn the enemy in a random location
                SpawnEnemy(e); //Spawn the enemy in a random location
            }
            else {
                GameManager.Instance.StartChildCoroutine(spawnDelayedEnemy(e)); //Use a monodevelop script to run the coroutine
            }
        }
    }

    private IEnumerator spawnDelayedEnemy (EnemyGroupInfo enemy) { //Spawns a delay enemy
        yield return new WaitForSeconds(enemy.SpawnDelay); //Wait
        //SpawnEnemy(enemy, WaveManager.instance.getRandomSpawnPos(enemy.Type).Use()); //Spawn the enemy in
        SpawnEnemy(enemy); //Spawn the enemy in
    }

    private void SpawnEnemy (EnemyGroupInfo enemy) {
        enemy.spawn(); //Spawns the enemy at the position
    }
}

[Serializable] public class EnemyGroupInfo {
    [SerializeField] private int amount = 1;
    [SerializeField] private EnemyType type;
    [SerializeField] private EnemyLevel level;
    /*[SerializeField] private float initialHealth = 1;
    [SerializeField] private float rewardAmount = 1;*/
    [SerializeField] private float spawnDelay;

    private bool spawned = false;

    public void spawn () {
        GameObject prefab;
        WaveManager.Instance.EnemyPrefabs.TryGetValue(type, out prefab); //Get the prefab of the enemy

        Enemy enemy;

        for (int i = 0; i < amount; i++) { //Iterate through each enemy
            SpawnLocation randomLocation = WaveManager.Instance.getRandomSpawnPos(Type); //Get a spawn location
            //Get the enemy from the pool
            enemy = SimplePool.Spawn(prefab, randomLocation.Position, Quaternion.identity).GetComponent<Enemy>();

            //Set enemy properties
            enemy.weaponPrefab = WaveManager.Instance.getEnemyWeapon(type, level);

            int lvl = (int)level;

            float multiplier = 1 + (lvl/10); //Get the multiplier

            Color clr = Color.red;
            if ((int)level <= WaveManager.Instance.EnemyLevelColors.Length) {
                clr = WaveManager.Instance.EnemyLevelColors[lvl];
            }

            enemy.SetColor(clr);

            //Apply the multiplier to the enemy's stats
            enemy.Speed = multiplier;
            enemy.TurnSpeed = multiplier;

            /*enemy.initialHealth = initialHealth;
            enemy.Reward = rewardAmount;*/

            int[] healthPerLevel = {1, 3, 5, 10, 20 };
            int[] rewardPerLevel = { 1, 3, 5, 10, 25 };

            enemy.initialHealth = healthPerLevel[lvl];
            enemy.Reward = rewardPerLevel[lvl];

            //Spawn the enemy
            enemy.Spawn(randomLocation.Position, Quaternion.identity);

            randomLocation.Use(); //Use the random location
        }
        spawned = true;
    }

    public bool isSpawned () {
        return spawned;
    }

    public float SpawnDelay {
        get {
            return spawnDelay;
        }
    }

    public EnemyType Type { //Returns the type of enemy that it is
        get {
            return type;
        }
    }

    public int Ammount {
        get {
            return amount;
        }
    }
}

public class WaveManager : MonoBehaviour {
    

    private static WaveManager instance;

    public static WaveManager Instance {
        get {
            return instance;
        }
    }

    [SerializeField] private Color[] enemyLevelColors;

    public Color[] EnemyLevelColors {
        get {
            return enemyLevelColors;
        }
    }

    public List<EnemyWave> enemyWaves; //Inputted by user

    //Initialization
    private List<SpawnLocation> spawnLocations = new List<SpawnLocation>(); 
    private Dictionary<EnemyType, List<SpawnLocation>> sortedSpawnLocations = new Dictionary<EnemyType, List<SpawnLocation>>();

    private Dictionary<EnemyType, GameObject> enemyPrefabs = new Dictionary<EnemyType, GameObject>();
    
    private Dictionary<EnemyType, Dictionary<EnemyLevel, GameObject>> enemyWeaponLevels = new Dictionary<EnemyType, Dictionary<EnemyLevel, GameObject>>();

    public Dictionary<EnemyType, GameObject> EnemyPrefabs {
        get {
            return enemyPrefabs;
        }
    }

    public GameObject getEnemyWeapon (EnemyType type, EnemyLevel level) {
        Dictionary<EnemyLevel, GameObject> weapons = new Dictionary<EnemyLevel, GameObject>();
        GameObject weaponPrefab = null;

        enemyWeaponLevels.TryGetValue(type, out weapons);

        weapons.TryGetValue(level, out weaponPrefab);

        return weaponPrefab;
    }

    private const float WAVETIMEOUT = 240; //4 minute timeout

    void Awake () {
        instance = this;

        enemyPrefabs.Clear();

        //Get the prefabs from the asset folder
        enemyPrefabs.Add(EnemyType.TURRET, Resources.Load("Prefabs/Enemy/Enemy_Turret", typeof(GameObject)) as GameObject);
        enemyPrefabs.Add(EnemyType.FIGHTER, Resources.Load("Prefabs/Enemy/Enemy_Fighter", typeof(GameObject)) as GameObject);

        foreach (EnemyType type in Enum.GetValues(typeof (EnemyType))) { //Iterate through each enemy type
            //Initialize a temporary dictionary to keep track of all the weapon prefabs of that enemy type
            Dictionary<EnemyLevel, GameObject> tempDict = new Dictionary<EnemyLevel, GameObject>(); 

            foreach (EnemyLevel level in Enum.GetValues(typeof(EnemyLevel))) { //Iterate through each enemy level
                //Attempt to load the enemy weapon prefab
                string path = "Prefabs/Enemy/Weapons/" + type.ToString().ToLower() + "_" + level.ToString().ToLower();

                GameObject tempPrefab = Resources.Load(path, typeof(GameObject)) as GameObject;
                if (tempPrefab != null) //If it exists
                    tempDict.Add(level, tempPrefab); //Add it to the temporary list
            }

            enemyWeaponLevels.Add(type, tempDict); //Add the list of weapons to the dictionary
        }
    }

    void OnEnable () {
        Enemy.OnSpawn += Enemy_OnSpawn;
        Enemy.OnDeath += Enemy_OnDeath;
    }

    void OnDisable () {
        Enemy.OnSpawn -= Enemy_OnSpawn;
        Enemy.OnDeath -= Enemy_OnDeath;
    }

    void Start () {
        spawnLocations.AddRange(GameObject.FindObjectsOfType<SpawnLocation>()); //Add the spawn locations to the list

        
        foreach (EnemyType type in Enum.GetValues(typeof(EnemyType))) {
            List<SpawnLocation> temp = new List<SpawnLocation>();
            foreach (SpawnLocation sl in spawnLocations) {
                if (sl.EnemyType == type)
                    temp.Add(sl);
            }
            sortedSpawnLocations.Add(type, temp);
        }

        //Preload 10 of each enemy types
        GameObject prefab;
        foreach (EnemyType value in Enum.GetValues(typeof(EnemyType))) {
            enemyPrefabs.TryGetValue(value, out prefab);
             
            SimplePool.Preload(prefab, 10);
        }

        GameManager.WavesAmount = enemyWaves.Count; //Set the amount of enemy waves
        StartCoroutine(startWaves()); //Begin the enemy waves
    }

    public SpawnLocation getRandomSpawnPos (EnemyType type) {
        List<SpawnLocation> locations = getLocationsOfType(type);

        int count = 0;

        while (count < 10) {
            int rand = UnityEngine.Random.Range(0, locations.Count);

            SpawnLocation sl = locations[rand];

            if (sl.Availabe) 
                return sl;

            count++;
        }

        print("Went past 10 iterations");

        return getSpawnPos(type); //If it's unable to find one within 10 iterations than try just going through them all
    }

    public SpawnLocation getSpawnPos (EnemyType type) {
        List<SpawnLocation> locations = getLocationsOfType(type);

        foreach (SpawnLocation loc in locations) {
            if (loc.Availabe) {
                return loc;
            }
        }

        return null;
    }

    private List<SpawnLocation> getLocationsOfType (EnemyType type) {
        List<SpawnLocation> locations = new List<SpawnLocation>();

        sortedSpawnLocations.TryGetValue(type, out locations);

        return locations;
    }

    private int currentIndex = 0;
    private int enemiesAlive = 0;

    IEnumerator startWaves () {
        yield return new WaitUntil(() => ShopManager.Instance.ShopOpening == false);
        yield return new WaitForSeconds(2f);
        yield return new WaitUntil(() => ShopManager.Instance.ShopOpening == false);

        for (currentIndex = 0; currentIndex < enemyWaves.Count; currentIndex++) {

            NotificationManager.Instance.ShowNotification(NotificationType.LARGE, "Wave " + (currentIndex + 1), 2f, Color.red);
            yield return new WaitUntil(() => NotificationManager.Instance.isRunning(NotificationType.LARGE) == false);
            yield return new WaitUntil(() => PauseManager.Instance.Paused == false);
            NotificationManager.Instance.CancelNotification(NotificationType.LARGE);

            enemyWaves[currentIndex].SpawnWave(); //Start the spawning sequence
            enemiesAlive = enemyWaves[currentIndex].EnemyAmount; //Set the amount of enemies

            float waveStartTimeStamp = Accessories.time;

            while (enemiesAlive > 0 && (Accessories.time - waveStartTimeStamp) <= WAVETIMEOUT) {
                yield return null;
            }

            if (enemiesAlive > 0) {
                Enemy[] aliveEnemies = GameObject.FindObjectsOfType<Enemy>();

                foreach (Enemy e in aliveEnemies) //Kill the remaining enemies alive
                    if (e.Alive) {
                        e.kill();
                    }
            }

            foreach (SpawnLocation loc in spawnLocations)  //Set all spawn locations to availabe
                loc.Availabe = true;

            if (Accessories.time - waveStartTimeStamp > WAVETIMEOUT)
                NotificationManager.Instance.ShowNotification(NotificationType.LARGE, "Wave Complete (timout)", 3f, Color.cyan);
            else
                NotificationManager.Instance.ShowNotification(NotificationType.LARGE, "Wave Complete!", 3f, Color.cyan);

            GameManager.completedWave(); //Add one to the completed wave
            yield return new WaitUntil(() => NotificationManager.Instance.isRunning(NotificationType.LARGE) == false);
            yield return new WaitUntil(() => PauseManager.Instance.Paused == false);
        }

        NotificationManager.Instance.ShowNotification(NotificationType.LARGE, "You Won!", 2f, Color.green);
        GameOverManager.Instance.GameOver = true;
        yield return new WaitUntil(() => NotificationManager.Instance.isRunning(NotificationType.LARGE) == false);
        yield return new WaitUntil(() => PauseManager.Instance.Paused == false);

        GameOverManager.Instance.OpenMenu(0.5f);

        yield return null;
    }
    

    private void Enemy_OnSpawn() {
        //enemiesAlive++;
    }

    private void Enemy_OnDeath() {
        enemiesAlive--;
    }
}
