using System.Collections;
using UnityEngine;
using Assets.PixelFantasy.PixelMonsters.Common.Scripts.ExampleScripts;

public class RexAI : MonoBehaviour
{
    private MonsterController2D _controller;
    private MonsterAnimation _animation;
    private Transform _player;

    private enum AIState { Patrol, Chase, Cooldown }
    private AIState _state = AIState.Patrol;

    [Header("General")]
    public string playerTag = "Player";

    [Header("Patrol")]
    public float patrolSpeedMultiplier = 0.5f;   // slower when patrolling
    public float wallCheckDistance = 0.2f;
    public LayerMask wallLayer;
    public float turnWaitTime = 1f;

    [Header("Chase")]
    public float chaseSpeedMultiplier = 1.0f;    // full speed when chasing
    public float detectionRange = 6f;
    public float stopChaseRange = 8f;
    public float verticalJumpThreshold = 1.5f;

    [Header("Attack")]
    public float attackRange = 1.3f;
    public float attackCooldown = 1.0f;

    private int _patrolDir = 1;
    private bool _waitingToTurn = false;
    private float _attackTimer = 0f;

    private float _baseMaxSpeed;

    private void Awake()
    {
        _controller = GetComponent<MonsterController2D>();
        _animation = GetComponent<MonsterAnimation>();

        _baseMaxSpeed = _controller.MaxSpeed;

        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
        {
            _player = playerObj.transform;
        }
    }

    private void Update()
    {
        // If no player found, just patrol
        if (_player == null)
        {
            PatrolUpdate();
            return;
        }

        // Cooldown timer for attacks
        _attackTimer -= Time.deltaTime;

        float distanceToPlayer = Vector2.Distance(_player.position, transform.position);
        bool playerInChaseRange = distanceToPlayer <= detectionRange;
        bool playerTooFar = distanceToPlayer > stopChaseRange;

        switch (_state)
        {
            case AIState.Patrol:
                PatrolUpdate();

                if (playerInChaseRange)
                {
                    _state = AIState.Chase;
                }
                break;

            case AIState.Chase:
                ChaseUpdate(distanceToPlayer);

                if (playerTooFar)
                {
                    _state = AIState.Patrol;
                }
                break;

            case AIState.Cooldown:
                // still chase but can't attack again yet
                ChaseUpdate(distanceToPlayer);

                if (_attackTimer <= 0f)
                {
                    _state = AIState.Chase;
                }
                break;
        }
    }

    private void PatrolUpdate()
    {
        // set speed for patrol 
        _controller.MaxSpeed = _baseMaxSpeed * patrolSpeedMultiplier;

        if (_waitingToTurn)
        {
            _controller.Input = Vector2.zero;
            return;
        }

        _controller.Input = new Vector2(_patrolDir, 0f);

        if (IsHittingWall(_patrolDir))
        {
            _controller.Input = Vector2.zero;
            StartCoroutine(TurnAroundAfterDelay());
        }
    }

    private bool IsHittingWall(int dir)
    {
        var col = GetComponent<Collider2D>();
        var bounds = col.bounds;


        float x = dir > 0 ? bounds.max.x : bounds.min.x;
        float y = bounds.min.y + 0.1f;  // just above the feet
        Vector2 origin = new Vector2(x, y);

        Vector2 direction = new Vector2(dir, 0f);

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, wallCheckDistance, wallLayer);

        Debug.DrawRay(origin, direction * wallCheckDistance, Color.red);

        return hit.collider != null;
    }


    private IEnumerator TurnAroundAfterDelay()
    {
        if (_waitingToTurn) yield break;

        _waitingToTurn = true;
        _animation.Ready();  // optional pause animation

        yield return new WaitForSeconds(turnWaitTime);

        _patrolDir *= -1;
        _waitingToTurn = false;
    }

    private void ChaseUpdate(float distanceToPlayer)
    {
        // set speed for chase (no compounding)
        _controller.MaxSpeed = _baseMaxSpeed * chaseSpeedMultiplier;

        Vector2 toPlayer = _player.position - transform.position;
        float dirX = Mathf.Sign(toPlayer.x);
        float absX = Mathf.Abs(toPlayer.x);
        float absY = Mathf.Abs(toPlayer.y);

        // move towards player
        _controller.Input = new Vector2(dirX, 0f);

        // jump only while chasing, if player is higher
        if (absY > verticalJumpThreshold && _controller.IsGrounded)
        {
            // only jump if the player is clearly ahead, not already overlapping
            if (absX > attackRange && absX < attackRange * 2.5f)
            {
                _controller.Input = new Vector2(dirX, 1f);
            }
        }

        // attack if close enough and off cooldown
        if (distanceToPlayer <= attackRange && _attackTimer <= 0f)
        {
            _controller.Input = Vector2.zero;
            _animation.Attack();

            _attackTimer = attackCooldown;
            _state = AIState.Cooldown;
        }
    }
}
