using System;
using UnityEngine;

public class GameInputs : MonoBehaviour {

    public static GameInputs Instance;
    private PlayerInputAction inputActions;

    public event EventHandler<OnInputTrackEventArgs> OnMouseLeftClick;
    public event EventHandler<OnInputTrackEventArgs> OnTouch;
    public class OnInputTrackEventArgs : EventArgs {
        public UnityEngine.InputSystem.InputAction.CallbackContext ctx;
    }

    private void Awake() {
        if(Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        inputActions = new PlayerInputAction();
        inputActions.Game.Enable();

        inputActions.Game.MouseLeftDown.performed += MouseLeftDownPerformed;
        inputActions.Game.MouseLeftDown.canceled += MouseLeftDownPerformed;

        inputActions.Game.TouchInputs.performed += TouchInputPerformed;
        inputActions.Game.TouchInputs.canceled += TouchInputPerformed;
    }

    private void OnEnable() {
        if(inputActions != null)
            inputActions.Game.Enable();
    }

    private void OnDisable() {
        inputActions.Game.Disable();
    }   

    private void MouseLeftDownPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnMouseLeftClick?.Invoke(this, new OnInputTrackEventArgs { ctx = obj });
    }

    private void TouchInputPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnTouch?.Invoke(this, new OnInputTrackEventArgs { ctx = obj });
    }

}