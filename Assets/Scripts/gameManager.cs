using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class gameManager : MonoBehaviour {
    public GameObject menu_panel;
    public GameObject game_panel;
    public Sprite _reverse;

    public setDefaultsScript _defaultValues;
    public InputField _inputColor;

    public Sprite[] _cards_sprite;
    public List<int> cards;

    public GameObject _pill;
    public GameObject _cardsOnBoardContainer;
    public Transform[] _littlePillPos;
    public int _cardsOnPill = 5;
    static public gameManager instance;

    public GameObject _cardPrefab;
    public Transform _canvas;
    public int _currentHoldedCard=-1;


    public Text debug;
    public GameObject cardUnderPointer;

    public int[] currentExpectedCards = { 0, 13, 39, 26 };
    public string _additionalMessageToShow="";
    public GameObject _particle;
    public additionalEffects _additionaleffects;
	// Use this for initialization
	void Start () {
        instance = this;
        generateCards();
	}
	
	// Update is called once per frame
	void Update () {
        if(cardUnderPointer)
            debug.text = cardUnderPointer.name.ToString() + " " + _additionalMessageToShow;

        if (Input.GetKeyDown(KeyCode.Space))
        {
           // _additionaleffects._spawnBird(_littlePillPos[Random.Range(0,7)]);
           _additionaleffects._startRunawayCard(_pill.transform.GetChild(_pill.transform.childCount-1).GetComponent<RectTransform>());
        }
	}

    public void _changeRevers(int direction)
    {
        _reverse = _defaultValues._getNextReverse(direction);
    }

    public void _changeBackgroundColor()
    {
        int r, g, b = 0;
        bool[] isHex = new bool[3];
        string[] values = new string[3];

        if (_inputColor.text.Length < 6)
            _inputColor.text += "000000";

        values[0] = _inputColor.text.Substring(0, 2);
        values[1] = _inputColor.text.Substring(2, 2);
        values[2] = _inputColor.text.Substring(4, 2);

        isHex[0] = int.TryParse(values[0], System.Globalization.NumberStyles.HexNumber, null, out r);
        isHex[1] = int.TryParse(values[1], System.Globalization.NumberStyles.HexNumber, null, out g);
        isHex[2] = int.TryParse(values[2], System.Globalization.NumberStyles.HexNumber, null, out b);

        if (!isHex[0] || !isHex[1] || !isHex[2])
            return;
        Camera.main.backgroundColor = new Color(r,g,b);
    }

    public void _startGame()
    {
        menu_panel.SetActive(false);
        game_panel.SetActive(true);
    }

    private void generateCards()
    {
        GameObject createdCard;
        cards = new List<int>();

        for (int i = 0; i < 52; i++)
        {
            cards.Add(i);
        }
        cards = cards.OrderBy(x => Random.value).ToList();
        for (int i = 0; i < cards.Count; i++)
        {
            createdCard = (GameObject)Instantiate(_cardPrefab);
            createdCard.GetComponent<cardScript>().image = _cards_sprite[cards[i]];
            if ((cards[i] + 1) % 13 == 0)
                createdCard.GetComponent<cardScript>()._isKing = true;
            
                createdCard.GetComponent<cardScript>().value=cards[i];
            createdCard.name = "card_"+cards[i];

            if (i < 24)
            {
                createdCard.transform.SetParent(_pill.transform);
                createdCard.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }
            else
            {
                createdCard.transform.SetParent(_cardsOnBoardContainer.transform);
            }
           
        }
        StartCoroutine(setCardsOnBoard());
    }



    public void _pillClicked(){
        
        foreach(Transform card in _pill.transform){
            card.GetComponent<cardScript>()._hideCard();
            Debug.Log("DAS");
        }
    }

    public void _takeCardFromPill()
    {
        _cardsOnPill--;
    }

   IEnumerator setCardsOnBoard()
    {
       
        GameObject currentCard;
        GameObject prevCard=null;
        for (int collumns = 0; collumns < 7; collumns++)
        {
            prevCard = _littlePillPos[collumns].gameObject;
            for (int rows = 0; rows < 1 + collumns; rows++)
            {
                currentCard = _cardsOnBoardContainer.transform.GetChild(0).gameObject;
                if (rows == collumns)
                {
                    currentCard.GetComponent<cardScript>()._showFront();
                    currentCard.GetComponent<RectTransform>().SetParent(prevCard.transform);
                 
                }
                else
                {
                    currentCard.GetComponent<RectTransform>().SetParent(prevCard.transform);

                }
                    
                    currentCard.GetComponent<cardScript>()._isOnPile = false;
                    currentCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -10);
                    _littlePillPos[collumns].SetAsLastSibling();
                    prevCard = currentCard;
                    if(currentCard.GetComponent<cardScript>()._isRevealed)
                    currentCard.GetComponent<cardScript>()._createParticle(currentCard.transform.position);
                    yield return new WaitForSeconds(0.01f);
                    //yield return new WaitUntil(()=>Input.GetKeyDown(KeyCode.W));
            }
        }

    }

    public Vector2 _checkIfIsCurrentExpectedCard(int value)
    {
        if(value==currentExpectedCards[0])
        {
            currentExpectedCards[0]++;
            gameManager.instance._additionaleffects._fireUp(0,currentExpectedCards[0]%13);
            return new Vector2(13, 197);
        }else if(value==currentExpectedCards[1])
        {
            
            currentExpectedCards[1]++;
            gameManager.instance._additionaleffects._fireUp(1, currentExpectedCards[1] % 13);
            return new Vector2(132, 197);
        }else if(value==currentExpectedCards[2])
        {
            
            currentExpectedCards[2]++;
            gameManager.instance._additionaleffects._fireUp(2, currentExpectedCards[2] % 13);
            return new Vector2(255, 197);
        }else if(value==currentExpectedCards[3]){
            
            currentExpectedCards[3]++;
            gameManager.instance._additionaleffects._fireUp(3, currentExpectedCards[3] % 13);
            return new Vector2(376, 197);
        }
        return Vector2.zero;
    }

    public InputField _debugCardInput;
    public void _debugGiveMeCard()
    {
        GameObject a = GameObject.Find("card_" + _debugCardInput.text);
        a.transform.position = Vector3.zero;

    }

   
  
}
