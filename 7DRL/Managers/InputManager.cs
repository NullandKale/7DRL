using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DRL.Managers
{
    public static class InputManager
    {
        public static bool isKeyFalling(Key k)
        {
            KeyStates kS = Keyboard.GetKeyStates(k);

            if (kS == KeyStates.Down && kS == KeyStates.Toggled)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool isKeyRising(Key k)
        {
            KeyStates kS = Keyboard.GetKeyStates(k);

            if (kS == KeyStates.None && kS == KeyStates.Toggled)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool isKeyHeld(Key k)
        {
            KeyStates kS = Keyboard.GetKeyStates(k);

            if(kS == KeyStates.Down && kS != KeyStates.Toggled)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
