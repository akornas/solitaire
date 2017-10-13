using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class birdScript : MonoBehaviour {
    private Vector2 position;
    private Vector2 endPosition;
    private RectTransform rectTransform;
    private int goLeft = 1;
    private bool goingStealCard = false;
    private Vector2 cardXY = new Vector2(-1000, 0);
    private Transform stolenCard;
    private int amplitude = 0;
	// Use this for initialization
   void Awake(){
       rectTransform = GetComponent<RectTransform>();
       endPosition = Random.Range(0, 2) == 0 ? new Vector2(624, 484) : new Vector2(-624, 484);
       amplitude = Random.Range(50, 100);
   }
	void Start () {
        position = rectTransform.anchoredPosition;
     
	}
	
	// Update is called once per frame
	void Update () {
        if (Mathf.Abs(cardXY.x-position.x)>4f && stolenCard==null)
        {
            position.x += goLeft* 100 * Time.fixedDeltaTime;
            position.y = -200 - Mathf.Sin(position.x % 360 * Mathf.PI / 180) * amplitude;
            rectTransform.anchoredPosition = position;
        }
        else if(!goingStealCard)
        {
            
            position = Vector2.MoveTowards(position, cardXY, 500f * Time.fixedDeltaTime);
            rectTransform.anchoredPosition = position;
            if (position == cardXY)
            {
                stolenCard = transform.parent;
               
                transform.SetParent(gameManager.instance._canvas);
                position = rectTransform.anchoredPosition;
                goingStealCard = true;
            }
        }
        else
        {
            stolenCard.SetParent(this.transform);
            stolenCard.SetAsFirstSibling();
            position = Vector2.MoveTowards(position, endPosition, 500f * Time.fixedDeltaTime);
            rectTransform.anchoredPosition = position;
        }
	}

   

    public void _setCardXY(Vector2 xy)
    {
        if (rectTransform.anchoredPosition.x > 0)
            goLeft = -1;
        cardXY = xy;
       
    }
}
