using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINPCState_Alerted1 : AINPCState
{
    public float _maxDuration = 10f;
    public float _waypointAngleThreshold = 90f;
    public float _threatAngleThreshold = 10f;
    public float _directionChangeTime = 1.5f;

    private float _timer = 0f;
    private float _directionChangeTimer = 0f;

    public override AIStateType GetStateType()
    {
        return AIStateType.Alerted;
    }

    public override void OnEnterState()
    {
        Debug.Log("Entered Alerted State");
        base.OnEnterState();
        if (_npcStateMachine == null)
            return;

        _npcStateMachine.NavAgentControl(true, false);
        _npcStateMachine.seeking = 0;
        _npcStateMachine.speed = 0;

        _timer = _maxDuration;
        _directionChangeTimer = 0f;
    }

    public override AIStateType OnUpdate()
    {
        _timer -= Time.deltaTime;
        _directionChangeTimer += Time.deltaTime;

        if (_timer <= 0f)
        {
            _npcStateMachine.agent.SetDestination(_npcStateMachine.GetWaypointPosition(false));
            _npcStateMachine.agent.isStopped = false;
            _timer = _maxDuration;
        }

        if (_npcStateMachine.VisualThreat.type == AITargetType.Visual_Player)
        {
            Debug.Log("Entering Pursuit State");
            _npcStateMachine.SetTarget(_npcStateMachine.VisualThreat);
            return AIStateType.Pursuit;
        }

        if (_npcStateMachine.AudioThreat.type == AITargetType.Audio)
        {
            Debug.Log("Setting Target");
            _npcStateMachine.SetTarget(_npcStateMachine.AudioThreat);
            _timer = _maxDuration;
        }

        float angle;

        if ((_npcStateMachine.targetType == AITargetType.Audio) && !_npcStateMachine.isTargetReached)
        {
            angle = AIState.FindSignedAngle(_npcStateMachine.transform.forward, _npcStateMachine.targetPosition - _npcStateMachine.transform.position);

            if (_npcStateMachine.targetType == AITargetType.Audio && Mathf.Abs(angle) < _threatAngleThreshold)
            {
                return AIStateType.Pursuit;
            }

            if (_directionChangeTimer > _directionChangeTime)
            {
                _npcStateMachine.seeking = (int)Mathf.Sign(angle);
                _directionChangeTimer = 0f;
            }
        }
        else if (_npcStateMachine.targetType == AITargetType.Waypoint && !_npcStateMachine.agent.pathPending)
        {
            angle = AIState.FindSignedAngle(_npcStateMachine.transform.forward, _npcStateMachine.agent.steeringTarget - _npcStateMachine.transform.position);
            if (Mathf.Abs(angle) < _waypointAngleThreshold)
                return AIStateType.Patrol;

            if (_directionChangeTimer > _directionChangeTime)
            {
                _npcStateMachine.seeking = (int)Mathf.Sign(angle);
                _directionChangeTimer = 0f;
            }
        }

        return AIStateType.Alerted;
    }
}
