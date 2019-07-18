using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINPCState_Pursuit1 : AINPCState
{
    public float _speed = 1f;
    public float _slerpSpeed = 5f;
    public float _repathDistanceMultiplier = 0.035f;
    public float _repathVisualMinDuration = 0.05f;
    public float _repathVisualMaxDuration = 5f;
    public float _repathAudioMinDuration = 0.25f;
    public float _repathAudioMaxDuration = 5f;
    public float _maxDuration = 40f;

    private float _timer;
    private float _repathTimer;

    public override AIStateType GetStateType()
    {
        return AIStateType.Pursuit;
    }

    public override void OnEnterState()
    {
        Debug.Log("Entered Pursuit State");
        base.OnEnterState();
        if (_npcStateMachine == null)
            return;

        _npcStateMachine.NavAgentControl(true, false);
        _npcStateMachine.seeking = 0;
        _npcStateMachine.speed = _speed;

        _timer = 0f;
        _repathTimer = 0f;

        _npcStateMachine.agent.SetDestination(_npcStateMachine.targetPosition);
        _npcStateMachine.agent.isStopped = false;
    }

    public override AIStateType OnUpdate()
    {
        _timer += Time.deltaTime;
        _repathTimer += Time.deltaTime;

        if (_timer > _maxDuration)
            return AIStateType.Patrol;

        // Add in method of checking if we are in shooting range
        /*if (_npcStateMachine.targetType == AITargetType.Visual_Player && _npcStateMachine.inShootingRange)
        {
            return AIStateType.Attack;
        }*/

        if (_npcStateMachine.isTargetReached)
        {
            switch (_stateMachine.targetType)
            {
                case AITargetType.Audio:
                    _stateMachine.ClearTarget();
                    return AIStateType.Alerted;
            }
        }

        if (_npcStateMachine.agent.isPathStale ||
            !_npcStateMachine.agent.hasPath ||
            _npcStateMachine.agent.pathStatus != UnityEngine.AI.NavMeshPathStatus.PathComplete)
        {
            return AIStateType.Alerted;
        }

        if (!_npcStateMachine.useRootRotation && _npcStateMachine.targetType == AITargetType.Visual_Player &&
            _npcStateMachine.VisualThreat.type == AITargetType.Visual_Player && _npcStateMachine.isTargetReached)
        {
            Vector3 targetPos = _npcStateMachine.targetPosition;
            targetPos.y = _npcStateMachine.transform.position.y;
            Quaternion newRot = Quaternion.LookRotation(targetPos - _npcStateMachine.transform.position);
            _npcStateMachine.transform.rotation = newRot;
        }
        else if (!_npcStateMachine.useRootRotation && !_npcStateMachine.isTargetReached)
        {
            Quaternion newRot = Quaternion.LookRotation(_npcStateMachine.agent.desiredVelocity);
            _npcStateMachine.transform.rotation = Quaternion.Slerp(_npcStateMachine.transform.rotation, newRot, Time.deltaTime * _slerpSpeed);
        }
        else if (_npcStateMachine.isTargetReached)
        {
            return AIStateType.Alerted;
        }

        // Visual threat that's a player
        if (_npcStateMachine.VisualThreat.type == AITargetType.Visual_Player)
        {
            if (_npcStateMachine.targetPosition != _npcStateMachine.VisualThreat.position)
            {
                if (Mathf.Clamp(_npcStateMachine.VisualThreat.distance * _repathDistanceMultiplier, _repathVisualMinDuration, _repathVisualMaxDuration) < _repathTimer)
                {
                    _npcStateMachine.agent.SetDestination(_npcStateMachine.VisualThreat.position);
                    _repathTimer = 0f;
                }
            }

            _npcStateMachine.SetTarget(_npcStateMachine.VisualThreat);

            return AIStateType.Pursuit;
        }

        if (_npcStateMachine.targetType == AITargetType.Visual_Player)
            return AIStateType.Patrol;

        if (_npcStateMachine.AudioThreat.type == AITargetType.Audio)
        {
            if (_npcStateMachine.targetType == AITargetType.Audio)
            {
                int currentID = _npcStateMachine.targetColliderID;
                if (currentID == _npcStateMachine.AudioThreat.collider.GetInstanceID())
                {
                    if (_npcStateMachine.targetPosition != _npcStateMachine.AudioThreat.position)
                    {
                        if (Mathf.Clamp(_npcStateMachine.AudioThreat.distance * _repathDistanceMultiplier, _repathAudioMinDuration, _repathAudioMaxDuration) < _repathTimer)
                        {
                            _npcStateMachine.agent.SetDestination(_npcStateMachine.AudioThreat.position);
                            _repathTimer = 0f;
                        }

                        _npcStateMachine.SetTarget(_npcStateMachine.AudioThreat);
                        return AIStateType.Pursuit;
                    }
                }
                else
                {
                    _npcStateMachine.SetTarget(_npcStateMachine.AudioThreat);
                    return AIStateType.Alerted;
                }
            }
        }

        return AIStateType.Pursuit;
    }
}
