using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    [SerializeField] Sprite[] healthSprites;

    private Image image;

    private int spriteIndex = 10;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSpriteIndex(int spriteIndex)
    {
        this.spriteIndex = spriteIndex;
        image.sprite = healthSprites[spriteIndex];
    }

}
