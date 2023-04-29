using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private int currPlayerScrap;
    private int collectedScrap;

    private bool isDocking = false;

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
    }
    public void ChangeScrapVal(int scrapChange)
    {
        int tempTarget = scrapChange;
        if(scrapChange == -1 || scrapChange == 0)
        {
            tempTarget = 0;
        }

        currPlayerScrap = tempTarget;
    }

    private void Update()
    {
        print(currPlayerScrap);
    }

    public void DockingScrap()
    {
        if(!isDocking)
        {
            //lerp or change overtime the players scrap
            StartCoroutine(DockScraps());
            isDocking = true;
        }

    }
    public void StopDock()
    {
        StopCoroutine(DockScraps());
    }

    IEnumerator DockScraps()
    {
        while(currPlayerScrap > 0)
        {
            yield return new WaitForSeconds(1f);
            currPlayerScrap--;
            collectedScrap++;
        }
        isDocking = false;
    }
}
