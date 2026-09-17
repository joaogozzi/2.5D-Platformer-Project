using UnityEngine;
using UnityEngine.InputSystem;

public class InputRemapping : MonoBehaviour
{
    private InputActionsMap _inputActions;
    private const string KEYBINDINGS_SAVE_KEY = "PlayerKeybindings";

    private void Awake()
    {
        _inputActions = InputManager.Instance.GetInputActions();
        LoadKeybindings();
    }

    public void RemapAction(string actionName, InputBinding newBinding, int bindingIndex = 0)
    {
        InputAction action = _inputActions.asset.FindAction(actionName);

        if (action != null)
        {
            // Remove conflitos com o novo mapeamento
            InputActionRebindingExtensions.ApplyBindingOverride(
                action,
                bindingIndex,
                newBinding);

            SaveKeybindings();
        }
    }

    public void ResetToDefault()
    {
        foreach (InputActionMap map in _inputActions.asset.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }
        SaveKeybindings();
    }

    private void SaveKeybindings()
    {
        string rebinds = _inputActions.asset.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(KEYBINDINGS_SAVE_KEY, rebinds);
        PlayerPrefs.Save();
    }

    private void LoadKeybindings()
    {
        if (PlayerPrefs.HasKey(KEYBINDINGS_SAVE_KEY))
        {
            string rebinds = PlayerPrefs.GetString(KEYBINDINGS_SAVE_KEY);
            _inputActions.asset.LoadBindingOverridesFromJson(rebinds);
        }
    }

    // Método para iniciar o processo de remapeamento interativo
    public void StartInteractiveRebind(string actionName, System.Action onComplete = null)
    {
        InputAction action = _inputActions.asset.FindAction(actionName);

        if (action == null) return;

        var rebindOperation = action.PerformInteractiveRebinding()
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(op =>
            {
                SaveKeybindings();
                onComplete?.Invoke();
                op.Dispose();
            })
            .Start();
    }
}