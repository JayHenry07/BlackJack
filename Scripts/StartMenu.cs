/** Jamie Henry
 *  BlackJack
 *  Start menu script
 */

// import packages
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    // function that loads the game scene
    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    // function that quits the game
    public void QuitGame()
    {
        Application.Quit();
    }
}
