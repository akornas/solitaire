using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class additionalEffects : MonoBehaviour {
    public GameObject _fireParticles;
    public GameObject _bird;
    private GameObject[] fire;
    private Vector3[] fire_position = { new Vector3(0.21f, 3.47f, 0), new Vector3(1.69f, 3.47f, 0), new Vector3(3.42f, 3.47f, 0), new Vector3(4.94f, 3.47f, 0) };
	// Use this for initialization

    private bool runAwayCard;
    
    void Start () {
        fire = new GameObject[4];
	}
	
	// Update is called once per frame
	void Update () {
     
	}

    ParticleSystem.EmissionModule emissionModule;
    public void _fireUp(int whichFire, int intensity)
    {
        if (fire[whichFire]==null)
        {
           fire[whichFire]=(GameObject)  Instantiate(_fireParticles, fire_position[whichFire], transform.rotation);
        }
        if (intensity != 0)
        {
            emissionModule = fire[whichFire].GetComponent<ParticleSystem>().emission;
            emissionModule.rateOverTime = intensity;
        }
    }

    public void _spawnBird(Transform littlePill)
    {
        Transform card = littlePill;
        if(card.childCount>0)
        do
        {
            card = card.GetChild(0);
        } while (card.childCount > 0);
        if (card.GetComponent<cardScript>())
        {
            GameObject b =
            Instantiate(_bird, gameManager.instance._canvas);

            Vector2 startPos = Random.Range(0, 2) == 0 ? new Vector2(-654, -226) : new Vector2(654, -226);
            b.GetComponent<RectTransform>().anchoredPosition = startPos;



            if (card.GetComponent<cardScript>()._isRevealed)
            {
                b.transform.SetParent(card);
                b.GetComponent<birdScript>()._setCardXY(card.GetComponent<RectTransform>().anchoredPosition);
            }
        }
    }

    public void _startRunawayCard(RectTransform card)
    {
        runAwayCard = true;
        StartCoroutine(cardRunaway(card));
    }

    IEnumerator cardRunaway(RectTransform card)
    {
        //-91 91 y
        // -65 65 x
        card.transform.parent.SetAsLastSibling();
        Vector2 pos = Input.mousePosition;
        do
        {
            if (Input.GetKey(KeyCode.Q))
            {
                runAwayCard = false;
                card.anchoredPosition = new Vector2(119.1f, 0);
            }
            pos = Input.mousePosition;
            pos.x -= 151;
            pos.y -= 590;
            if (Mathf.Abs(card.anchoredPosition.x - pos.x) < 65 && Mathf.Abs(card.anchoredPosition.y - pos.y) < 151)
            {
                card.anchoredPosition = new Vector2(Random.Range(-100,810), Random.Range(-500,100));
            }
           
            yield return new WaitForEndOfFrame();
        } while (runAwayCard);
       
    }
}
