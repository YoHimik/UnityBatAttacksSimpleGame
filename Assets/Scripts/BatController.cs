using System;
using UnityEngine;
using Random = System.Random;

public class BatController : MonoBehaviour {
    private enum BatState {
        Starting,
        Waiting,
        FlyingToPlayer,
        Attacking,
        Returning
    }

    private const float Speed = 3f;
    private const int TargetTime = 3;
    private const int MinHeight = 15;
    private const int MaxHeight = 35;
    private const int MinRange = 20;
    private const int MaxRange = 100;
    private const int MinWaitSeconds = 1;
    private const int MaxWaitSeconds = 10;
    private const string MainCameraTag = "MainCamera";
    private const string AttackAnimName = "attack1";
    private const string AttackAnimTrigName = "BatAttack";
    private const string StayAnimTrigName = "BatStay";
    private const string FlyAnimTrigName = "BatFly";

    [SerializeField] private Animator _animator;
    private BatState _state;
    private Vector3 _endPosition;
    private Vector3 _lastMainCameraPosition;
    private int _waitTime;
    private float _elapsedWaitTime;
    private float _elapsedNewTargetTime;

    void Start() {
        transform.position = MakeNewRandomPoint();
        _state = BatState.Starting;
    }

    void Update() {
        switch (_state) {
            case BatState.Starting:
                StartNewFly();
                break;
            case BatState.Waiting:
                Wait();
                break;
            case BatState.FlyingToPlayer:
                UpdatePlayerPosition();
                break;
            case BatState.Attacking:
                WaitAttackEnd();
                break;
        }
    }

    void FixedUpdate() {
        switch (_state) {
            case BatState.FlyingToPlayer:
                FlyToPlayer();
                break;
            case BatState.Returning:
                FlyToEndPoint();
                break;
        }
    }

    private void StartNewFly() {
        var r = new Random(DateTime.Now.Second);
        _endPosition = MakeNewRandomPoint();
        _waitTime = r.Next(MinWaitSeconds, MaxWaitSeconds + 1);
        _elapsedNewTargetTime = TargetTime;
        _state = BatState.Waiting;
    }

    private void Wait() {
        _elapsedWaitTime += Time.deltaTime;
        _animator.SetTrigger(StayAnimTrigName);
        if (_elapsedWaitTime < _waitTime)
            return;
        _elapsedWaitTime = 0;
        _state = BatState.FlyingToPlayer;
    }

    private void FlyToPlayer() {
        var step = Speed * Time.deltaTime;
        var targetDir = _lastMainCameraPosition - transform.position;
        var newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDir);
        transform.position = Vector3.MoveTowards(transform.position, _lastMainCameraPosition, step);
        _animator.SetTrigger(FlyAnimTrigName);
        if (transform.position != _lastMainCameraPosition)
            return;
        _state = BatState.Returning;
    }

    void OnTriggerEnter(Component collision)
    {
        if (collision.gameObject.tag != MainCameraTag)
            return;
        _animator.SetTrigger(AttackAnimTrigName);
        _state = BatState.Attacking;
    }

    private void UpdatePlayerPosition() {
        _elapsedNewTargetTime += Time.deltaTime;
        if (_elapsedNewTargetTime < TargetTime) return;
        _lastMainCameraPosition = GameObject.FindGameObjectWithTag(MainCameraTag).transform.position;
        _elapsedNewTargetTime = 0;
    }

    private void WaitAttackEnd() {
        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(AttackAnimName))
            return;
        _state = BatState.Returning;
    }

    private void FlyToEndPoint() {
        var step = Speed * Time.deltaTime;
        var targetDir = _endPosition - transform.position;
        var newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDir);
        transform.position = Vector3.MoveTowards(transform.position, _endPosition, step);
        if (transform.position != _endPosition)
            return;
        _state = BatState.Starting;
    }

    private Vector3 MakeNewRandomPoint() {
        var r = new Random(DateTime.Now.Millisecond);
        var minPoint = (int) Math.Sqrt((MinRange * MinRange - MinHeight * MinHeight) / 2);
        var maxPoint = (int) Math.Sqrt((MaxRange * MaxRange - MaxHeight * MaxHeight) / 2);
        return new Vector3(_lastMainCameraPosition.x + r.Next(minPoint, maxPoint + 1) * (r.Next(0, 2) * 2 - 1),
            _lastMainCameraPosition.y + r.Next(MinHeight, MaxHeight + 1),
            _lastMainCameraPosition.z + r.Next(minPoint, maxPoint + 1) * (r.Next(0, 2) * 2 - 1));
    }
}