using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AINPCState : AIState
{
	protected AINPCStateMachine _npcStateMachine;
    protected int _visualLayerMask = -1;
    protected int _playerLayerMask = -1;

    private void Awake()
    {
        _playerLayerMask = LayerMask.GetMask("Player");
        _visualLayerMask = LayerMask.GetMask("Player", "Visual Aggravator") + 1;
    }

    public override void SetStateMachine(AIStateMachine stateMachine)
	{
        if (stateMachine.GetType() == typeof(AINPCStateMachine))
		{
			base.SetStateMachine(stateMachine);
			_npcStateMachine = (AINPCStateMachine)stateMachine;
		}
	}

    public override void OnTriggerEvent(AITriggerEventType eventType, Collider other)
	{
		if (_npcStateMachine == null)
			return;

        if (eventType != AITriggerEventType.Exit)
        {
            AITargetType curType = _npcStateMachine.VisualThreat.type;

            if (other.CompareTag("Player"))
            {
                float distance = Vector3.Distance(_npcStateMachine.sensorPosition, other.transform.position);
                if (curType != AITargetType.Visual_Player || (curType == AITargetType.Visual_Player && distance < _npcStateMachine.VisualThreat.distance))
                {
                    RaycastHit hitInfo;
                    if (ColliderIsVisible(other, out hitInfo, _playerLayerMask))
                        _npcStateMachine.VisualThreat.Set(AITargetType.Visual_Player, other, other.transform.position, distance);
                }
            }
            else if (other.CompareTag("AI Sound Emitter"))
            {
                SphereCollider soundTrigger = (SphereCollider)other;
                if (soundTrigger == null)
                    return;

                Vector3 agentSensorPosition = _npcStateMachine.sensorPosition;
                Vector3 soundPos;
                float soundRadius;
                AIState.ConvertSphereColliderToWorldSpace(soundTrigger, out soundPos, out soundRadius);
                float distanceToThreat = (soundPos - agentSensorPosition).magnitude;
                float distanceFactor = (distanceToThreat / soundRadius);

                // Too far away
                if (distanceFactor > 1.0f)
                    return;

                if (distanceToThreat < _npcStateMachine.AudioThreat.distance)
                {
                    _npcStateMachine.AudioThreat.Set(AITargetType.Audio, other, soundPos, distanceToThreat);
                }
            }
        }
	}

    protected virtual bool ColliderIsVisible(Collider other, out RaycastHit hitInfo, int layerMask = -1)
    {
        hitInfo = new RaycastHit();
        if (_npcStateMachine == null)
            return false;

        Vector3 head = _stateMachine.sensorPosition;
        Vector3 direction = other.transform.position - head;
        float angle = Vector3.Angle(direction, transform.forward);

        if (angle > _npcStateMachine.fieldOfView * 0.5f)
            return false;

        RaycastHit[] hits = Physics.RaycastAll(head, direction.normalized, _npcStateMachine.sensorRadius * _npcStateMachine.sight, layerMask);
        float closestColliderDistance = float.MaxValue;
        Collider closestCollider = null;
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.distance < closestColliderDistance)
            {
                closestColliderDistance = hit.distance;
                closestCollider = hit.collider;
                hitInfo = hit;
            }
        }

        if (closestCollider && closestCollider.gameObject == other.gameObject)
            return true;

        return false;
    }
}
