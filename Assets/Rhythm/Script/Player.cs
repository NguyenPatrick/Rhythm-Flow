using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    private const float hitboxFactor = 4.5f;
    private const float spawnFactor = 17.5f;
    private const int winScore = 1000;
    private const float speedLimit = 22.5f;
    private const float spawnTimeLimit = 0.35f;



    public float spawnTime = 1f; 
    public float speed = -5f;
    public float combo;
    public float score;


    public enum PlayerNumber
    {
        One,
        Two
    }

    public HitBox innerLeftHitBox;
    public HitBox innerCenterHitBox;
    public HitBox innerRightHitBox;
    public HitBox outerLeftHitBox;
    public HitBox outerCenterHitBox;
    public HitBox outerRightHitBox;

    public Vector2[] spawnPositions;
    private PlayerNumber playerNum;
    private GameObject notePrefab;
    private GameObject ringPrefab;

    // create all the hit boxes and spawn positions for the player
    public Player(PlayerNumber playerNum, GameObject notePrefab, GameObject innerHitBoxPrefab, GameObject outerHitBoxPrefab, GameObject ringPrefab)
    {

        this.playerNum = playerNum;
        this.ringPrefab = ringPrefab;

        Vector2 hitBoxSpawnCoordinate;
        spawnPositions = new Vector2[3];

        if (this.playerNum == PlayerNumber.One)
        {
            hitBoxSpawnCoordinate = new Vector2(-7.75f, -9.75f);
            spawnPositions[0] = new Vector2(hitBoxSpawnCoordinate.x - hitboxFactor, hitBoxSpawnCoordinate.y + spawnFactor);
            spawnPositions[1] = new Vector2(hitBoxSpawnCoordinate.x, hitBoxSpawnCoordinate.y + spawnFactor);
            spawnPositions[2] = new Vector2(hitBoxSpawnCoordinate.x + hitboxFactor, hitBoxSpawnCoordinate.y + spawnFactor);
        }
        else
        {
            hitBoxSpawnCoordinate = new Vector2(7.75f, -9.75f);
            spawnPositions[0] = new Vector2(hitBoxSpawnCoordinate.x - hitboxFactor, hitBoxSpawnCoordinate.y + spawnFactor);
            spawnPositions[1] = new Vector2(hitBoxSpawnCoordinate.x, hitBoxSpawnCoordinate.y + spawnFactor);
            spawnPositions[2] = new Vector2(hitBoxSpawnCoordinate.x + hitboxFactor, hitBoxSpawnCoordinate.y + spawnFactor);
        }

        innerLeftHitBox = Game.createGameComponent(hitBoxSpawnCoordinate.x - hitboxFactor, hitBoxSpawnCoordinate.y, innerHitBoxPrefab).GetComponent<HitBox>();
        innerCenterHitBox = Game.createGameComponent(hitBoxSpawnCoordinate.x, hitBoxSpawnCoordinate.y, innerHitBoxPrefab).GetComponent<HitBox>();
        innerRightHitBox = Game.createGameComponent(hitBoxSpawnCoordinate.x + hitboxFactor, hitBoxSpawnCoordinate.y, innerHitBoxPrefab).GetComponent<HitBox>();

        outerLeftHitBox = Game.createGameComponent(hitBoxSpawnCoordinate.x - hitboxFactor, hitBoxSpawnCoordinate.y, outerHitBoxPrefab).GetComponent<HitBox>();
        outerCenterHitBox = Game.createGameComponent(hitBoxSpawnCoordinate.x, hitBoxSpawnCoordinate.y, outerHitBoxPrefab).GetComponent<HitBox>();
        outerRightHitBox = Game.createGameComponent(hitBoxSpawnCoordinate.x + hitboxFactor, hitBoxSpawnCoordinate.y, outerHitBoxPrefab).GetComponent<HitBox>();
    }

    public void detectChange()
    {

        if (this.playerNum == PlayerNumber.One)
        {

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
        else {

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
            score = score + 500 + (((combo/0.25f)/2f) * (-speed/0.25f)) + ((-speed) / 0.25f);
            combo = combo + 1;

            if (speed > -spawnTimeLimit) {
                speed = speed - 0.25f;
            }

            if(spawnTime > spawnTimeLimit) {
                spawnTime = spawnTime - 0.025f;
            }

        }
        else if (innerHitBox.getNoteIsTouching() && outerHitBox.getNoteIsTouching())
        {
            Destroy(innerHitBox.getNoteObject());
            Ring ringObject = ((GameObject)Instantiate(ringPrefab, position, ringPrefab.transform.rotation)).GetComponent<Ring>();
            ringObject.createYellowRing();
            score = score + 250 + ((5 * (-speed -5)) / 0.25f);
        }
        else
        {
            Ring ringObject = ((GameObject)Instantiate(ringPrefab, position, ringPrefab.transform.rotation)).GetComponent<Ring>();
            ringObject.createRedRing();
            combo = 0;
        }
    }

    private void createNote()
    {
        int hitBoxCoordinatePosition = Random.Range(0, spawnPositions.Length);
        Vector2 tempSpawnCoordinate = spawnPositions[hitBoxCoordinatePosition];

        Vector2 newNotePosition = new Vector2(tempSpawnCoordinate.x, tempSpawnCoordinate.y); // based on a random spawn point
        GameObject newNoteObject = (GameObject)Instantiate(notePrefab, newNotePosition, notePrefab.transform.rotation);
   
        newNoteObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, speed);
    }


    public IEnumerator noteTimer()
    {
        if (Game.isPaused == false)
        {
            createNote();
        }

        yield return new WaitForSeconds(spawnTime);

        StartCoroutine(noteTimer());
    }
}