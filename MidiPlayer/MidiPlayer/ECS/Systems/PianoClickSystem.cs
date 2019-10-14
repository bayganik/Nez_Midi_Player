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
        public override void Process(Entity entity)
        {
            //
            // ONLY piano key entities come here
            //
            MainGameScene = entity.Scene as MainScene;              //hand entity belongs to MainScene

            SpriteRenderer sp = entity.GetComponent<SpriteRenderer>();
            PlayingComponent pc = entity.GetComponent<PlayingComponent>();
            if (pc.ChannelMsg.Command == ChannelCommand.NoteOn)
            {
                sp.Color = Color.Blue;
            }
            else
            {
                sp.Color = Color.White;
            }
            entity.RemoveComponent<PlayingComponent>();

            if (PrevKey == null)
            {
                PrevKey = entity;
                return;
            }

            SpriteRenderer PrevSp = PrevKey.GetComponent<SpriteRenderer>();
            PrevSp.Color = Color.White;
            PrevKey = entity;

        }
    }
}
