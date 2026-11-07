using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace DeepCurrent
{
    // Reads keyboard + mouse once per frame, before the game logic.
    // Works with both the legacy and the new Input System.
    [DefaultExecutionOrder(-80)]
    public class InputBridge : Singleton<InputBridge>
    {
        public Vector2 MoveAxis { get; private set; }
        public Vector2 MouseScreenPos { get; private set; }
        public bool MouseHeld { get; private set; }
        public bool KeyFireHeld { get; private set; }
        public bool PausePressed { get; private set; }
        public bool EnterPressed { get; private set; }

        void Update()
        {
            PausePressed = false;
            EnterPressed = false;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            ReadNewInput();
#else
            ReadLegacyInput();
#endif
        }

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        void ReadNewInput()
        {
            float mx = 0f, my = 0f;
            Keyboard kb = Keyboard.current;
            if (kb != null)
            {
                if (kb.wKey.isPressed || kb.upArrowKey.isPressed) my += 1f;
                if (kb.sKey.isPressed || kb.downArrowKey.isPressed) my -= 1f;
                if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) mx -= 1f;
                if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) mx += 1f;

                PausePressed = kb.pKey.wasPressedThisFrame || kb.escapeKey.wasPressedThisFrame;
                EnterPressed = kb.enterKey.wasPressedThisFrame || kb.numpadEnterKey.wasPressedThisFrame;
                KeyFireHeld = kb.spaceKey.isPressed || kb.jKey.isPressed;
            }

            Mouse ms = Mouse.current;
            MouseHeld = ms != null && ms.leftButton.isPressed;
            if (ms != null) MouseScreenPos = ms.position.ReadValue();

            MoveAxis = new Vector2(mx, my);
        }
#else
        void ReadLegacyInput()
        {
            float mx = 0f, my = 0f;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) my += 1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) my -= 1f;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) mx -= 1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) mx += 1f;

            PausePressed = Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape);
            EnterPressed = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);

            MouseScreenPos = Input.mousePosition;
            MouseHeld = Input.GetMouseButton(0);
            KeyFireHeld = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.J);

            MoveAxis = new Vector2(mx, my);
        }
#endif

        // True while the pointer sits on top of a UI element (buttons).
        public bool PointerOverUI()
        {
            var es = UnityEngine.EventSystems.EventSystem.current;
            return es != null && es.IsPointerOverGameObject();
        }
    }
}
