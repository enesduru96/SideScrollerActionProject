using System.Collections.Generic;
using System.Collections;
using UnityEngine;


using ShadowFort.Utilities;
using System;
using UnityEngine.Splines;

public class CharacterMoverAndRotator : MonoBehaviour
{
    private CharacterActor characterActor;

    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private float _positionDuration;
    private float _positionElapsedTime;
    private bool _isMovingPositionUpdate;


    private Vector3 _startForward;
    private Vector3 _targetForward;
    private float _rotationDuration;
    private float _rotationElapsedTime;
    private bool _isRotating;


    private Vector3 _kinMoveDirection;
    private float _kinMoveSpeed;
    private float _kinMoveDuration;
    private float _kinMoveElapsedTime;
    private bool isKinematicMovingPositionAdd;


    private Vector3 _velocityDirection;
    private float _velocityForce;
    private float _velocityPushDuration;
    private float _velocityPushElapsedTime;
    private bool _isMovingSetCharacterVelocity;


    private CharacterState.InterpolationType _interpolationType;


    private CharacterLocalEventManager playerLocalEventManager;


    private void OnEnable()
    {
        if (characterActor.IsPlayer)
        {
            playerLocalEventManager.CharacterActions.OnDrawSheatheStarted += HandleDrawSheatheButtonPressed;
        }
    }

    private void OnDisable()
    {
        if (characterActor.IsPlayer)
        {
            playerLocalEventManager.CharacterActions.OnDrawSheatheStarted -= HandleDrawSheatheButtonPressed;
        }
    }

    private void Awake()
    {
        characterActor = this.GetComponentInBranch<CharacterActor>();

        if (characterActor.IsPlayer)
        {
            playerLocalEventManager = this.GetComponentInBranch<CharacterActor, CharacterLocalEventManager>();
        }
    }


    private void HandleDrawSheatheButtonPressed()
    {
        //if (!_characterActor.IsGrounded || _characterActor.BusyCombatStateTransitionLocked)
        //    return;

        Vector3 turnDirection = characterActor.IsFacingRight() ? Vector3Utility.AlmostRight : Vector3Utility.AlmostLeft;

        StartRotate(characterActor.Forward, turnDirection, 0.2f, CharacterState.InterpolationType.Linear);
    }



    public void StartVelocityPush(Vector3 direction, float duration, float force)
    {
        _velocityDirection = direction;
        _velocityPushDuration = duration;
        _velocityPushElapsedTime = 0f;
        _velocityForce = force;
        _isMovingSetCharacterVelocity = true;
    }
    public void StartVelocityPushCurve(Vector3 direction, float duration, AnimationCurve curve)
    {
        velocityDirectionNew = direction;
        velocityDuration = duration;
        _velocityPushElapsedTime = 0f;
        velocityCurve = curve;
        _isMovingSetCharacterVelocityWithCurve = true;
    }

    public void StartMoveUpdatePosition(Vector3 start, Vector3 target, float duration, CharacterState.InterpolationType interpolation)
    {
        _startPosition = start;
        _targetPosition = target;
        _positionDuration = duration;
        _positionElapsedTime = 0f;
        _isMovingPositionUpdate = true;
        _interpolationType = interpolation;
    }

    public void StartRotate(Vector3 start, Vector3 target, float duration, CharacterState.InterpolationType interpolation)
    {
        _startForward = start;
        _targetForward = target;
        _rotationDuration = duration;
        _rotationElapsedTime = 0f;
        _isRotating = true;
        _interpolationType = interpolation;
    }

    public void StartKinematicMoveAddPosition(Vector3 direction, float speed, float duration)
    {
        _kinMoveDirection = direction;
        _kinMoveSpeed = speed;
        _kinMoveDuration = duration;
        _kinMoveElapsedTime = 0f;
        isKinematicMovingPositionAdd = true;
    }


    private float ledgeDuration;
    private float ledgeElapsedTime;
    private SplineContainer splineContainer;
    private bool enteredLedgeAbove = false;
    private bool enteredLedgeForward = false;
    private Vector3 ledgeEntryStartPosition;

    public void StartLedgeTopUp(float _ledgeDuration, SplineContainer movementPath)
    {
        ledgeEntryStartPosition = characterActor.Position;
        ledgeDuration = _ledgeDuration;
        splineContainer = movementPath;
        ledgeElapsedTime = 0f;
        enteredLedgeForward = true;
    }
    public void StartLedgeAboveEntry(float _ledgeDuration, SplineContainer movementPath)
    {
        ledgeEntryStartPosition = characterActor.Position;
        ledgeDuration = _ledgeDuration;
        splineContainer = movementPath;
        ledgeElapsedTime = 0f;
        enteredLedgeAbove = true;
    }

    public void Update()
    {

        if (isKinematicMovingPositionAdd)
        {
            _kinMoveElapsedTime += Time.deltaTime;
            Vector3 deltaPosition = _kinMoveDirection * _kinMoveSpeed * Time.deltaTime;
            characterActor.Position += deltaPosition;

            if (_kinMoveElapsedTime >= _kinMoveDuration)
            {
                isKinematicMovingPositionAdd = false;
                OnPushCompleted?.Invoke();
            }
        }

        else if (_isMovingPositionUpdate)
        {
            _positionElapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(_positionElapsedTime / _positionDuration);
            t = ApplyInterpolation(t, _interpolationType);
            characterActor.Position = Vector3.Lerp(_startPosition, _targetPosition, t);

            if (t >= 1f)
            {
                _isMovingPositionUpdate = false;
                OnMoveCompleted?.Invoke();
            }
        }



    }

    private AnimationCurve velocityCurve;
    private float velocityDuration;
    private Vector3 velocityDirectionNew;
    private bool _isMovingSetCharacterVelocityWithCurve = false;


    private void FixedUpdate()
    {
        if (_isMovingSetCharacterVelocity)
        {
            _velocityPushElapsedTime += Time.fixedDeltaTime;
            characterActor.Velocity = _velocityDirection * _velocityForce;

            if (_velocityPushElapsedTime >= _velocityPushDuration)
            {
                _isMovingSetCharacterVelocity = false;
                characterActor.Velocity = Vector3.zero;
            }
        }

        else if (_isMovingSetCharacterVelocityWithCurve)
        {
            _velocityPushElapsedTime += Time.fixedDeltaTime;

            float t = _velocityPushElapsedTime / velocityDuration;
            float curveValue = velocityCurve.Evaluate(t);

            characterActor.Velocity = velocityDirectionNew * curveValue * 10;

            if (t >= 1f)
            {
                _isMovingSetCharacterVelocityWithCurve = false;
                characterActor.PlanarVelocity = Vector3.zero;
            }
        }


        if (_isRotating)
        {
            _rotationElapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(_rotationElapsedTime / _rotationDuration);
            t = ApplyInterpolation(t, _interpolationType);
            Vector3 currentForward = Vector3.Slerp(_startForward, _targetForward, t);
            characterActor.SetYaw(currentForward);

            if (t >= 1f)
            {
                _isRotating = false;
                //characterActor.SetYaw(targetForward);
                OnRotateCompleted?.Invoke();
            }
        }

        if (enteredLedgeForward)
        {
            ledgeElapsedTime += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(ledgeElapsedTime / ledgeDuration);
            t = ApplyInterpolation(t, CharacterState.InterpolationType.EaseIn);
            Vector3 splinePosition = Vector3.Lerp(ledgeEntryStartPosition, splineContainer.EvaluatePosition(t), t);
            characterActor.Position = splinePosition;

            if (t >= 1f)
            {
                enteredLedgeForward = false;
            }
        }

        else if (enteredLedgeAbove)
        {
            ledgeElapsedTime += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(ledgeElapsedTime / ledgeDuration);
            t = ApplyInterpolation(t, CharacterState.InterpolationType.EaseIn);
            float reversedT = 1 - t;
            Vector3 splinePosition = Vector3.Lerp(ledgeEntryStartPosition, splineContainer.EvaluatePosition(reversedT), t);
            characterActor.Position = splinePosition;

            if (t >= 1f)
            {
                enteredLedgeAbove = false;
            }
        }

    }

    public bool IsPushing => isKinematicMovingPositionAdd;
    public bool IsMoving => _isMovingPositionUpdate;
    public bool IsRotating => _isRotating;

    public Action OnPushCompleted;
    public Action OnMoveCompleted;
    public Action OnRotateCompleted;

    private float ApplyInterpolation(float t, CharacterState.InterpolationType interpolationType)
    {
        switch (interpolationType)
        {
            case CharacterState.InterpolationType.Linear:
                return t;
            case CharacterState.InterpolationType.EaseIn:
                return t * t;
            case CharacterState.InterpolationType.EaseOut:
                return 1 - Mathf.Pow(1 - t, 2);
            case CharacterState.InterpolationType.EaseInOut:
                return Mathf.SmoothStep(0, 1, t);
            default:
                return t;
        }
    }
}
