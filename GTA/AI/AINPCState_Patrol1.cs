using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINPCState_Patrol1 : AINPCState
{
    public float _turnOnSpotThreshold = 80f;
    [Range(0f, 3f)]
    public float _speed = 1f;
    public float _slerpSpeed = 5f;

    public override AIStateType GetStateType()
    {
        return AIStateType.Patrol;
    }

    public override void OnEnterState()
    {
        Debug.Log("Entered Patrol state");
        base.OnEnterState();
        if (_npcStateMachine == null)
            return;

        _npcStateMachine.NavAgentControl(true, false);
        _npcStateMachine.seeking = 0;
        _npcStateMachine.speed = _speed;
        _npcStateMachine.agent.SetDestination(_npcStateMachine.GetWaypointPosition(false));
        _npcStateMachine.agent.isStopped = false;
    }

    public override AIStateType OnUpdate()
    {
        if (_npcStateMachine.VisualThreat.type == AITargetType.Visual_Player)
        {
            _npcStateMachine.SetTarget(_npcStateMachine.VisualThreat);
            return AIStateType.Pursuit;
        }

        if (_npcStateMachine.AudioThreat.type == AITargetType.Audio)
        {
            _npcStateMachine.SetTarget(_npcStateMachine.AudioThreat);
            return AIStateType.Alerted;
        }

        float angle = Vector3.Angle(_npcStateMachine.transform.forward, (_npcStateMachine.agent.steeringTarget - _npcStateMachine.transform.position));
        if (angle > _turnOnSpotThreshold)
        {
            return AIStateType.Alerted;
        }

        if (!_npcStateMachine.useRootRotation)
        {
            Quaternion newRot = Quaternion.LookRotation(_npcStateMachine.agent.desiredVelocity);
            _npcStateMachine.transform.rotation = Quaternion.Slerp(_npcStateMachine.transform.rotation, newRot, Time.deltaTime * _slerpSpeed);
        }

        if (_npcStateMachine.agent.isPathStale || !_npcStateMachine.agent.hasPath ||
            _npcStateMachine.agent.pathStatus != UnityEngine.AI.NavMeshPathStatus.PathComplete)
        {
            _npcStateMachine.agent.SetDestination(_npcStateMachine.GetWaypointPosition(true));
        }

        return AIStateType.Patrol;
    }

    public override void OnDestinationReached(bool isReached)
    {
        if (_npcStateMachine == null || !isReached)
            return;

        if (_npcStateMachine.targetType == AITargetType.Waypoint)
            _npcStateMachine.agent.SetDestination(_npcStateMachine.GetWaypointPosition(true));
    }

    /*public override void OnAnimatorIKUpdated()
    {
        if (_npcStateMachine == null)
            return;

        _npcStateMachine.animator.SetLookAtPosition(_npcStateMachine.targetPosition + Vector3.up);
        _npcStateMachine.animator.SetLookAtWeight(0.55f);
    }*/
}
