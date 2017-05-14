using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;

namespace template
{
    class InputHandler
    {
        KeyboardState previousState;
        KeyboardState currentState;

        public InputHandler()
        {
            currentState = OpenTK.Input.Keyboard.GetState();
            previousState = currentState;
        }

        public void Update()
        {
            previousState = currentState;
            currentState = OpenTK.Input.Keyboard.GetState();
        }

        public bool KeyDown(Key key)
        {
            return !previousState[key] && currentState[key];
        }
    }
}
