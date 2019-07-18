using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINPCStateMachine : AIStateMachine
{
    [SerializeField]
    [Range(10f, 360f)]
	float _fieldOfView = 50f;
    [SerializeField]
	int _health = 100;
    [SerializeField]
    [Range(0f, 1f)]
    float _sight = 0.5f;
    
	private float _speed = 0f;
    private int _seeking = 0;

    public float fieldOfView
    {
        get { return _fieldOfView; }
    }

    public int seeking
    {
        get { return _seeking; }
        set { _seeking = value; }
    }

    public float sight { get { return _sight; } }

    public float speed
	{
        get { return _speed; }
        set { _speed = value; }
	}

    protected override void Update()
	{
		base.Update();

        if (_animator != null)
		{
            _animator.SetInteger("seeking", _seeking);
			_animator.SetFloat("speed", _speed);
		}
	}
}
