/** Jamie Henry
 *  BlackJack
 *  Script for updating card face sprite
 */

// import packages
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateSprite : MonoBehaviour
{
    // declare variables
    public Sprite cardFace;
    public Sprite cardBack;
    public bool isFaceUp = false;
    private SpriteRenderer spriteRenderer;
    private BlackJack blackjack;

    // Start is called before the first frame update
    void Start()
    {
        List<string> deck = BlackJack.GenerateDeck();
        blackjack = FindObjectOfType<BlackJack>();

        // loop through deck and populate the sprite
        for (int i = 0; i < deck.Count; i++)
        {
            if (this.name == deck[i])
            {
                cardFace = blackjack.cardFaces[i];
                break;
            }
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // populate right sprite for the card's orientation
        if (isFaceUp)
        {
            spriteRenderer.sprite = cardFace;
        }
        else
        {
            spriteRenderer.sprite = cardBack;
        }
        gameObject.GetComponent<Image>().sprite = spriteRenderer.sprite;
    }
}
