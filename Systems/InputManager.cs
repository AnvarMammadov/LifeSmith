using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace LifeSmith.Systems
{
    public class InputManager
    {
        private static InputManager _instance;
        public static InputManager Instance => _instance ??= new InputManager();

        private MouseState _currentMouseState;
        private MouseState _previousMouseState;
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;

        public Point MousePosition => _currentMouseState.Position;
        public bool IsLeftMouseButtonPressed => _currentMouseState.LeftButton == ButtonState.Pressed;
        public bool WasLeftMouseButtonJustPressed => 
            _currentMouseState.LeftButton == ButtonState.Pressed && 
            _previousMouseState.LeftButton == ButtonState.Released;
        public bool WasLeftMouseButtonJustReleased => 
            _currentMouseState.LeftButton == ButtonState.Released && 
            _previousMouseState.LeftButton == ButtonState.Pressed;

        private InputManager() { }

        public void Update()
        {
            _previousMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();

            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();
        }

        public bool IsKeyPressed(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key);
        }

        public bool WasKeyJustPressed(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);
        }
        
        public bool IsKeyJustPressed(Keys key) => WasKeyJustPressed(key);

        public bool IsMouseInRectangle(Rectangle rect)
        {
            return rect.Contains(MousePosition);
        }
    }
}
