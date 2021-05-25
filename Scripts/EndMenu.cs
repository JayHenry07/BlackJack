/** Jamie Henry
 *  BlackJack
 *  Script for end menu buttons
 */

// import packages
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndMenu : MonoBehaviour
{
    // function for game to return to start scene
    public void ReturnToStart()
    {
        SceneManager.LoadScene("StartScene");
    }

    // function for game to restart and play another round
    public void RestartGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
