using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Game : MonoBehaviour {

    // prefabs
    public GameObject innerHitBoxPrefab;
    public GameObject outerHitBoxPrefab;
    public GameObject notePrefab;
    public GameObject ringPrefab;
    public GameObject deletePrefab;

    public static bool isPaused = false;
    public static bool gameOver = false;
    private int gameTime = 60;

    public GameObject pauseButton;
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    public GameObject timeMenu;
    public GameObject player1ScoreMenu;
    public GameObject player2ScoreMenu;


    public static Player newP1;
    public static Player newP2;

    public Text scoreTextP1;
    public Text comboTextP1;
    public Text scoreTextP2;
    public Text comboTextP2;
    public Text timeText;
    public Text winnerText;


    public static GameObject createGameComponent(float x, float y, GameObject prefab)
    {
        Vector2 newPosition = new Vector2(x, y);
        GameObject newPrefab = (GameObject)Instantiate(prefab, newPosition, prefab.transform.rotation);
        return newPrefab;
    }


    private void createNote(Player player)
    {
        int hitBoxCoordinatePosition = Random.Range(0, player.spawnPositions.Length);
        Vector2 tempSpawnCoordinate = player.spawnPositions[hitBoxCoordinatePosition];
        Vector2 newNotePosition = new Vector2(tempSpawnCoordinate.x, tempSpawnCoordinate.y); // based on a random spawn point
        GameObject newNoteObject = (GameObject)Instantiate(notePrefab, newNotePosition, notePrefab.transform.rotation);

        newNoteObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, player.speed);
    }


    public IEnumerator noteTimer(Player player)
    {
        if (Game.isPaused == false)
        {
            createNote(player);
        }

        yield return new WaitForSeconds(player.spawnTime);

        StartCoroutine(noteTimer(player));
    }

    void Start () {


        newP1 = new Player(Player.PlayerNumber.One, notePrefab, innerHitBoxPrefab, outerHitBoxPrefab, ringPrefab);
        newP2 = new Player(Player.PlayerNumber.Two, notePrefab, innerHitBoxPrefab, outerHitBoxPrefab, ringPrefab);
        createGameComponent(0, -12.75f, deletePrefab);

        StartCoroutine(noteTimer(newP1));
        StartCoroutine(noteTimer(newP2));
        StartCoroutine(countDown());

        isPaused = false;
        gameOver = false;
    }


    void Update()
    {
        scoreTextP1.text = "Score: " + newP1.score;
        comboTextP1.text = "Combo: " + newP1.combo;
        scoreTextP2.text = "Score: " + newP2.score;
        comboTextP2.text = "Combo: " + newP2.combo;
        timeText.text = "Time Left: " + gameTime;

        if(isPaused == false){
            newP1.detectChange();
            newP2.detectChange();
        }
    }


    private IEnumerator countDown()
    {
        yield return new WaitForSeconds(1f);

        if (gameTime > 0 && isPaused == false)
        {
            gameTime = gameTime - 1;
        }
        else if (gameTime == 0)
        {
            endGame();
        }

        StartCoroutine(countDown());
    }


    public void PauseGame()
    {

        Debug.Log("Paused");
        isPaused = true;
        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);

        foreach (GameObject note in GameObject.FindObjectsOfType(typeof(GameObject)))
        {
            if (note.name == Note.noteName)
            {
                note.GetComponent<Note>().GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            }
        }
    }

    public void ResumeGame(){

        isPaused = false;
        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);

        foreach (GameObject note in GameObject.FindObjectsOfType(typeof(GameObject)))
        {
            if (note.name == Note.noteName)
            {
                Vector2 position = note.GetComponent<Transform>().position;

                if (position.x < 0)
                {
                    note.GetComponent<Note>().GetComponent<Rigidbody2D>().velocity = new Vector2(0, newP1.speed);
                }
                else
                {
                    note.GetComponent<Note>().GetComponent<Rigidbody2D>().velocity = new Vector2(0, newP2.speed);
                }
            }
        }
    }
    public void Restart() {
        Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
    }

    private void endGame()
    { 
        isPaused = true;
        gameOver = true;

        gameOverMenu.SetActive(true);
        pauseButton.SetActive(false);
        pauseMenu.SetActive(false);
        timeMenu.SetActive(false);
        player1ScoreMenu.SetActive(false);
        player2ScoreMenu.SetActive(false);

        string winner = "";
        if(newP1.score > newP2.score){
            winner = "Player 1!";
        }
        else if (newP2.score > newP1.score){
            winner = "Player 2!";
        }

        if(winner != ""){
            winnerText.text = "The Winner is " + winner + "\n\n Player 1 Score: " +
            newP1.score + "\n Player 2 Score: " + newP2.score;
        }
        else {
            winnerText.text = "The Outcome is a Tie! \n\n Player 1 Score: " +
            newP1.score + "\n Player 2 Score: " + newP2.score;
        }
            
        foreach (GameObject obj in GameObject.FindObjectsOfType(typeof(GameObject)))
        {
            if (obj.name == Note.noteName || obj.name == HitBox.innerHitBoxName 
                || obj.name == HitBox.outerHitBoxName || obj.name == HitBox.boundaryName)
            {
                Destroy(obj);
            }
        }
    }
}
