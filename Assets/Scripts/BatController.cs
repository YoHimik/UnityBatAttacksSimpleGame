using System;
using UnityEngine;
using Random = System.Random;

public class BatController : MonoBehaviour {
    [SerializeField] private Animator _animator;

    private enum BatState {
        Starting,
        Waiting,
        FlyingToPlayer,
        Attacking,
        Returning
    }

    private const float MovementSpeed = 5f;
    private const float RotationSpeed = 3f;
    private const float TargetTime = 3f;
    private const float SmoothFlightCoef = 0.7f;
    private const float SmoothReturnFlightCoef = 40f;
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

    private bool _returned;
    private BatState _state;
    private Vector3 _endPosition;
    private Vector3 _lastMainCameraPosition;
    private Vector3[] _path;
    private int _pathIndex;
    private int _waitTime;
    private float _elapsedWaitTime;
    private float _elapsedNewTargetTime = TargetTime;

    void Start() {
        transform.position = MakeNewRandomPoint();
        _state = BatState.Starting;
    }

    void Update() {
        switch (_state) {
            case BatState.Starting:
                StartNewFly();
                break;
            case BatState.FlyingToPlayer:
                UpdatePlayerPosition();
                RotateTo(_path[_pathIndex]);
                FlyToPlayer();
                break;
            case BatState.Returning:
                if (!_returned)
                    MakeReturnPath();
                RotateTo(_path[_pathIndex]);
                FlyToEndPoint();
                break;
        }
    }

    void FixedUpdate() {
        switch (_state) {
            case BatState.Waiting:
                Wait();
                break;
            case BatState.Attacking:
                WaitAttackEnd();
                break;
        }
    }

    private void StartNewFly() {
        _returned = false;
        var r = new Random(DateTime.Now.Millisecond);
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
        _elapsedWaitTime = 0f;
        _state = BatState.FlyingToPlayer;
    }

    private void RotateTo(Vector3 to) {
        var targetDir = to - transform.position;
        transform.rotation =
            Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, targetDir, RotationSpeed * Time.deltaTime,
                0.0f));
    }

    private void UpdatePlayerPosition() {
        _elapsedNewTargetTime += Time.deltaTime;
        if (_elapsedNewTargetTime < TargetTime) return;
        _elapsedNewTargetTime = 0f;
        _lastMainCameraPosition = GameObject.FindGameObjectWithTag(MainCameraTag).transform.position;
        var temp = Vector2.Lerp(new Vector2(transform.position.x, transform.position.z),
            new Vector2(_lastMainCameraPosition.x, _lastMainCameraPosition.z), SmoothFlightCoef);
        var v = new Vector3(temp.x, _lastMainCameraPosition.y, temp.y);
        _pathIndex = 0;
        _path = BezierCurve.MakeQuadraticBezierCurve(
            (int) Vector3.Distance(transform.position, _lastMainCameraPosition),
            transform.position, v, _lastMainCameraPosition);
    }

    private void FlyToPlayer() {
        if (!FlyByPath())
            return;
        _state = BatState.Returning;
    }

    void OnTriggerEnter(Component collision)
    {
        if (collision.gameObject.tag != MainCameraTag && _state != BatState.FlyingToPlayer)
            return;
        _animator.SetTrigger(AttackAnimTrigName);
        _state = BatState.Attacking;
    }

    private void WaitAttackEnd()
    {
        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(AttackAnimName))
            return;
        _state = BatState.Returning;
    }

    private void FlyToEndPoint() {
        if (!FlyByPath())
            return;
        _state = BatState.Starting;
    }

    private bool FlyByPath() {
        _animator.SetTrigger(FlyAnimTrigName);
        var step = MovementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _path[_pathIndex], step);
        if (Vector3.Distance(transform.position, _path[_pathIndex]) > 0)
            return false;
        if (_pathIndex == _path.Length - 1) {
            _pathIndex = 0;
            return true;
        }

        _pathIndex++;
        return false;
    }

    private void MakeReturnPath() {
        _pathIndex = 0;
        _path = BezierCurve.MakeQuadraticBezierCurve(
            (int) Vector3.Distance(transform.position, _endPosition),
            transform.position,
            transform.position + transform.rotation * Vector3.forward * SmoothReturnFlightCoef,
            _endPosition);
        _returned = true;
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