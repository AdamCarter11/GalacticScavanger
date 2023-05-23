using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // scrap
    private int currPlayerScrap;
    private int collectedScrap;
    private int goalScrap;
    // gas
    private int currPlayerGas;
    private int collectedGas;
    private int goalGas;

    private bool isDocking = false;
    bool canDock = true;

    //Timer variables
    [Header("TEXT ui elements")]
    [SerializeField] TMP_Text timerText;
    [SerializeField] TMP_Text scrapText, depositedText;
    [SerializeField] TMP_Text gasText, depositedGasText;
    [SerializeField] TMP_Text scrapGoalText, gasGoalText;
    [SerializeField] TMP_Text levelUpText;
    [Header("Game variables")]
    [SerializeField] float StartingTime;
    [SerializeField] int totalGoalAmount = 20;

    float timeLeft;
    bool timerOn = true;

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
        goalScrap = Random.Range(0, 20);
        goalGas = totalGoalAmount - goalScrap;
        print("Scrap Goal: " + goalScrap + " metal goal: " + goalGas);
        scrapGoalText.text = "Scrap goal: " + goalScrap;
        gasGoalText.text = "Gas goal: " + goalGas;

        AssignTextVals();
    }
    public void ChangeScrapVal(int scrapChange)
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

    }

    private void Update()
    {
        //print(currPlayerScrap);
        TimerFunc();
    }

    private void TimerFunc()
    {
        if (timerOn)
        {
            if(timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                UpdateTimer(timeLeft);
            }
            else
            {
                timerOn = false;
                // gameover
                Cursor.visible = true;
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

    public void DockingScrap(GameObject scrapObj, int maxScrapCap, string whatType)
    {
        if (!isDocking)
        {
            canDock = true;
            //lerp or change overtime the players scrap
            //StartCoroutine(DockScraps());
            StartCoroutine(DockScraps(scrapObj, maxScrapCap, whatType));
            isDocking = true;
        }

    }
    public void StopDock()
    {
        canDock = false;
        //StopCoroutine(DockScraps());
        StopCoroutine(DockScraps(null, 0, ""));
    }

    IEnumerator DockScraps(GameObject scrapObj, int maxScrapCap, string whatType)
    {
        while (scrapObj != null && scrapObj.GetComponent<CollectableBehavior>().scrapCap > 0 && canDock)
        {
            yield return new WaitForSeconds(1f);
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
        if(currPlayerScrap >= goalScrap && currPlayerGas >= goalGas)
        {
            // Level up condition
            StartCoroutine(levelUpTextDisplay());
        }
    }
    IEnumerator levelUpTextDisplay()
    {
        levelUpText.enabled = true;
        yield return new WaitForSeconds(2f);
        levelUpText.enabled = false;
    }
    void AssignTextVals()
    {
        scrapText.text = "Scrap: " + currPlayerScrap;
        depositedText.text = "Deposited: " + collectedScrap;
        gasText.text = "Gas: " + currPlayerGas;
        depositedGasText.text = "Deposited: " + collectedGas;
    }
}
