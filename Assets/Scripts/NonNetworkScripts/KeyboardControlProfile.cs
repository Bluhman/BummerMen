using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class KeyboardControlProfile : UnityInputDeviceProfile
{

    public KeyboardControlProfile()
    {
        Name = "Keyboard";
        Meta = "Keyboard control for this silly bomb game.";

        SupportedPlatforms = new[]
            {
                "Windows",
                "Mac",
                "Linux"
            };

        Sensitivity = 1.0f;
        LowerDeadZone = 0.0f;
        UpperDeadZone = 1.0f;

        ButtonMappings = new[]
        {
            new InputControlMapping
            {
                Handle = "Lay Bomb",
                Target = InputControlType.Action1,
                Source = KeyCodeButton(KeyCode.Space)
            },
            new InputControlMapping
            {
                Handle = "Special Bomb",
                Target = InputControlType.Action2,
                Source = KeyCodeButton(KeyCode.LeftControl, KeyCode.RightControl)
            },
            new InputControlMapping
            {
                Handle = "Detonate",
                Target = InputControlType.Action3,
                Source = KeyCodeButton(KeyCode.LeftShift, KeyCode.RightShift)
            },
            new InputControlMapping
            {
                Handle = "Pause",
                Target = InputControlType.Menu,
                Source = KeyCodeButton(KeyCode.Escape, KeyCode.Return)
            }
        };

        AnalogMappings = new[]
        {
            new InputControlMapping
            {
                Handle = "Move X",
                Target = InputControlType.LeftStickX,
                Source = KeyCodeAxis(KeyCode.A, KeyCode.D)
            },
            new InputControlMapping
                {
                    Handle = "Move Y",
                    Target = InputControlType.LeftStickY,
                    Source = KeyCodeAxis( KeyCode.S, KeyCode.W )
                }
        };
    }
}
