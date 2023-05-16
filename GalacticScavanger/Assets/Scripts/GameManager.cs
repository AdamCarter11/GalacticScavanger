using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private int currPlayerScrap;
    private int collectedScrap;

    private bool isDocking = false;
    bool canDock = true;

    //Timer variables
    [SerializeField] TMP_Text timerText;
    [SerializeField] float StartingTime;
    [SerializeField] TMP_Text scrapText, depositedText;
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
    }
    public void ChangeScrapVal(int scrapChange)
    {
        int tempTarget = scrapChange;
        if(scrapChange == -1 || scrapChange == 0)
        {
            collectedScrap += currPlayerScrap;
            tempTarget = 0;
        }

        currPlayerScrap = tempTarget;
        scrapText.text = "Scrap: " + currPlayerScrap;
        depositedText.text = "Deposited: " + collectedScrap;
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
                //SceneManager.LoadScene("GameOver");
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

    public void DockingScrap(GameObject scrapObj, int maxScrapCap)
    {
        if(!isDocking)
        {
            canDock = true;
            //lerp or change overtime the players scrap
            StartCoroutine(DockScraps(scrapObj, maxScrapCap));
            isDocking = true;
        }

    }
    public void StopDock()
    {
        canDock = false;
        StopCoroutine(DockScraps(null, 0));
    }

    IEnumerator DockScraps(GameObject scrapObj, int maxScrapCap)
    {
        while(scrapObj.GetComponent<CollectableBehavior>().scrapCap > 0 && canDock)
        {
            yield return new WaitForSeconds(1f);
            currPlayerScrap++;
            scrapObj.GetComponent<CollectableBehavior>().scrapCap--;
            //collectedScrap++;
            scrapText.text = "Scrap: " + currPlayerScrap;
            //depositedText.text = "Deposited: " + collectedScrap;
            //print(currPlayerScrap);
        }
        if (scrapObj.GetComponent<CollectableBehavior>().scrapCap <= 0)
        {
            Destroy(scrapObj);
        }
        isDocking = false;
    }
}
