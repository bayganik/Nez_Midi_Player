using Microsoft.Xna.Framework;
using Nez;
using Nez.UI;
namespace MidiPlayer.Scenes
{
    class DialogBar : BaseUI
    {

        public DialogBar(int renderLayer) : base(renderLayer) { }

        protected override void Setup()
        {
            SetBackgroundColor(new Color(60, 60, 60));
            table.Add(new Label("Main Menu").SetFontScale(5));
            table.Row().SetPadTop(20);
            table.Add(new Label("This is our main menu for our game!").SetFontScale(2));
        }

        protected override void PositionElements()
        {
            var targetHeight = Screen.Height / 3;
            table.SetBounds(0, Screen.Height - targetHeight, Screen.Width, targetHeight);
        }
    }
}
