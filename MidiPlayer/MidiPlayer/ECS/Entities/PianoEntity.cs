using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;

using MidiPlayer.ECS.Components;
namespace MidiPlayer.ECS.Entities
{
    public class PianoEntity
    {
        NezSpriteFont font;
        Entity TextEntity;
        const int White_Key_Layer = -7;
        const int Black_Key_Layer = -8;
        //
        // Piano
        //
        int whitekeywidth = 15;
        int blackkeywidth = 9;
        int MaxKeys = 85;

        int MinNote = 24;
        int MaxNote = 99;

        float xpos;
        public PianoEntity(Scene scene, Vector2 PianoPos, int Channel)
        {
            //font = new NezSpriteFont(scene.Content.Load<SpriteFont>("Arial"));

            //TextEntity = scene.CreateEntity("txt");
            //TextEntity.Transform.Position = new Vector2(PianoPos.X - 5, PianoPos.Y - 50);
            //TextEntity.Transform.Scale = new Vector2(1, 1);
            //var txt = new Text(Graphics.instance.bitmapFont, "Ch " + Channel.ToString(), new Vector2(0, 0), Color.White);
            //txt.SetFont(font);
            //TextEntity.AddComponent(txt);

            int note_offset = 0;
            int assigned_note = MinNote - 1;
            //
            // White keys
            //
            for (int i = 0; i < MaxKeys; i++)
            {
                int m = (i / 12);
                int n = i % 12;
                int key_offset = -1;
                //
                // white notes
                //
                switch (n)
                {
                    case 0:
                        key_offset = 0;
                        assigned_note = assigned_note + 1;
                        break;
                    case 2:
                        key_offset = 1;
                        assigned_note = assigned_note + 2;
                        break;
                    case 4:
                        key_offset = 2;
                        assigned_note = assigned_note + 2;
                        break;
                    case 5:
                        key_offset = 3;
                        assigned_note = assigned_note + 1;
                        break;
                    case 7:
                        key_offset = 4;
                        assigned_note = assigned_note + 2;
                        break;
                    case 9:
                        key_offset = 5;
                        assigned_note = assigned_note + 2;
                        break;
                    case 11:
                        key_offset = 6;
                        assigned_note = assigned_note + 2;
                        break;
                }
                if (key_offset >= 0)
                {
                    var pkey = scene.CreateEntity("pkey");
                    pkey.AddComponent(new SpriteRenderer(scene.Content.Load<Texture2D>("Piano/wkey_med")).SetRenderLayer(White_Key_Layer));
                    NoteComponent comp = new NoteComponent();
                    comp.IsOn = false;
                    comp.NoteID = assigned_note;

                    pkey.Tag = assigned_note;
                    pkey.Name = "pkey" + assigned_note.ToString();
                    pkey.AddComponent(comp);
                    //pkey.AddComponent(new BoxCollider());
                    xpos = PianoPos.X + (key_offset + m * 7) * whitekeywidth;
                    pkey.SetPosition(xpos, PianoPos.Y);
                }
            }
            //
            // black keys
            //
            assigned_note = MinNote;
            int offset = 0;
            for (int i = 0; i < MaxKeys; i++)
            {
                int m = (i / 12);
                int n = i % 12;
                int key_offset = -1;
                switch (n)
                {
                    case 1:
                        key_offset = 0;
                        assigned_note = assigned_note + 1 + offset;
                        break;
                    case 3:
                        key_offset = 1;
                        assigned_note = assigned_note + 2;
                        break;
                    case 6:
                        key_offset = 3;
                        assigned_note = assigned_note + 3;
                        break;
                    case 8:
                        key_offset = 4;
                        assigned_note = assigned_note + 2;
                        break;
                    case 10:
                        key_offset = 5;
                        assigned_note = assigned_note + 2;
                        offset = 2;
                        break;
                }
                if (key_offset >= 0)
                {
                    var pkey = scene.CreateEntity("pkey");
                    pkey.AddComponent(new SpriteRenderer(scene.Content.Load<Texture2D>("Piano/bkey_med")).SetRenderLayer(Black_Key_Layer));
                    NoteComponent comp = new NoteComponent();
                    comp.IsOn = false;
                    comp.NoteID = assigned_note;

                    pkey.Tag = assigned_note;
                    pkey.Name = "pkey" + assigned_note.ToString();
                    pkey.AddComponent(new BoxCollider());
                    xpos = (PianoPos.X + ((key_offset + m * 7) * whitekeywidth) + (whitekeywidth - blackkeywidth / 2)) - (blackkeywidth / 2);
                    pkey.SetPosition(xpos, PianoPos.Y - 14);

                }
            }

        }
    }
}
