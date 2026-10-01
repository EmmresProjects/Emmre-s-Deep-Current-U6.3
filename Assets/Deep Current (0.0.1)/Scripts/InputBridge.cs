using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace DeepCurrent
{
    // Keyboard and mouse input. Read it before the game loop.
    [DefaultExecutionOrder(-80)]
    public class InputBridge : MonoBehaviour
    {
        public Vector2 MoveAxis { get; private set; }
        public Vector2 MouseScreenPos { get; private set; }
        public bool FireHeld { get; private set; }
        public bool MouseHeld { get; private set; }
        public bool KeyFireHeld { get; private set; }
        public bool PausePressed { get; private set; }
        public bool EnterPressed { get; private set; }

        void Awake()
        {
            DC.Input = this;
        }

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
            }

            Mouse ms = Mouse.current;
            MouseHeld = ms != null && ms.leftButton.isPressed;
            if (ms != null) MouseScreenPos = ms.position.ReadValue();
            KeyFireHeld = kb != null && (kb.spaceKey.isPressed || kb.jKey.isPressed);
            FireHeld = MouseHeld || KeyFireHeld;

            MoveAxis = new Vector2(mx, my);
        }
#else
        void ReadLegacyInput()
        {
            float mx = 0f, my = 0f;
            if (UnityEngine.Input.GetKey(KeyCode.W) || UnityEngine.Input.GetKey(KeyCode.UpArrow)) my += 1f;
            if (UnityEngine.Input.GetKey(KeyCode.S) || UnityEngine.Input.GetKey(KeyCode.DownArrow)) my -= 1f;
            if (UnityEngine.Input.GetKey(KeyCode.A) || UnityEngine.Input.GetKey(KeyCode.LeftArrow)) mx -= 1f;
            if (UnityEngine.Input.GetKey(KeyCode.D) || UnityEngine.Input.GetKey(KeyCode.RightArrow)) mx += 1f;

            PausePressed = UnityEngine.Input.GetKeyDown(KeyCode.P) || UnityEngine.Input.GetKeyDown(KeyCode.Escape);
            EnterPressed = UnityEngine.Input.GetKeyDown(KeyCode.Return) || UnityEngine.Input.GetKeyDown(KeyCode.KeypadEnter);

            MouseScreenPos = UnityEngine.Input.mousePosition;
            MouseHeld = UnityEngine.Input.GetMouseButton(0);
            KeyFireHeld = UnityEngine.Input.GetKey(KeyCode.Space) || UnityEngine.Input.GetKey(KeyCode.J);
            FireHeld = MouseHeld || KeyFireHeld;

            MoveAxis = new Vector2(mx, my);
        }
#endif

        // True while the pointer sits on top of a UI element (Buttons)
        public bool PointerOverUI()
        {
            var es = UnityEngine.EventSystems.EventSystem.current;
            if (es == null) return false;
            return es.IsPointerOverGameObject();
        }
    }
}
