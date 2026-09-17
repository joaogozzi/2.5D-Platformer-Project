using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputBuffering : MonoBehaviour
{
    private static InputBuffering _instance;
    public static InputBuffering Instance => _instance;

    private Dictionary<string, BufferedInput> _bufferedInputs = new Dictionary<string, BufferedInput>();
    private float _bufferDuration = 0.2f; // 200ms de buffer

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void BufferInput(string inputName, InputAction.CallbackContext context)
    {
        if (!_bufferedInputs.ContainsKey(inputName))
        {
            _bufferedInputs.Add(inputName, new BufferedInput(inputName, context, Time.time));
        }
        else
        {
            _bufferedInputs[inputName] = new BufferedInput(inputName, context, Time.time);
        }
    }

    public bool TryGetBufferedInput(string inputName, out InputAction.CallbackContext context)
    {
        context = new InputAction.CallbackContext();

        if (_bufferedInputs.TryGetValue(inputName, out BufferedInput bufferedInput))
        {
            if (Time.time - bufferedInput.TimeBuffered <= _bufferDuration)
            {
                context = bufferedInput.Context;
                _bufferedInputs.Remove(inputName);
                return true;
            }
            _bufferedInputs.Remove(inputName);
        }

        return false;
    }

    public void ClearBuffer()
    {
        _bufferedInputs.Clear();
    }

    private class BufferedInput
    {
        public string InputName { get; }
        public InputAction.CallbackContext Context { get; }
        public float TimeBuffered { get; }

        public BufferedInput(string inputName, InputAction.CallbackContext context, float timeBuffered)
        {
            InputName = inputName;
            Context = context;
            TimeBuffered = timeBuffered;
        }
    }
}
