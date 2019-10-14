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
    public class PianoEntitySmallKey
    {
        NezSpriteFont font;
        Entity TextEntity;
        //
        // Piano
        //
        int whitekeywidth = 15;
        int blackkeywidth = 9;
        int MaxKeys = 85;

        int MinNote = 24;
        int MaxNote = 99;

        float xpos;
        public PianoEntitySmallKey(Scene scene, Vector2 PianoPos, int Channel)
        {
            //font = new NezSpriteFont(scene.Content.Load<SpriteFont>("Arial"));

            //TextEntity = scene.CreateEntity("");
            //TextEntity.Transform.Position = new Vector2(PianoPos.X - 5, PianoPos.Y - 50);
            //TextEntity.Transform.Scale = new Vector2(1, 1);
            //var  = new Text(Graphics.instance.bitmapFont, "Ch " + Channel.ToString(), new Vector2(0, 0), Color.White);
            //.SetFont(font);
            //TextEntity.AddComponent();

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
                    pkey.AddComponent(new SpriteRenderer(scene.Content.Load<Texture2D>("Piano/wkey")).SetRenderLayer(-7));
                    NoteComponent comp = new NoteComponent();
                    comp.IsOn = false;
                    comp.NoteID = assigned_note;

                    pkey.Tag = assigned_note;
                    pkey.Name = "pkey" + Channel.ToString() + assigned_note.ToString();
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
                    pkey.AddComponent(new SpriteRenderer(scene.Content.Load<Texture2D>("Piano/bkey")).SetRenderLayer(-8));
                    NoteComponent comp = new NoteComponent();
                    comp.IsOn = false;
                    comp.NoteID = assigned_note;
                    if (assigned_note == 63)
                    {
                        int junk = 0;
                    }
                    pkey.Tag = assigned_note;
                    pkey.Name = "pkey" + Channel.ToString() + assigned_note.ToString();
                    pkey.AddComponent(new BoxCollider());
                    xpos = (PianoPos.X + ((key_offset + m * 7) * whitekeywidth) + (whitekeywidth - blackkeywidth / 2)) - (blackkeywidth / 2);
                    pkey.SetPosition(xpos, PianoPos.Y - 10);

                }
            }

        }
    }
}
