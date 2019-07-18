using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINPCState_Idle1 : AINPCState
{
    public Vector2 _idleTimeRange = new Vector2(10f, 60f);

    private float _idleTime;
    private float _timer;

    public override AIStateType GetStateType()
    {
        return AIStateType.Idle;
    }

    public override void OnEnterState()
    {
        Debug.Log("Entered Idle State");
        base.OnEnterState();
        if (_npcStateMachine == null)
            return;

        _idleTime = Random.Range(_idleTimeRange.x, _idleTimeRange.y);
        _timer = 0f;

        _npcStateMachine.NavAgentControl(true, false);
        _npcStateMachine.speed = 0;
        _npcStateMachine.seeking = 0;
        _npcStateMachine.ClearTarget();
    }

    public override AIStateType OnUpdate()
    {
        if (_npcStateMachine == null)
            return AIStateType.Idle;

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

        _timer += Time.deltaTime;

        if (_timer > _idleTime)
            return AIStateType.Patrol;

        return AIStateType.Idle;
    }
}
