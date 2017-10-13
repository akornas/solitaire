using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class setDefaultsScript : MonoBehaviour {
    public Sprite _blankCard;
    public Sprite _defaultCardReverse;

    string pathToReversesDirectory;

    private int reversesInDirectory = 0;
	// Use this for initialization
	void Start () {
        pathToReversesDirectory = Application.persistentDataPath + "/reverses";

        if (!Directory.Exists(pathToReversesDirectory))
            Directory.CreateDirectory(pathToReversesDirectory);
        if (!File.Exists(pathToReversesDirectory +"/default_1.png"))
            saveImageToFile(_defaultCardReverse, _blankCard);

        countReverses();
        
	}
	
	

    void saveImageToFile(Sprite image,Sprite blankCard)
    {
        Texture2D newReverse = new Texture2D((int)blankCard.rect.width, (int)blankCard.rect.height);
        for (int y = 0; y < (int)blankCard.rect.height; y++)
        {
            for (int x = 0; x < (int)blankCard.rect.width; x++)
            {
              
                if(blankCard.texture.GetPixel(x,y)==Color.white)
                    newReverse.SetPixel(x, y, image.texture.GetPixel(x % (int)image.rect.width, y % (int)image.rect.height));
                else
                    newReverse.SetPixel(x, y, blankCard.texture.GetPixel(x, y));
            }
        }
        File.WriteAllBytes(pathToReversesDirectory+ "/default_1.png", newReverse.EncodeToPNG());
    }


    void countReverses()
    {
        reversesInDirectory = Directory.GetFiles(pathToReversesDirectory, "*", SearchOption.TopDirectoryOnly).Length;
    }

    int currentRevers = 0;
    public Sprite _getNextReverse(int direction)
    {
        currentRevers += direction;
        if (currentRevers >= reversesInDirectory) reversesInDirectory = 0;
        else if (currentRevers < 0) currentRevers = reversesInDirectory - 1;

        byte[] image = File.ReadAllBytes(Directory.GetFiles(pathToReversesDirectory)[currentRevers]);
        Texture2D loadedTexture = new Texture2D(1, 1);
        loadedTexture.LoadImage(image);
        return Sprite.Create(loadedTexture,new Rect(0, 0, 100, 155),Vector2.zero);
    }
}
