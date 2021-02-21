using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PacWoman : MonoBehaviour
{
    public Vector2 direction = Vector2.zero;
    public Vector2 nextDirection;
    public TurningPoint currentTurningPoint;
    public TurningPoint targetTurningPoint;
    public TurningPoint previousTurningPoint;
    public Mapa mapa;
    [SerializeField] Sprite pausedSprite;
    private SoundManager soundManager;
    [SerializeField] float speed;
    public int score = 0;
    public Text scoreText;
    public int lives = 3;
    public Text livesText;

    void Awake()
    {
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        currentTurningPoint = GetTurningPointAtPosition(transform.position);
        previousTurningPoint = currentTurningPoint;
        direction = Vector2.left;
    }

    private void Start()
    {
        //ChangePosition(direction);
    }


    void ChangePosition(Vector2 _direction) //Travelling TurningPoint to TurningPoint
    {
        //direction = _direction;
        
        if(_direction != direction)
        {
            nextDirection = _direction;
        }
        


        /*
        if(currentTurningPoint != null)
        {
            TurningPoint moveToTurningPoint = CanMove(_direction);
            if (moveToTurningPoint != null)
            {
                direction = _direction;
                targetTurningPoint = moveToTurningPoint;
                previousTurningPoint = currentTurningPoint;
                currentTurningPoint = null;
            }else Debug.Log("Got Here2");
        }
        */
    }

    TurningPoint GetTurningPointAtPosition(Vector2 _position)
    {
        if (mapa.mapPoints[(int)_position.x, (int)_position.y].GetComponent<TurningPoint>() != null)
        {
            return mapa.mapPoints[(int)_position.x, (int)_position.y].GetComponent<TurningPoint>();
        }
        return null;
    }

    TurningPoint CanMove(Vector2 _direction)
    {
        TurningPoint moveToTurningPoint = null;
        for (int i = 0; i < currentTurningPoint.neighborsTurningPoints.Length; i++)
        {
            if (currentTurningPoint.directionToNeighborTurningPoint[i] == _direction)
            {
                moveToTurningPoint = currentTurningPoint.neighborsTurningPoints[i];
                break;
            }
        }
        Debug.Log("Can Move to:" + moveToTurningPoint);
        return moveToTurningPoint;
    }

    void MovePacman()
    {
        if(targetTurningPoint != null)
        {
            /*
            if(nextDirection == direction*-1)
            {
                direction *= -1;
                TurningPoint tempTurningPoint = targetTurningPoint;
                targetTurningPoint = previousTurningPoint;
                previousTurningPoint = tempTurningPoint;
            }
            */

            if (ReachedTargetTurningPoint())
            {
                Debug.Log("Arrived");
                currentTurningPoint = targetTurningPoint;
                transform.position = currentTurningPoint.transform.position;

                /*
                GameObject destiny = GetPortal(currentTurningPoint.transform.position);
                if (destiny != null)
                {
                    transform.position = destiny.transform.position;
                    currentTurningPoint = destiny.GetComponent<TurningPoint>();
                }
                */

                TurningPoint moveToTurningPoint = CanMove(nextDirection);
                if (moveToTurningPoint == null)
                {
                    moveToTurningPoint = CanMove(direction);
                    if (moveToTurningPoint == null)
                    {
                        direction = Vector2.zero;
                    }
                    else
                    {
                        targetTurningPoint = moveToTurningPoint;
                        previousTurningPoint = currentTurningPoint;
                        currentTurningPoint = null;
                    }
                }
                else
                {
                    direction = nextDirection;
                    targetTurningPoint = moveToTurningPoint;
                    previousTurningPoint = currentTurningPoint;
                    currentTurningPoint = null;
                }
            }
            else if (direction != Vector2.zero)
            {
                transform.position += (Vector3)direction * speed * Time.deltaTime;
            } else transform.position += (Vector3)nextDirection * speed * Time.deltaTime;
        }
    }
    float DistanceToPreviousTurningPoint(Vector2 _targetPosition)
    {
        Vector2 vector = _targetPosition - (Vector2)previousTurningPoint.transform.position;
        return vector.sqrMagnitude;
    }

    bool ReachedTargetTurningPoint()
    {
        float distanceToTarget = DistanceToPreviousTurningPoint(targetTurningPoint.transform.position);
        float travelledDistance = DistanceToPreviousTurningPoint(transform.position);
        return (travelledDistance > distanceToTarget);
    }
    void CheckInput()
    {
        if (Input.GetKeyDown("left"))
        {
            ChangePosition(Vector2.left);
        }
        else if (Input.GetKeyDown("right"))
        {
            ChangePosition(Vector2.right); 
        }
        else if (Input.GetKeyDown("up"))
        {
            ChangePosition(Vector2.up);
        }
        else if (Input.GetKeyDown("down"))
        {
            ChangePosition(Vector2.down);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        scoreText.text = "Score:" + score.ToString();
        livesText.text = "Lives:" + lives.ToString();
        CheckInput();
        MovePacman();
        UpdateOrientation();
        UpdateEatingAnimation();
    }

    private void UpdateEatingAnimation()
    {
        if(direction == Vector2.zero)
        {
            gameObject.GetComponent<Animator>().enabled = false;
            gameObject.GetComponent<SpriteRenderer>().sprite = pausedSprite;
        }else
        {
            gameObject.GetComponent<Animator>().enabled = true;
        }
    }

    private void PacManDies()
    {
        if (lives > 1)
        {
            Destroy(this);
        }
    }

    /**
    bool CanMoveInDirection(Vector2 _direction)
    {

        Transform point = GameObject.Find("Mapa").GetComponent<Mapa>().mapPoints[(int)transform.position.x, (int)transform.position.y];
        if (point != null)
        {
            GameObject pointGameObject = point.gameObject;
            Vector2[] vectorToNextPoints = pointGameObject.GetComponent<TurningPoint>().vectorToNextPoint;
            for (int i = 0; i < vectorToNextPoints.Length; i++)
            {
                if (vectorToNextPoints[i] == _direction)
                {
                    return true;
                }
            }
        }
        return false;
    }
    **/
    private void  OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Dots"))
        {
            score++;
            Destroy(other.gameObject);
        }
        /**
        if (other.tag == "Ghost")
        {
            String ghostName = other.GetComponent<Collider2D>().gameObject.name;

            // Get the AudioSource
            AudioSource audioSource = soundManager.GetComponent<AudioSource>();

            // If the Ghosts name matches
            if (ghostName == "RedGhost")
            {
                if (redGhostScript.isGhostBlue)
                {

                    // Call for the Ghost to be reset and for its destination 
                    // to now be Ms. Pac-Man
                    redGhostScript.ResetGhostAfterEaten(gameObject);

                    // Play eating ghost sound
                    SoundManager.SMinstance.PlayOneShot(SoundManager.SMinstance.eatingGhost);

                    // Increase the score
                    //IncreaseTextUIScore(400);
                }
                else
                {

                    // Play Ms. Pac-Man dies sound
                    SoundManager.SMinstance.PlayOneShot(SoundManager.SMinstance.pacManDies);

                    // Turn off the dot eating sound
                    audioSource.Stop();

                    // Destroy Ms. Pac-Man
                    Destroy(gameObject);
                }
            }
            else if (ghostName == "PinkGhost")
            {
                if (pinkGhostScript.isGhostBlue)
                {
                    pinkGhostScript.ResetGhostAfterEaten(gameObject);
                    SoundManager.SMinstance.PlayOneShot(SoundManager.SMinstance.eatingGhost);
                    //IncreaseTextUIScore(400);
                }
                else
                {
                    SoundManager.SMinstance.PlayOneShot(SoundManager.SMinstance.pacManDies);
                    audioSource.Stop();
                    Destroy(gameObject);
                }
            }
            else if (ghostName == "BlueGhost")
            {
                if (blueGhostScript.isGhostBlue)
                {
                    blueGhostScript.ResetGhostAfterEaten(gameObject);
                    SoundManager.SMinstance.PlayOneShot(SoundManager.SMinstance.eatingGhost);
                    //IncreaseTextUIScore(400);
                }
                else
                {
                    SoundManager.SMinstance.PlayOneShot(SoundManager.SMinstance.pacManDies);
                    audioSource.Stop();
                    Destroy(gameObject);
                }
            }
            else if (ghostName == "OrangeGhost")
            {
                if (orangeGhostScript.isGhostBlue)
                {
                    orangeGhostScript.ResetGhostAfterEaten(gameObject);
                    SoundManager.SMinstance.PlayOneShot(SoundManager.SMinstance.eatingGhost);
                    //IncreaseTextUIScore(400);
                }
                else
                {
                    SoundManager.SMinstance.PlayOneShot(SoundManager.SMinstance.pacManDies);
                    audioSource.Stop();
                    Destroy(gameObject);
                }
            }

        }
        **/
        if (other.tag == "Pills")
        {
            SoundManager.SMinstance.PlayOneShot(SoundManager.SMinstance.powerUp);
            Destroy(other.gameObject);
            //redGhostScript.TurnGhostBlue();
            //pinkGhostScript.TurnGhostBlue();
            //blueGhostScript.TurnGhostBlue();
            //orangeGhostScript.TurnGhostBlue();

        }
    }
    GameObject GetPortal(Vector2 _position)
    {
        TurningPoint currentTurningPoint = GetTurningPointAtPosition(transform.position);
        if (currentTurningPoint.isPortal)
        {
            return currentTurningPoint.destinationPortal;
        }
        return null;
    }

    void UpdateOrientation()
    {
        if (direction == Vector2.left)
        {
            //left
            transform.localScale = Vector2.one;
            transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
        else if (direction == Vector2.right)
        {
            //right
            transform.localScale = new Vector2(-1, 1);
            transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
        else if (direction == Vector2.up)
        {
            //up
            transform.localScale = new Vector2(1, -1);
            transform.localRotation = Quaternion.Euler(0, 0, 270);
        }
        else if (direction == Vector2.down)
        {
            //down
            transform.localScale = Vector2.one;
            transform.localRotation = Quaternion.Euler(0, 0, 90);
        }
    }

}
