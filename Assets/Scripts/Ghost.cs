using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public enum Mode
    {
        Chase,
        Scatter,
        Frightened,
        Eaten
    }
    public Mode currentMode = Mode.Scatter;
    public Mode previousMode;
    public enum GhostType
    {
        Red,
        Pink,
        Orange,
        Blue
    }
    public GhostType ghostType;

    public TurningPoint currentTurningPoint, targetTurningPoint, previousTurningPoint, scatterTurningPoint, startingTurningPoint;
    public Vector2 direction, nextDirection;


    public float currentSpeed = 0f;
    public float previousSpeed = 0f;
    public float frightenedSpeed = 3f;
    public float eatenSpeed = 15f;
    public float normalSpeed = 5f;
    public float speed = 0f;

    public Mapa mapa; //Instance to the gameboard
    private SoundManager soundManager;
    GameObject pacMan; //Instance to PacMan
    //Timers
    public float ghostReleaseTimer;
    public float frightenedModeTimer;
    public float blinkTimer;
    public int scatterModeTimer1 = 7;
    public int chaseModeTimer1 = 20;
    public int scatterModeTimer2 = 7;
    public int chaseModeTimer2 = 20;
    public int scatterModeTimer3 = 5;
    public int chaseModeTimer3 = 20;
    public int scatterModeTimer4 = 5;
    public int chaseModeTimer4 = 20;

    public float ghostReleaseTime;
  
    public int frightenedModeDuration = 10;
    public int startBlinkingAt = 7;

    public bool frightenedModeIsWhite = false;
    public bool isInGhostHouse;


    int modeChangeIteration = 1;
    float modeChangeTimer = 0;

    public RuntimeAnimatorController frightenedGhost;
    public RuntimeAnimatorController whiteGhost;

    public Sprite eyesUp;
    public Sprite eyesDown;
    public Sprite eyesLeft;
    public Sprite eyesRight;
    public Sprite defaultSprite;

    public int lives = 3;
    public bool canMove = true;
    private void Start()
    {
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        pacMan = GameObject.FindGameObjectWithTag("Pacman");
        TurningPoint turningPoint = GetTurningPointAtPosition(transform.position);
        if(turningPoint != null)
        {
            currentTurningPoint = turningPoint;
            
        }
        startingTurningPoint = GetTurningPointAtPosition(transform.position);
        if (isInGhostHouse)
        {
            direction = currentTurningPoint.directionToNeighborTurningPoint[0];
            targetTurningPoint = currentTurningPoint.neighborsTurningPoints[0];
        }
        else
        {
            direction = Vector2.left;
            targetTurningPoint = ChooseNextTurningPoint();
        }
        previousTurningPoint = currentTurningPoint;
        UpdateAnimatorController();
    }
    private void Update()
    {
        if (canMove)
        {
            ModeUpdate();
            ReleaseGhosts();
            CheckIsInGhostHouse();
            UpdateSpeed();
            Move();
        }
    }
    private void Move()
    {
        if(targetTurningPoint != null && targetTurningPoint != currentTurningPoint && !isInGhostHouse)
        {
            if (OverShotTarget())
            {
                currentTurningPoint = targetTurningPoint;
                transform.position = currentTurningPoint.transform.position;

                GameObject otherPortal = GetPortal(currentTurningPoint.transform.position);
                if (otherPortal != null)
                {
                    transform.position = otherPortal.transform.position;
                    currentTurningPoint = otherPortal.GetComponent<TurningPoint>();
                }
                targetTurningPoint = ChooseNextTurningPoint();
                previousTurningPoint = currentTurningPoint;
                currentTurningPoint = null;
                UpdateAnimatorController();
            }
            else
            {
                transform.position += (Vector3) direction * currentSpeed * Time.deltaTime;
            }
        }
    }
    private void CheckIsInGhostHouse()
    {
        TurningPoint tP = null;
        if(currentMode == Mode.Eaten)
        {
            tP = GetTurningPointAtPosition(transform.position);
            if(tP != null)
            {
                if (tP.GetComponent<TurningPoint>().ghostHouse)
                {
                    currentSpeed = normalSpeed;
                    ChangeMode(Mode.Chase);
                    UpdateAnimatorController();
                    currentTurningPoint = GetTurningPointAtPosition(transform.position);
                    direction = Vector2.up;
                    targetTurningPoint = currentTurningPoint.neighborsTurningPoints[0];
                    previousTurningPoint = currentTurningPoint;
                }
            }
        }
    }
    private void UpdateAnimatorController()
    {

        if (currentMode == Mode.Frightened)
        {
            GetComponent<Animator>().runtimeAnimatorController = frightenedGhost;
        }
        else if (currentMode == Mode.Eaten)
        {
            GetComponent<Animator>().runtimeAnimatorController = null;
            if (direction == Vector2.up)
            {
                GetComponent<SpriteRenderer>().sprite = eyesUp;
            }
            else if (direction == Vector2.down)
            {
                GetComponent<SpriteRenderer>().sprite = eyesDown;
            }
            else if (direction == Vector2.left)
            {
                GetComponent<SpriteRenderer>().sprite = eyesLeft;
            }
            else if (direction == Vector2.right)
            {
                GetComponent<SpriteRenderer>().sprite = eyesRight;
            }
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = defaultSprite;
            GetComponent<Animator>().runtimeAnimatorController = null;
        }
    }
    private void ReleaseGhosts()
    {
        ghostReleaseTimer += Time.deltaTime;
        if (ghostReleaseTimer > ghostReleaseTime)
        {
            if (isInGhostHouse)
            {
                ReleaseGhost();
            }
        }
    }
    private void ReleaseGhost()
    {
        currentSpeed = normalSpeed;
        isInGhostHouse = false;
    }
    Vector2 GetPinkGhostTargetTurningPoint()
    {
        return (Vector2)pacMan.transform.position + (4 * pacMan.GetComponent<PacWoman>().currentDirection);
    }
    Vector2 GetBlueGhostTargetTurningPoint()
    {
        Vector2 targetPosition = (Vector2)pacMan.transform.position + (4 * pacMan.GetComponent<PacWoman>().currentDirection);
        Vector2 currentPosition = transform.position ;
        float distance = GetDistance(currentPosition, targetPosition);

        return new Vector2(targetPosition.x + distance * distance, targetPosition.y + distance * distance);
    }
    Vector2 GetOrangeGhostTargetTurningPoint()
    {
        Vector2 targetPosition = Vector2.zero;
        float distance = GetDistance(pacMan.transform.position, transform.position);
        if(distance > 8)
        {
            targetPosition = pacMan.transform.position;
        }
        else
        {
            targetPosition = scatterTurningPoint.transform.position;
        }
        return targetPosition;
    }
    Vector2 GetRedGhostTargetTurningPoint()
    {
        return pacMan.transform.position;
    }
    Vector2 GetGhostTargetPosition()
    {
        Vector2 targetPosition = Vector2.zero; ;
        if(ghostType == GhostType.Red)
        {
            targetPosition = GetRedGhostTargetTurningPoint();
        } else if (ghostType == GhostType.Pink)
        {
            targetPosition =  GetPinkGhostTargetTurningPoint();
        }
        else if (ghostType == GhostType.Blue)
        {
            targetPosition = GetBlueGhostTargetTurningPoint();
        }
        else if (ghostType == GhostType.Orange)
        {
            targetPosition = GetOrangeGhostTargetTurningPoint();
        }
        return targetPosition;

    }
    TurningPoint ChooseNextTurningPoint()
    {
        Vector2 targetPosition = Vector2.zero;
        TurningPoint moveToTurningPoint = null;

        if (currentMode == Mode.Chase)
        {
            targetPosition = GetGhostTargetPosition();
        }
        else if (currentMode == Mode.Scatter) {
            targetPosition = scatterTurningPoint.transform.position;
        }
        else if (currentMode == Mode.Frightened)
        {

        }
        else if (currentMode == Mode.Eaten)
        {
            targetPosition =  new Vector2(13.5f, 16f);
        }

        int totalPossibleTurningPoints = 0;
        TurningPoint[] possibleTurningPoints = new TurningPoint[4];
        Vector2[] possibleTurningPointsDirection = new Vector2[4];

        for (int i = 0; i < currentTurningPoint.neighborsTurningPoints.Length; i++)
        {
            //Ghost don't go in reverse
            if (currentTurningPoint.directionToNeighborTurningPoint[i] != (direction * -1))
            {
                if(currentMode != Mode.Eaten)
                {
                    TurningPoint tP = null;
                    tP = GetTurningPointAtPosition(transform.position);
                    if (tP != null)
                    {
                        if (tP.GetComponent<TurningPoint>().ghostHouseEntrance)
                        {
                            if (currentTurningPoint.directionToNeighborTurningPoint[i] != Vector2.down)
                            {
                                possibleTurningPoints[totalPossibleTurningPoints] = currentTurningPoint.neighborsTurningPoints[i];
                                possibleTurningPointsDirection[totalPossibleTurningPoints] = currentTurningPoint.directionToNeighborTurningPoint[i];
                                totalPossibleTurningPoints++;
                            }
                        }
                        else
                        {
                            possibleTurningPoints[totalPossibleTurningPoints] = currentTurningPoint.neighborsTurningPoints[i];
                            possibleTurningPointsDirection[totalPossibleTurningPoints] = currentTurningPoint.directionToNeighborTurningPoint[i];
                            totalPossibleTurningPoints++;
                        }
                    }
                }
                else
                {
                    possibleTurningPoints[totalPossibleTurningPoints] = currentTurningPoint.neighborsTurningPoints[i];
                    possibleTurningPointsDirection[totalPossibleTurningPoints] = currentTurningPoint.directionToNeighborTurningPoint[i];
                    totalPossibleTurningPoints++;
                }
            }
        }


        if(totalPossibleTurningPoints == 1)
        {
            moveToTurningPoint = possibleTurningPoints[0];
            direction = possibleTurningPointsDirection[0];
        }    
        if(totalPossibleTurningPoints > 1)
        {
            float leastDistance = 100000000000000f;
            for (int i = 0; i < totalPossibleTurningPoints; i++)
            {
                if (possibleTurningPointsDirection[i] != null &&  possibleTurningPointsDirection[i] != Vector2.zero && possibleTurningPoints[i] != null)
                {
                    float distance = GetDistance(possibleTurningPoints[i].transform.position, new Vector2(Mathf.RoundToInt(targetPosition.x), Mathf.RoundToInt(targetPosition.y))); ;
                    if (distance < leastDistance)
                    {
                        leastDistance = distance;
                        moveToTurningPoint = possibleTurningPoints[i];
                        direction = possibleTurningPointsDirection[i];
                    } 
                }
            }
        }
        return moveToTurningPoint;  
    }
    public void HasBeenEaten()
    {
        ChangeMode(Mode.Eaten);
    }
    TurningPoint GetTurningPointAtPosition(Vector2 _position)
    {
        GameObject gameObjectAtPosition = mapa.mapPoints[(int)_position.x, (int)_position.y];
        if (gameObjectAtPosition != null)
        {
            if(mapa.mapPoints[(int)_position.x, (int)_position.y].GetComponent<TurningPoint>() != null)
            {
                return mapa.mapPoints[(int)_position.x, (int)_position.y].GetComponent<TurningPoint>();
            }
        }
        return null;
    }
    private float GetDistance(Vector2 a, Vector2 b)
    {
        return Mathf.Sqrt(((a.x - b.x) * (a.x - b.x)) + ((a.y - b.y) * (a.y - b.y)));
    }
    public void ChangeMode(Mode m)
    {
        if(currentMode != m)
        {
            previousMode = currentMode;
        }
        currentMode = m;
        UpdateAnimatorController();
    }
    void UpdateSpeed()
    {
        if (currentMode == Mode.Eaten)
        {
            previousSpeed = currentSpeed;
            currentSpeed = eatenSpeed;
        }
        if (currentMode == Mode.Frightened)
        {
            previousSpeed = currentSpeed;
            currentSpeed = frightenedSpeed;
        }
        if (currentMode != Mode.Frightened && currentMode != Mode.Eaten)
        {
            previousSpeed = currentSpeed;
            currentSpeed = normalSpeed;
        }
    }
    public void StartFrightenedMode()
    {
        if(currentMode != Mode.Eaten)
        {
            ChangeMode(Mode.Frightened);
            SoundManager.SMinstance.PlayClipOnLoop(soundManager.ghostAS, soundManager.frightened);
            frightenedModeTimer = 0;
        }
    }
    void ModeUpdate()
    {
        if(currentMode != Mode.Frightened)
        {
            modeChangeTimer += Time.deltaTime;
            if(modeChangeIteration == 1)
            {
                if(currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer1)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }

                if(currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer1)
                {
                    modeChangeIteration = 2;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                             
                }
            }
            //2
            else if (modeChangeIteration == 2)
            {
                if (currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer2)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }

                if (currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer2)
                {
                    modeChangeIteration = 3;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                    
                }
            }
            //3
            else if (modeChangeIteration == 3)
            {
                if (currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer3)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }

                if (currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer3)
                {
                    modeChangeIteration = 4;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                    
                }
            }
            //4
            if (modeChangeIteration == 4)
            {
                if (currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer4)
                {
                    ChangeMode(previousMode);
                    modeChangeTimer = 0;
                }
            }


        }
        else if (currentMode == Mode.Frightened)
        {
            frightenedModeTimer += Time.deltaTime;
            if(frightenedModeTimer >= frightenedModeDuration)
            {
                frightenedModeTimer = 0;
                SoundManager.SMinstance.PlayClipOnLoop(soundManager.ghostAS, soundManager.ghostMove);
                ChangeMode(previousMode);
            }

            if (frightenedModeTimer >= startBlinkingAt)
            {
                blinkTimer += Time.deltaTime;
                if(blinkTimer >= 0.1)
                {
                    blinkTimer = 0f;
                    if (frightenedModeIsWhite)
                    {
                        GetComponent<Animator>().runtimeAnimatorController = frightenedGhost;
                        frightenedModeIsWhite = false;
                    } else
                    {
                        GetComponent<Animator>().runtimeAnimatorController = whiteGhost;
                        frightenedModeIsWhite = true;
                    }
                }
            }


        }
    }
    GameObject GetPortal(Vector2 _position)
    {
        TurningPoint currentTurningPoint = GetTurningPointAtPosition(_position);
        if(currentTurningPoint != null)
        {
            if (currentTurningPoint.isPortal)
            {
                return currentTurningPoint.destinationPortal;
            }
        }
        return null;
    }
    float LengthFromNode(Vector2 _targetPosition)
    {
        Vector2 vector = _targetPosition - (Vector2)previousTurningPoint.transform.position;
        return vector.sqrMagnitude;
    }
    bool OverShotTarget()
    {
        float nodeToTarget = LengthFromNode(targetTurningPoint.transform.position);
        float nodeToSelf = LengthFromNode(transform.position);
        return (nodeToSelf > nodeToTarget);
    }

    public void Restart()
    {
        canMove = true;
        ChangeMode(Mode.Scatter);
        GetComponent<SpriteRenderer>().enabled = true;
        transform.position = startingTurningPoint.transform.position;

        ghostReleaseTimer = 0;
        modeChangeIteration = 1;
        modeChangeTimer = 0;
        
        if(gameObject.name != "Blinky - Shadow - Red")
        {
            isInGhostHouse = true;
        }
        currentTurningPoint = startingTurningPoint;
        if (isInGhostHouse)
        {
            direction = currentTurningPoint.directionToNeighborTurningPoint[0];
            targetTurningPoint = currentTurningPoint.neighborsTurningPoints[0];
        }
        else
        {
            direction = Vector2.left;
            targetTurningPoint = ChooseNextTurningPoint();
        }
        previousTurningPoint = currentTurningPoint;
        UpdateAnimatorController();
    }
}