using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public class Game : MonoBehaviour {

    // prefabs
    public GameObject innerHitBoxPrefab;
    public GameObject outerHitBoxPrefab;
    public GameObject notePrefab;
    public GameObject ringPrefab;

    private const float hitboxFactor = 4.5f;
    private const float spawnFactor = 17.5f;
    private const int maxNumberOfNotes = 5;
    private const float waitTime = 1.5f; // cooldown for notes

    private int totalNotes;
    private float speed = -3f;


    private int gameTime = 60;
    private bool isPaused = false;
    private bool gameOver = false;


    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    public GameObject pauseButton;

    private Player newP1;
    private Player newP2;


    private class Player{

        public enum PlayerNumber
        {
            One,
            Two
        }

        public Vector2[] spawnPositions;
        public HitBox innerLeftHitBox;
        public HitBox innerCenterHitBox;
        public HitBox innerRightHitBox;
        public HitBox outerLeftHitBox;
        public HitBox outerCenterHitBox;
        public HitBox outerRightHitBox;

        private PlayerNumber playerNum;
        private GameObject ringPrefab;

        // create all the hit boxes and spawn positions for the player
        public Player(PlayerNumber playerNum, GameObject innerHitBoxPrefab, GameObject outerHitBoxPrefab, GameObject ringPrefab){

            this.playerNum = playerNum;
            this.ringPrefab = ringPrefab;

            Vector2 hitBoxSpawnCoordinate;
            spawnPositions = new Vector2[3];

            if (this.playerNum == PlayerNumber.One){
                hitBoxSpawnCoordinate = new Vector2(-7.75f, -9.5f);
                spawnPositions[0] = new Vector2(hitBoxSpawnCoordinate.x - hitboxFactor, hitBoxSpawnCoordinate.y + spawnFactor);
                spawnPositions[1] = new Vector2(hitBoxSpawnCoordinate.x, hitBoxSpawnCoordinate.y + spawnFactor);
                spawnPositions[2] = new Vector2(hitBoxSpawnCoordinate.x + hitboxFactor, hitBoxSpawnCoordinate.y + spawnFactor);
            }
            else
            {
                hitBoxSpawnCoordinate = new Vector2(7.75f, -9.5f);
                spawnPositions[0] = new Vector2(hitBoxSpawnCoordinate.x - hitboxFactor, hitBoxSpawnCoordinate.y + spawnFactor);
                spawnPositions[1] = new Vector2(hitBoxSpawnCoordinate.x, hitBoxSpawnCoordinate.y + spawnFactor);
                spawnPositions[2] = new Vector2(hitBoxSpawnCoordinate.x + hitboxFactor, hitBoxSpawnCoordinate.y + spawnFactor);
            }

            innerLeftHitBox = createGameComponent(hitBoxSpawnCoordinate.x - hitboxFactor, hitBoxSpawnCoordinate.y, innerHitBoxPrefab).GetComponent<HitBox>();
            innerCenterHitBox = createGameComponent(hitBoxSpawnCoordinate.x, hitBoxSpawnCoordinate.y, innerHitBoxPrefab).GetComponent<HitBox>();
            innerRightHitBox = createGameComponent(hitBoxSpawnCoordinate.x + hitboxFactor, hitBoxSpawnCoordinate.y, innerHitBoxPrefab).GetComponent<HitBox>();

            outerLeftHitBox = createGameComponent(hitBoxSpawnCoordinate.x - hitboxFactor, hitBoxSpawnCoordinate.y, outerHitBoxPrefab).GetComponent<HitBox>();
            outerCenterHitBox = createGameComponent(hitBoxSpawnCoordinate.x, hitBoxSpawnCoordinate.y, outerHitBoxPrefab).GetComponent<HitBox>();
            outerRightHitBox = createGameComponent(hitBoxSpawnCoordinate.x + hitboxFactor, hitBoxSpawnCoordinate.y, outerHitBoxPrefab).GetComponent<HitBox>();
        }

        public void detectChange(){

            if (this.playerNum == PlayerNumber.One) {

                if (Input.GetKeyDown(KeyCode.A))
                {
                    controlGameComponent(innerLeftHitBox, outerLeftHitBox);
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    controlGameComponent(innerCenterHitBox, outerCenterHitBox);
                }
                else if (Input.GetKeyUp(KeyCode.D))
                {
                    controlGameComponent(innerRightHitBox, outerRightHitBox);
                }
            }
            else if (this.playerNum == PlayerNumber.Two){

                if (Input.GetKeyDown(KeyCode.J))
                {
                    controlGameComponent(innerLeftHitBox, outerLeftHitBox);
                }
                else if (Input.GetKeyDown(KeyCode.K))
                {
                    controlGameComponent(innerCenterHitBox, outerCenterHitBox);
                }
                else if (Input.GetKeyUp(KeyCode.L))
                {
                    controlGameComponent(innerRightHitBox, outerRightHitBox);
                }
            }
        }

        private void controlGameComponent(HitBox innerHitBox, HitBox outerHitBox)
        {
            Vector2 position = innerHitBox.GetComponent<Transform>().position;

            if (innerHitBox.getNoteIsTouching() && !outerHitBox.getNoteIsTouching())
            {
                Destroy(innerHitBox.getNoteObject());
                Ring ringObject = ((GameObject)Instantiate(ringPrefab, position, ringPrefab.transform.rotation)).GetComponent<Ring>();
                ringObject.createGreenRing();
            }
            else if (innerHitBox.getNoteIsTouching() && outerHitBox.getNoteIsTouching())
            {
                Destroy(innerHitBox.getNoteObject());
                Ring ringObject = ((GameObject)Instantiate(ringPrefab, position, ringPrefab.transform.rotation)).GetComponent<Ring>();
                ringObject.createYellowRing();
            }
            else
            {
                Ring ringObject = ((GameObject)Instantiate(ringPrefab, position, ringPrefab.transform.rotation)).GetComponent<Ring>();
                ringObject.createRedRing();
            }
        }
    }

    private static GameObject createGameComponent(float x, float y, GameObject prefab)
    {
        Vector2 newPosition = new Vector2(x, y);
        GameObject newPrefab = (GameObject)Instantiate(prefab, newPosition, prefab.transform.rotation);
        return newPrefab;
    }


    void Start () {

        newP1 = new Player(Player.PlayerNumber.One, innerHitBoxPrefab, outerHitBoxPrefab, ringPrefab);
        newP2 = new Player(Player.PlayerNumber.Two, innerHitBoxPrefab, outerHitBoxPrefab, ringPrefab);

        StartCoroutine(countDown());
        StartCoroutine(noteTimer());

        isPaused = false;
        gameOver = false;
    }

    public IEnumerator noteTimer()
    {
        if (isPaused == false && totalNotes < maxNumberOfNotes)
        {
            createNote();
        }

        yield return new WaitForSeconds(waitTime);

        StartCoroutine(noteTimer());
    }

    public IEnumerator countDown()
    {
        yield return new WaitForSeconds(1f);

        if (gameTime > 0 && isPaused == false)
        {
            gameTime = gameTime - 1;
        }
        else if(gameTime == 0)
        {
            endGame();
        }

        StartCoroutine(countDown());
    }


    void Update()
    {
        newP1.detectChange();
        newP2.detectChange();    
    }

    private void createNote()
    {
        int hitBoxCoordinatePosition = Random.Range(0, newP1.spawnPositions.Length - 2);
        Vector2 tempSpawnCoordinate = newP1.spawnPositions[hitBoxCoordinatePosition];

        Vector2 newNotePosition = new Vector2(tempSpawnCoordinate.x, tempSpawnCoordinate.y); // based on a random spawn point
        GameObject newNoteObject = (GameObject)Instantiate(notePrefab, newNotePosition, notePrefab.transform.rotation);

        newNoteObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, speed);
        totalNotes = totalNotes + 1;
    }


    public void pauseGame()
    {
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

    public void resumeGame(){

        isPaused = false;
        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);

        foreach (GameObject note in GameObject.FindObjectsOfType(typeof(GameObject)))
        {
            if (note.name == Note.noteName)
            {
                note.GetComponent<Note>().GetComponent<Rigidbody2D>().velocity = new Vector2(0, speed);
            }
        }
    }

    public void endGame()
    {
        isPaused = true;
        gameOver = true;
    }
}
