using OpenTK;
using OpenTK.Input;
using System.Drawing;


//This is a slimmed down version of InputManager from
//https://github.com/NullandKale/SimpleTopDownShooter/blob/master/CS162Final/Managers/InputManager.cs

namespace nullEngine.Managers
{
    public class InputManager
    {
        //keyboard state storage, one for the current frame and one for the last frame
        private KeyboardState lastKeyState;
        private KeyboardState currentKeyState;

        public InputManager()
        {
            _7DRL.Game.g.onUpdate.Add(update);
        }

        public void update()
        {
            //on update if the currentKeyState is not invalid set lastKeyState to the old currentKeyState
            if (currentKeyState != null)
            {
                lastKeyState = currentKeyState;
            }

            //update currentKeyState
            currentKeyState = Keyboard.GetState();

        }
        //keyboard state functions
        public bool isKeyRising(Key k)
        {
            if (!isKeystateValid())
            {
                return false;
            }
            else
            {
                return lastKeyState.IsKeyDown(k) && currentKeyState.IsKeyUp(k);
            }
        }

        public bool isKeyFalling(Key k)
        {
            if (!isKeystateValid())
            {
                return false;
            }
            else
            {
                return lastKeyState.IsKeyUp(k) && currentKeyState.IsKeyDown(k);
            }
        }

        public bool isKeyHeld(Key k)
        {
            if (!isKeystateValid())
            {
                return false;
            }
            else
            {
                return lastKeyState.IsKeyDown(k) && currentKeyState.IsKeyDown(k);
            }
        }

        //check that the keyboard state is valid | this might not be needed
        private bool isKeystateValid()
        {
            return currentKeyState != null && lastKeyState != null;
        }
    }
}
