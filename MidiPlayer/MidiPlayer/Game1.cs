using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Nez;

namespace MidiPlayer
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Core
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1() : base(1600, 900, false, true, "Using Sanford MIDI Library Example")
        { }
        protected override void Initialize()
        {

            base.Initialize();
            //System.Diagnostics.Debug.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(System.Console.Out));

            IsMouseVisible = true;
            debugRenderEnabled = true;
            Window.AllowUserResizing = true;
            debugRenderEnabled = false;

            scene = new Scenes.MainScene();
        }

    }
}
