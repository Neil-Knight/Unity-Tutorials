using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float _speed;
    public float _desiredRotationSpeed;
    public GameObject _inputDirectionCompass;
    public Camera _camera;
    public bool _showSolverDebug = true;
    Vector3 _desiredMoveDirection;
    float _inputX;
    float _inputZ;
    Animator _animator;
    bool _isGrounded = true;
    CharacterController _controller;
    float _angle2Target;
    float _verticalVelocity;
    Vector3 _moveVector;
    bool _isInAir = false;
    float _distanceToGround;

    Vector3 _rightFootPosition;
    Vector3 _leftFootPosition;
    Vector3 _rightFootIKPosition;
    Vector3 _leftFootIKPosition;
    Quaternion _leftFootIKRotation;
    Quaternion _rightFootIKRotation;
    float _lastPelvisPositionY;
    float _lastRightFootPositionY;
    float _lastLeftFootPositionY;

    [Header("Feet Grounder")]
    public bool _enableFeetIK = true;
    [Range(0,2)]
    [SerializeField]
    float _heightFromGroundRaycast = 1.14f;
    [Range(0, 2)]
    [SerializeField]
    float _raycastDownDistance = 1.5f;
    [SerializeField]
    LayerMask _environmentLayer;
    [SerializeField]
    float _pelvisOffset;
    [Range(0, 1)]
    [SerializeField]
    float _pelvisUpAndDownSpeed = 0.28f;
    [Range(0, 1)]
    [SerializeField]
    float _feetToIKPositionSpeed = 0.5f;


    // Start is called before the first frame update
    void Start()
    {
        _animator = this.GetComponent<Animator>();
        _controller = this.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        InputMagnitude();
        CheckGrounding();
    }

    void FixedUpdate()
    {
        if (!_enableFeetIK)
            return;

        // Feet grounding
        AdjustFeetTarget(ref _rightFootPosition, HumanBodyBones.RightFoot);
        AdjustFeetTarget(ref _leftFootPosition, HumanBodyBones.LeftFoot);
        // Find and Raycast to the ground
        FeetPositionSolver(_rightFootPosition, ref _rightFootIKPosition, ref _rightFootIKRotation);
        FeetPositionSolver(_leftFootPosition, ref _leftFootIKPosition, ref _leftFootIKRotation);

        // Parent the Input Direction Compass to the Player
        _inputDirectionCompass.transform.position = transform.position;
        _inputDirectionCompass.transform.rotation = Quaternion.identity;

        _inputX = Input.GetAxis("Horizontal");
        _inputZ = Input.GetAxis("Vertical");
        _animator.SetFloat("InputZ", _inputZ, 0f, Time.deltaTime);
        _animator.SetFloat("InputX", _inputX, 0f, Time.deltaTime);
        _speed = new Vector2(_inputX, _inputZ).sqrMagnitude;
        _animator.SetFloat("InputMagnitude", _speed, 0f, Time.deltaTime);

        Vector3 targetDirection = _inputDirectionCompass.transform.InverseTransformPoint(_desiredMoveDirection);
        _angle2Target = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;

        var cam = Camera.main;
        var forward = cam.transform.forward;
        var right = cam.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        _desiredMoveDirection = forward * _inputZ + right * _inputX;
        _desiredMoveDirection.y = 0f;

        _inputDirectionCompass.transform.Rotate(0, 0, _angle2Target);
        _inputDirectionCompass.transform.Translate(_desiredMoveDirection);

        if (_desiredMoveDirection != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_desiredMoveDirection), _desiredRotationSpeed * Time.deltaTime);

        _controller.Move(_desiredMoveDirection * Time.deltaTime);
    }

    void InputMagnitude()
    {
        bool running = Input.GetButton("Running");
        _animator.SetBool("isRunning", running);

        bool crouching = Input.GetButton("Crouching");
        _animator.SetBool("isCrouching", crouching);

        _animator.SetBool("isGrounded", _isGrounded);

        if (_isGrounded && !_isInAir)
        {
            bool jumping = Input.GetButton("Jump");
            if (jumping)
            {
                _animator.SetBool("isJumping", jumping);
                _isInAir = true;
                jumping = false;
            }
        }
        else
            _isInAir &= !_isGrounded;
    }

    void CheckGrounding()
    {
        if (_isGrounded)
            _distanceToGround = 0.1f;
        else
            _distanceToGround = 0.35f;

        if (Physics.CheckCapsule(transform.position, Vector3.down, _distanceToGround, 1 << LayerMask.NameToLayer("Ground")))
            _isGrounded = true;
        else
            _isGrounded = false;
    }

    #region Feet Grounding
    private void OnAnimatorIK(int layerIndex)
    {
        if (!_enableFeetIK)
            return;

        MovePelvisHeight();
        _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
        MoveFeetToIKPoint(AvatarIKGoal.RightFoot, _rightFootIKPosition, _rightFootIKRotation, ref _lastRightFootPositionY);
        _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
        MoveFeetToIKPoint(AvatarIKGoal.LeftFoot, _leftFootIKPosition, _leftFootIKRotation, ref _lastLeftFootPositionY);
    }
    #endregion

    #region Feet Grounding Methods
    void MoveFeetToIKPoint(AvatarIKGoal foot, Vector3 positionIKHolder, Quaternion rotationIKHolder, ref float lastFootPositionY)
    {
        Vector3 targetIKPosition = _animator.GetIKPosition(foot);
        if (positionIKHolder != Vector3.zero)
        {
            targetIKPosition = transform.InverseTransformPoint(targetIKPosition);
            positionIKHolder = transform.InverseTransformPoint(positionIKHolder);
            float y = Mathf.Lerp(lastFootPositionY, positionIKHolder.y, _feetToIKPositionSpeed);
            targetIKPosition.y += y;
            lastFootPositionY = y;
            targetIKPosition = transform.TransformPoint(targetIKPosition);
            _animator.SetIKRotation(foot, rotationIKHolder);
        }

        _animator.SetIKPosition(foot, targetIKPosition);
    }

    void MovePelvisHeight()
    {
        if (_rightFootIKPosition == Vector3.zero || _leftFootIKPosition == Vector3.zero ||
            _lastPelvisPositionY == 0)
        {
            _lastPelvisPositionY = _animator.bodyPosition.y;
            return;
        }

        float leftOffsetPosition = _leftFootIKPosition.y - transform.position.y;
        float rightOffsetPosition = _rightFootIKPosition.y - transform.position.y;
        float totalOffset = (leftOffsetPosition < rightOffsetPosition) ? leftOffsetPosition : rightOffsetPosition;
        Vector3 newPelvisPosition = _animator.bodyPosition + Vector3.up * totalOffset;
        newPelvisPosition.y = Mathf.Lerp(_lastPelvisPositionY, newPelvisPosition.y, _pelvisUpAndDownSpeed);
        _animator.bodyPosition = newPelvisPosition;
        _lastPelvisPositionY = _animator.bodyPosition.y;
    }

    void FeetPositionSolver(Vector3 fromSkyPosition, ref Vector3 feetIKPositions, ref Quaternion feetIKRotations)
    {
        // Raycast handling section
        RaycastHit feetOutHit;
        if (_showSolverDebug)
            Debug.DrawLine(fromSkyPosition, fromSkyPosition + Vector3.down * (_raycastDownDistance + _heightFromGroundRaycast), Color.yellow);

        if (Physics.Raycast(fromSkyPosition, Vector3.down, out feetOutHit, _raycastDownDistance + _heightFromGroundRaycast, _environmentLayer))
        {
            feetIKPositions = fromSkyPosition;
            feetIKPositions.y = feetOutHit.point.y + _pelvisOffset;
            feetIKRotations = Quaternion.FromToRotation(Vector3.up, feetOutHit.normal) * transform.rotation;
            return;
        }

        feetIKPositions = Vector3.zero;
    }

    void AdjustFeetTarget(ref Vector3 feetPositions, HumanBodyBones foot)
    {
        feetPositions = _animator.GetBoneTransform(foot).position;
        feetPositions.y = transform.position.y + _heightFromGroundRaycast;
    }
    #endregion
}
