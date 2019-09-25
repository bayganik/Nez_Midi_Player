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
            table.add(new Label("Main Menu").setFontScale(5));
            table.row().setPadTop(20);
            table.add(new Label("This is our main menu for our game!").setFontScale(2));
        }

        protected override void PositionElements()
        {
            var targetHeight = Screen.height / 3;
            table.setBounds(0, Screen.height - targetHeight, Screen.width, targetHeight);
        }
    }
}
