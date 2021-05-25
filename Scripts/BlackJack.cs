/** Jamie Henry
 *  BlackJack
 *  Script for BlackJack game
 */

// import packages
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackJack : MonoBehaviour
{
    // declare variables
    public Sprite[] cardFaces;
    public GameObject playerArea;
    public GameObject dealerArea;
    public GameObject cardPrefab;
    public GameObject endPlayerArea;
    public GameObject endDealerArea;
    public Button hitButton;
    public Button stayButton;
    public Button toStartButton;
    public Button restartGameButton;
    public Text winLoseText;
    public Text playerScoreText;
    public Text dealerScoreText;
    private static string[] suits = {"Heart", "Club", "Spade", "Diamond"};
    private static string[] cardValues = {"Ace", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Jack", "Queen", "King"};
    private int playerScore = 0;
    private int dealerScore = 0;
    private bool gameOver = false;
    private bool playerWon = false;
    private bool dealerWon = false;
    private List<string> deck = GenerateDeck();
    private List<string> playerHand = new List<string>();
    private List<string> dealerHand = new List<string>();
    private List<GameObject> inPlayerArea = new List<GameObject>();
    private List<GameObject> inDealerArea = new List<GameObject>();
    System.Random random = new System.Random();

    //  Start is called before the first frame update
    void Start()
    {
        // hide after game objects that are visible
        toStartButton.gameObject.SetActive(false);
        restartGameButton.gameObject.SetActive(false);
        ShuffleDeck();
        DealBeginning();
        hitButton.onClick.AddListener(HitClicked);
        stayButton.onClick.AddListener(StayClicked);
    }

    //  Update is called every frame
    void Update()
    {
        // check if game over is true to see who won
        if (gameOver)
        {
            // check if player or dealer busted
            if (playerScore > 21 || dealerScore > 21)
            {
                // check who busted and assign who wins
                if (playerScore > 21)
                {
                    playerWon = false;
                    dealerWon = true;
                }
                else
                {
                    playerWon = true;
                    dealerWon = false;
                }

            }
            // check if player or dealer got blackjack
            else if (playerScore == 21 || dealerScore == 21)
            {
                // check who got blackjack and assign who wins
                if (playerScore == 21 && dealerScore != 21)
                {
                    playerWon = true;
                    dealerWon = false;
                }
                else if (dealerScore == 21 && playerScore != 21)
                {
                    playerWon = false;
                    dealerWon = true;
                }
                // finally both are blackjack so winner has the fewer cards in their hand (player win ties)
                else
                {
                    if (playerHand.Count < dealerHand.Count)
                    {
                        playerWon = true;
                        dealerWon = false;
                    }
                    else if (playerHand.Count == dealerHand.Count)
                    {
                        playerWon = true;
                        dealerWon = false;
                    }
                    else
                    {
                        playerWon = false;
                        dealerWon = true;
                    }
                }
            }
            // finally check who has the higher score
            else
            {
                if (playerScore > dealerScore)
                {
                    playerWon = true;
                    dealerWon = false;
                }
                // check if the player and the dealer have the same score
                else if(playerScore == dealerScore)
                {
                    // check who has the least cards (player wins ties)
                    if (playerHand.Count < dealerHand.Count)
                    {
                        playerWon = true;
                        dealerWon = false;
                    }else if (playerHand.Count == dealerHand.Count)
                    {
                        playerWon = true;
                        dealerWon = false;
                    }
                    else
                    {
                        playerWon = false;
                        dealerWon = true;
                    }
                }
                else
                {
                    playerWon = false;
                    dealerWon = true;
                }
            }
        }

        // check if anyone won and then go to win/lose screen
        if (playerWon || dealerWon)
        {
            // check who won
            if (playerWon)
            {
                // set win/lose text to player wins
                winLoseText.text = "You Win!";
            }
            else
            {
                // set win/lose text to player loses
                winLoseText.text = "You Lose!";
            }

            // populate score texts for dealer and player
            playerScoreText.text = "Score: " + playerScore;
            dealerScoreText.text = "Score : " + dealerScore;

            // set hit/stay buttons to not active and restart/main menu buttons to active
            hitButton.gameObject.SetActive(false);
            stayButton.gameObject.SetActive(false);
            restartGameButton.gameObject.SetActive(true);
            toStartButton.gameObject.SetActive(true);

            // change parent of dealer/player hands to end areas
            for (int i = 0; i < inPlayerArea.Count; i++)
            {
                // transfer parents and make sure card is face up
                inPlayerArea[i].transform.SetParent(endPlayerArea.transform, false);
                inPlayerArea[i].GetComponent<UpdateSprite>().isFaceUp = true;
            }

            for (int i = 0; i < inDealerArea.Count; i++)
            {
                // transfer parents and make sure card is face up
                inDealerArea[i].transform.SetParent(endDealerArea.transform, false);
                inDealerArea[i].GetComponent<UpdateSprite>().isFaceUp = true;
            }

            // set game over back to false so update function stops
            gameOver = false;
        }
    }

    // function for when the hit button is pressed
    // deal a player a card
    private void HitClicked()
    {
        DealCard(true, true);

        // check if player bust or got 21
        if (playerScore >= 21)
        {
            gameOver = true;
        }
    }

    // function for when the stay button is pressed
    // dealer hits until they are above or at 17 and then update score and check who won
    private void StayClicked()
    {
        // update dealer score then dealer hits until he is 17 or greater
        UpdateScore(false);
        while (dealerScore < 17)
        {
            DealCard(false, true);

            // stop if dealer bust
            if (dealerScore >= 21)
            {
                break;
            }
        }

        // set game over to true
        gameOver = true;
    }

    // function to update score of dealer or player
    // dealer or player score updated is based on boolean
    private void UpdateScore(bool playerScoreUpdated)
    {
        // temp variable to keep track of aces present
        int numAces = 0;

        // check if player or dealer score is being updated
        if (playerScoreUpdated)
        {
            // initialize score to 0 to begin
            playerScore = 0;

            // loop through player hand to update score
            for (int i = 0; i < playerHand.Count; i++)
            {
                // Add up all scores
                playerScore += GetScore(playerHand[i]);

                // check if ace is present in the hand
                if (playerHand[i].Substring(0, playerHand[i].IndexOf("_")).Equals("Ace"))
                {
                    numAces++;
                }
            }

            // if the player busted, check for every ace the player has and decrease the score appropriately
            while (playerScore > 21)
            {
                // if ace is present in hand, update the score from 11 to 1
                // and update the number of aces remaining in the player's hand
                if (numAces > 0)
                {
                    playerScore -= 10;
                    numAces--;
                }
                else
                {
                    break;
                }
            }
        }
        else
        {
            // initialize score to 0 to begin
            dealerScore = 0;

            // loop through dealer hand to update score
            for (int i = 0; i < dealerHand.Count; i++)
            {
                // Add up all scores
                dealerScore += GetScore(dealerHand[i]);

                // check if ace is present in the hand
                if (dealerHand[i].Substring(0, dealerHand[i].IndexOf("_")).Equals("Ace"))
                {
                    numAces++;
                }
            }

            // if ace was present in hand, check for every ace the dealer has and decrease the score appropriately
            while (dealerScore > 21)
            {
                // if ace is present in hand, update the score from 11 to 1
                // and update the number of aces remaining in the dealer's hand
                if (numAces > 0)
                {
                    dealerScore -= 10;
                    numAces--;
                }
                else
                {
                    break;
                }
            }
        }
    }

    // function that deals the beginning part of the blackjack game
    private void DealBeginning()
    {
        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                case 0: // first card dealt to player face up
                    DealCard(true, true);
                    break;
                case 1: // second card dealt to dealer face down
                    DealCard(false, false);
                    break;
                case 2: // third card dealt to player face up
                    DealCard(true, true);
                    break;
                case 3: // fourth card dealt to dealer face up
                    DealCard(false, true);
                    break;
            }
        }

        // check if blackjack was dealt first round
        if (playerScore == 21 || dealerScore == 21)
        {
            gameOver = true;
        }
    }

    // function that returns a score based on string of cardvalue passed in
    private int GetScore(string card)
    {
        // temp variables for testing and returning
        int score = 0;
        // card format is "Value_Suit"
        string cardValue = card.Substring(0, card.IndexOf("_"));

        // check what the card value is and assign the correct score
        switch (cardValue)
        {
            case "Two":
                score = 2;
                break;
            case "Three":
                score = 3;
                break;
            case "Four":
                score = 4;
                break;
            case "Five":
                score = 5;
                break;
            case "Six":
                score = 6;
                break;
            case "Seven":
                score = 7;
                break;
            case "Eight":
                score = 8;
                break;
            case "Nine":
                score = 9;
                break;
            case "Ten":
            case "Jack":
            case "Queen":
            case "King":
                score = 10;
                break;
            case "Ace":
                score = 11;
                break;
        } 

        // return final score
        return score;
    }

    // function that deals a card to the player or dealer with 2 inputs
    // bool input: player dealt or not, bool input: face up or not
    // update score based on if player or dealer was dealt
    private void DealCard(bool playerDealt, bool faceUp)
    {
        // create new game object and remove from the deck to simulate the card leaving the deck
        GameObject cardDealt = Instantiate(cardPrefab, new Vector2(0, 0), Quaternion.identity);
        cardDealt.name = deck[deck.Count - 1];
        deck.RemoveAt(deck.Count - 1);

        // check if player(true) was dealt or dealer(false) was
        if (playerDealt)
        {
            playerHand.Add(cardDealt.name);
            inPlayerArea.Add(cardDealt);
            cardDealt.transform.SetParent(playerArea.transform, false);
        }
        else
        {
            dealerHand.Add(cardDealt.name);
            inDealerArea.Add(cardDealt);
            cardDealt.transform.SetParent(dealerArea.transform, false);
        }

        // card is face up/face down based on passed in boolean
        cardDealt.GetComponent<UpdateSprite>().isFaceUp = faceUp;

        // update score after dealing card
        UpdateScore(playerDealt);
    }

    // function that creates a deck of all suits and all card values
    public static List<string> GenerateDeck()
    {
        // variable to populate and return
        List<string> builtDeck = new List<string>();

        // two for loops to populate deck
        for (int i = 0; i < suits.Length; i++)
        {
            for (int j = 0; j < cardValues.Length; j++)
            {
                builtDeck.Add(cardValues[j] + "_" + suits[i]);
            }
        }

        // return deck variable
        return builtDeck;
    }

    // function that shuffles the deck
    private void ShuffleDeck()
    {
        // temporary variables for storing initial values
        string temp;
        int randomNum;

        // loop through and randomize the deck
        for (int i = 0; i < deck.Count; i++)
        {
            randomNum = random.Next(deck.Count - 1);
            temp = deck[i];
            deck[i] = deck[randomNum];
            deck[randomNum] = temp;
        }
    }
}
