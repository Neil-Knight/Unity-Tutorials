using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    private static GameSceneManager _instance;

    public static GameSceneManager instance
    {
        get
        {
            if (_instance == null)
                _instance = (GameSceneManager)FindObjectOfType(typeof(GameSceneManager));

            return _instance;
        }
    }

    private Dictionary<int, AIStateMachine> _stateMachines = new Dictionary<int, AIStateMachine>();

    public void RegisterAIStateMachine(int key, AIStateMachine stateMachine)
    {
        if (!_stateMachines.ContainsKey(key))
            _stateMachines[key] = stateMachine;
    }

    public AIStateMachine GetAIStateMachine(int key)
    {
        AIStateMachine machine;
        if (_stateMachines.TryGetValue(key, out machine))
        {
            return machine;
        }

        return null;
    }
}
