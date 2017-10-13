using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cardScript : MonoBehaviour {
   

    public bool _isRevealed = false;
    public bool _isOnPile = true;
    public int value;
    public Sprite image;

    private bool moved = false;
    private Vector2 currentPosition;
    private stateBeforeMove state;
    private int[] matchedCard = {0,0};
    public bool _isHidden = false;
    public bool _isKing = false;
    public bool _isLittlePill = false;
    class stateBeforeMove{
        private Vector2 position;
        private Transform parent;
        private int order;
        private bool isHidden;
        public void setState(Vector2 _pos, Transform _par, int _ord, bool _isHidden)
        {
            position = _pos;
            parent = _par;
            order = _ord;
            isHidden = _isHidden;
        }

        public Vector2 getPosition()
        {
            return position;
        }

        public Transform getParent()
        {
            return parent;
        }

        public int getOrder()
        {
            return order;
        }

        public bool getIsHidden()
        {
            return isHidden;
        }
    }

	// Use this for initialization
	void Start () {
        state = new stateBeforeMove();
        if (_isKing)
        {
            matchedCard[0] = -2;
            matchedCard[1] = -2;
        }
        else if (value < 12)
        {
            matchedCard[0] = value + 27;
            matchedCard[1] = value + 40;
        }else if(value <25)
        {
            matchedCard[0] = value + 14;
            matchedCard[1] = value + 27;
        }else if(value<38)
        {
            matchedCard[0] = value - 25;
            matchedCard[1] = value - 12;
        }else
        {
            matchedCard[0] = value - 38;
            matchedCard[1] = value - 25;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (moved)
        {

            currentPosition = Input.mousePosition;
            GetComponent<RectTransform>().position = currentPosition;
        }
       
        
	}
    public void _move()
    {
       
        gameManager.instance._currentHoldedCard = value;
        moved = true;
        state.setState(transform.position, transform.parent,transform.GetSiblingIndex(),_isHidden);
        if (transform.parent.GetComponent<cardScript>() && !transform.parent.GetComponent<cardScript>()._isLittlePill)
            transform.parent.GetComponent<cardScript>()._isHidden = false;
        transform.SetParent(gameManager.instance._canvas);
        transform.SetAsLastSibling();
        GetComponent<UnityEngine.UI.Image>().raycastTarget = false;
       Transform[] allChildren = GetComponentsInChildren<Transform>();
       foreach (Transform child in allChildren)
       {
            child.GetComponent<cardScript>()._isHidden = true;
            child.GetComponent<UnityEngine.UI.Image>().raycastTarget = false;
        }

       
        gameManager.instance._additionalMessageToShow = value +" " +matchedCard[0].ToString() + " " + matchedCard[1].ToString();
    }

    public void _pointerAboveCard()
    {
        if (_isRevealed && !_isHidden)
            gameManager.instance.cardUnderPointer = this.gameObject;
    }
 
    
    public void _clicked()
    {
        
        gameManager.instance._additionalMessageToShow = "";
        
            if (moved)
            {
                
                Transform[] allChildren = GetComponentsInChildren<Transform>();
                foreach (Transform child in allChildren)
                {
                    child.GetComponent<cardScript>()._isHidden = false;
                    child.GetComponent<UnityEngine.UI.Image>().raycastTarget = true;
                }

                if (gameManager.instance.cardUnderPointer && !gameManager.instance.cardUnderPointer.GetComponent<cardScript>()._isOnPile && (gameManager.instance.cardUnderPointer.GetComponent<cardScript>().value == matchedCard[0] || gameManager.instance.cardUnderPointer.GetComponent<cardScript>().value == matchedCard[1]))
                {
                    transform.position = gameManager.instance.cardUnderPointer.transform.position + new Vector3(0, -25, 0);
                    transform.SetParent(gameManager.instance.cardUnderPointer.transform);
                    gameManager.instance.cardUnderPointer.GetComponent<cardScript>()._isHidden = true;
                    hideWholePill();
                    checkIfItWasLastCardOnLittlePill();
                    _createParticle(transform.position);
                }
                else
                {
                    transform.SetParent(state.getParent());
                    transform.position = state.getPosition();
                    transform.SetSiblingIndex(state.getOrder());
                    _isHidden = state.getIsHidden();
                }
              
                gameManager.instance._currentHoldedCard = -1;
                moved = false;
                return;
            }else
       if (Input.GetMouseButtonUp(0))
       {
            if (_isOnPile)
            {
                _revealCard();
            }
            else if (transform.childCount == 0)
            {
                _showFront();
            }
        }
       else if (Input.GetMouseButtonUp(1) && transform.childCount == 0)
        {
            Vector2 newPos = gameManager.instance._checkIfIsCurrentExpectedCard(value);
            if (newPos!=Vector2.zero)
            {
                StartCoroutine(sendCardToTheEnd(newPos));
                
            }
        }

    }

    private void hideWholePill()
    {
        Transform p = transform;
       
            do
            {
                p = p.transform.parent;
            } while (!p.name.Contains("pos_"));
           Transform[] childrens = p.gameObject.GetComponentsInChildren<Transform>();

            for (int i = 0; i < childrens.Length-1; i++)
                {

                    if (childrens[i].GetComponent<cardScript>()._isRevealed)
                    {
                       childrens[i].GetComponent<cardScript>()._isHidden = true;
                    }
                }
    }
    private void checkIfItWasLastCardOnLittlePill()
    {
        if (state.getParent().name.Contains("pos_"))
        {
            state.getParent().GetComponent<cardScript>()._isHidden = false;
            state.getParent().GetComponent<UnityEngine.UI.Image>().raycastTarget = true;
        }
    }
    public void _revealCard()
    {
        _isRevealed = true;
        _isOnPile = false;
        _isHidden = true;
        GetComponent<RectTransform>().anchoredPosition = new Vector2(119.1f, 0);
        transform.SetAsLastSibling();
        gameManager.instance._takeCardFromPill();
        GetComponent<UnityEngine.UI.Image>().sprite = image;
    }

    public void _hideCard()
    {
        _isRevealed = false;
        _isOnPile = true;
        _isHidden = false;
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        transform.SetAsFirstSibling();
        GetComponent<UnityEngine.UI.Image>().sprite = gameManager.instance._reverse;
    }

    public void _showFront()
    {
        GetComponent<UnityEngine.UI.Image>().sprite = image;
        _isRevealed = true;
    }

    float duration = 0.2f;
    float magnitude = 1f;

    IEnumerator Shake()
    {

        float elapsed = 0.0f;

        Vector3 originalCamPos = GetComponent<RectTransform>().anchoredPosition;

        while (elapsed < duration)
        {

            elapsed += Time.deltaTime;

            float percentComplete = elapsed / duration;
            float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);
            float x = Random.value * 2.0f - 1.0f;
            float y = Random.value * 2.0f - 1.0f;
            x *= magnitude * damper;
            y *= magnitude * damper;

            GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, originalCamPos.z);

            yield return null;
        }

        GetComponent<RectTransform>().anchoredPosition = originalCamPos;
    }

    public void _createParticle(Vector3 p)
    {
        return;
        Instantiate(gameManager.instance._particle, Camera.main.ScreenToWorldPoint(p), Quaternion.Euler(Vector3.zero));
    }


    IEnumerator sendCardToTheEnd(Vector3 endPos)
    {
  
        state.setState(transform.position, transform.parent, transform.GetSiblingIndex(), _isHidden);
        if (transform.parent.GetComponent<cardScript>() && transform.parent.GetComponent<cardScript>()._isRevealed)
            transform.parent.GetComponent<cardScript>()._isHidden = false;
        transform.SetParent(gameManager.instance._canvas);
        transform.SetAsLastSibling();
        Vector3 currentPos = transform.GetComponent<RectTransform>().anchoredPosition;
       
            do
            {
                currentPos = Vector3.MoveTowards(currentPos, endPos, 1000f * Time.fixedDeltaTime);
                transform.GetComponent<RectTransform>().anchoredPosition = currentPos;
                yield return new WaitForSeconds(0.01f);
            } while (currentPos != endPos);
       

        transform.GetComponent<cardScript>().enabled = false;
        checkIfItWasLastCardOnLittlePill();
        
    }
}
