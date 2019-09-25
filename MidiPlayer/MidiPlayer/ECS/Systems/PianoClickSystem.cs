using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.UI;
using Nez.Sprites;
using MidiPlayer.Scenes;
using MidiPlayer.ECS.Components;
using Sanford.Multimedia.Midi;

namespace MidiPlayer.ECS.Systems
{
    public class PianoClickSystem : EntityProcessingSystem
    {
        /*
         * System used to visually display the clicking of piano keys
         */
        MouseState CurrentMouse;
        Vector2 MousePos;
        BoxCollider MouseCollider;

        MainScene MainGameScene;
        Entity PrevKey;

        public PianoClickSystem(Matcher matcher) : base(matcher)
        {

        }
        public override void process(Entity entity)
        {
            //
            // ONLY piano key entities come here
            //
            MainGameScene = entity.scene as MainScene;              //hand entity belongs to MainScene

            Sprite sp = entity.getComponent<Sprite>();
            PlayingComponent pc = entity.getComponent<PlayingComponent>();
            if (pc.ChannelMsg.Command == ChannelCommand.NoteOn)
            {
                sp.color = Color.Blue;
            }
            else
            {
                sp.color = Color.White;
            }
            entity.removeComponent<PlayingComponent>();

            if (PrevKey == null)
            {
                PrevKey = entity;
                return;
            }

            Sprite PrevSp = PrevKey.getComponent<Sprite>();
            PrevSp.color = Color.White;
            PrevKey = entity;

        }
    }
}
