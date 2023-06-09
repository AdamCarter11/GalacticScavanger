using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Transactions;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject player;

    // scrap
    private int currPlayerScrap;
    private int collectedScrap;
    private int goalScrap;
    // gas
    private int currPlayerGas;
    private int collectedGas;
    private int goalGas;
    // enemies
    /*[HideInInspector]*/ public int currEnemiesDestroyed;

    private bool isDocking = false;
    bool canDock = true;

    //Timer variables
    [Header("TEXT ui elements")]
    [SerializeField] TMP_Text timerText;
    [SerializeField] TMP_Text scrapText, depositedText;
    [SerializeField] TMP_Text gasText, depositedGasText;
    [SerializeField] TMP_Text destroyedEnemiesText;
    //[SerializeField] TMP_Text scrapGoalText, gasGoalText;
    [SerializeField] TMP_Text levelUpText;
    [Header("Game variables")]
    [SerializeField] float StartingTime;
    [SerializeField] int totalGoalAmount = 20;
    [SerializeField] int goalIncreaseAmount = 5;
    [SerializeField] float scrapCollectionTime = .5f;
    [SerializeField] int goalEnemies = 10;
    [SerializeField] int goalEnemiesIncreaseAmount = 5;
    [SerializeField] int missionsNeededToWin = 4;
    [SerializeField] EnemyManager enemyManager;
    Vector3 timerTextStartingSize;
    int missionsCompleted = 0;
    GameObject playerRef;

    float timeLeft;
    bool timerOn = true;
    int variableDiffIncrease = 0;

    private void Start()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        timeLeft = StartingTime;
        goalScrap = Random.Range(0, totalGoalAmount);
        goalGas = totalGoalAmount - goalScrap;
        print("Scrap Goal: " + goalScrap + " metal goal: " + goalGas);
        //scrapGoalText.text = "Scrap goal: " + goalScrap;
        //gasGoalText.text = "Gas goal: " + goalGas;
        destroyedEnemiesText.text = "0/" + goalEnemies;
        timerTextStartingSize = timerText.gameObject.transform.localScale;

        AssignTextVals();
    }
    public void DockingFunc(int scrapChange)
    {
            int tempTarget = scrapChange;
            if (scrapChange == -1 || scrapChange == 0)
            {
                collectedScrap += currPlayerScrap;
                tempTarget = 0;
            }
            currPlayerScrap = tempTarget;
            tempTarget = scrapChange;
            if (scrapChange == -1 || scrapChange == 0)
            {
                collectedGas += currPlayerGas;
                tempTarget = 0;
            }
            currPlayerGas = tempTarget;

        AssignTextVals();

        // Level up condition
        if (collectedScrap >= goalScrap && collectedGas >= goalGas && currEnemiesDestroyed >= goalEnemies)
        {
            print("Finished mission!");
            missionsCompleted++;
            // reset players scrap
            currPlayerScrap = 0;
            collectedScrap = 0;
            currPlayerGas = 0;
            collectedGas = 0;

            // reset players enemies destroyed
            currEnemiesDestroyed = 0;
            
            enemyManager.RespawnAllEnemies();

            // generate a new goal
            if (variableDiffIncrease % 2 == 0)
            {
                totalGoalAmount += goalIncreaseAmount;
                goalEnemies += goalEnemiesIncreaseAmount;
            }
            else
            {
                totalGoalAmount += Mathf.RoundToInt(goalIncreaseAmount / 2);
                goalEnemies += Mathf.RoundToInt(goalEnemiesIncreaseAmount / 2);
            }
            UpdateEnemiesKilled();
            variableDiffIncrease++;
            timeLeft = StartingTime;
            goalScrap = Random.Range(0, totalGoalAmount);
            goalGas = totalGoalAmount - goalScrap;
            print("Scrap Goal: " + goalScrap + " metal goal: " + goalGas);
            //scrapGoalText.text = "Scrap goal: " + goalScrap;
            //gasGoalText.text = "Gas goal: " + goalGas;

            // UPGRADE PLAYER
            player = GameObject.FindGameObjectWithTag("LevelUpManager");
            player.GetComponent<UpgradeSystem>().Upgrade();

            // display level up text
            StartCoroutine(levelUpTextDisplay());
        }

    }

    private void Update()
    {
        if(playerRef == null)
        {
            playerRef = GameObject.FindGameObjectWithTag("Ship");
        }
        else
        {
            TimerFunc();
        }
        //print(currPlayerScrap);
        
    }

    private void TimerFunc()
    {
        if (timerOn)
        {
            if(timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                if(timeLeft <= StartingTime / 2)
                {
                    timerText.gameObject.GetComponent<Animator>().enabled = true;
                    timerText.color = Color.red;
                }
                else
                {
                    timerText.gameObject.GetComponent<Animator>().enabled = false;
                    timerText.gameObject.transform.localScale = timerTextStartingSize;
                    timerText.color = Color.white; 
                }
                UpdateTimer(timeLeft);
            }
            else
            {
                timerOn = false;
                // gameover
                Cursor.visible = true;
                PlayerPrefs.SetString("CauseOfDeath", "Time");
                SceneManager.LoadScene("GameOver");
            }
        }
    }
    private void UpdateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        timerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }

    public void CollectingScrapFunc(GameObject scrapObj, int maxScrapCap, string whatType)
    {
        if (!isDocking)
        {
            canDock = true;
            //lerp or change overtime the players scrap
            //StartCoroutine(DockScraps());
            StartCoroutine(CollectingScrapCo(scrapObj, maxScrapCap, whatType));
            isDocking = true;
        }

    }
    public void StopDock()
    {
        canDock = false;
        //StopCoroutine(DockScraps());
        StopCoroutine(CollectingScrapCo(null, 0, ""));
    }

    IEnumerator CollectingScrapCo(GameObject scrapObj, int maxScrapCap, string whatType)
    {
        while (scrapObj != null && scrapObj.GetComponent<CollectableBehavior>().scrapCap > 0 && canDock)
        {
            yield return new WaitForSeconds(scrapCollectionTime);
            if(whatType == "Scrap")
            {
                currPlayerScrap++;
                scrapText.text = "Scrap: " + currPlayerScrap;
                print("collecting Scrap");
            }
            else if(whatType == "Gas")
            {
                currPlayerGas++;
                gasText.text = "Gas: " + currPlayerGas;
                print("collecting Gas");
            }
            else
            {
                print("Incorrect function parameter for dock function");
            }
            if (scrapObj != null)
                scrapObj.GetComponent<CollectableBehavior>().scrapCap--;
        }
        if(scrapObj != null)
        {
            if (scrapObj.GetComponent<CollectableBehavior>().scrapCap <= 0)
            {
                Destroy(scrapObj);
            }
        }
        isDocking = false;

        
    }
    IEnumerator levelUpTextDisplay()
    {
        // check for win condition
        if (missionsCompleted >= missionsNeededToWin)
        {
            levelUpText.text = "You Win!";
            
        }
        levelUpText.enabled = true;
        yield return new WaitForSeconds(3f);
        levelUpText.enabled = false;
        if (missionsCompleted >= missionsNeededToWin)
        {
            PlayerPrefs.SetString("CauseOfDeath", "Victory");
            SceneManager.LoadScene("GameOver");
        }
    }
    void AssignTextVals()
    {
        scrapText.text = "Scrap: " + currPlayerScrap;
        depositedText.text = "Deposited: " + collectedScrap + "/" + goalScrap;
        gasText.text = "Gas: " + currPlayerGas;
        depositedGasText.text = "Deposited: " + collectedGas + "/" + goalGas;
    }
    public void ChangeTIme(float changeTimeByVal)
    {
        timeLeft += changeTimeByVal;
    }

    public void UpdateEnemiesKilled()
    {
        destroyedEnemiesText.text = currEnemiesDestroyed + "/" + goalEnemies;
    }
}
