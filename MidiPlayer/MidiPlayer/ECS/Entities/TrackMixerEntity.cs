using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Nez;
using Nez.UI;
using Nez.BitmapFonts;
using Nez.Tiled;
using Nez.Sprites;
using Nez.Textures;

using MidiPlayer.ECS.Components;
using MidiPlayer.ECS.Systems;
using MidiPlayer.ECS.Entities;
using MidiPlayer.Scenes;
namespace MidiPlayer.ECS.Entities
{

    public class TrackMixerEntity
    {
        Entity Background;
        float StartX = 10;
        float StartY = 50;

        MainScene MainGameScene;
        SubtextureDrawable selecton;
        SubtextureDrawable selectoff;
        SubtextureDrawable soloon;
        SubtextureDrawable solooff;
        SubtextureDrawable muteon;
        SubtextureDrawable muteoff;
        SubtextureDrawable slidertick;
        SubtextureDrawable sliderknob;
        SubtextureDrawable trkon;
        SubtextureDrawable trkoff;

        Slider slider;

        Nez.UI.CheckBox sel;
        Nez.UI.CheckBox solo;
        Nez.UI.CheckBox mute;
        Nez.UI.CheckBox trk;

        public TrackMixerEntity(Scene scene, Vector2 TracksPos, UICanvas canvas, bool[] trackbtn)
        {

            //
            // ONLY piano key entities come here
            //
            MainGameScene = scene as MainScene;              //hand entity belongs to MainScene
            //
            // Background for mixer
            //
            StartX = TracksPos.X;
            StartY = TracksPos.Y;
            Background = scene.createEntity("background", new Vector2(StartX, StartY));
            Background.tag = 90;
            Background.addComponent(new Sprite(scene.content.Load<Texture2D>("Slider/BackgroundMetal")).setOrigin(new Vector2(0, 0)).setRenderLayer(99));


            StartX = TracksPos.X + 10;
            StartY = TracksPos.Y + 10;
            //
            // bitmap font for checkbox
            //
            Nez.BitmapFonts.BitmapFont bf = scene.content.Load<BitmapFont>("fonts/futura");
            slidertick = new SubtextureDrawable((scene.content.Load<Texture2D>("Slider/SliderMetalSmall")));
            sliderknob = new SubtextureDrawable((scene.content.Load<Texture2D>("Slider/SliderKnobBlk")));

            selecton = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/SelOn")));
            selectoff = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/SelOff")));

            soloon = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/SoloOn")));
            solooff = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/SoloOff")));

            muteon = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/MuteOn")));
            muteoff = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/MuteOff")));
            //
            // Add all the buttons for each track used (I assume 16 maximum)
            //
            int trackNo = 0;
            foreach (bool trkOnOff in trackbtn)
            {
                //
                // track is being used (check mark) button is disabled
                //
                sel = canvas.stage.addElement(new Nez.UI.CheckBox("", new CheckBoxStyle(selectoff, selecton, bf, Color.Black)));
                sel.setPosition(StartX, StartY);
                sel.onClicked += MainGameScene.Track_Selection;
                sel.isChecked = trkOnOff;                           //on or off button?
                sel.setDisabled(!trkOnOff);                         //if !trkOnOff = true then track not in use
                sel.tag = trackNo;
                //
                // volume slider
                //
                slider = canvas.stage.addElement(new Slider(0, 100, 5, true, new SliderStyle(slidertick, sliderknob)));
                slider.setPosition(StartX, StartY + 30);
                slider.setHeight(150);

                slider.onChanged += MainGameScene.Vol_Changed;
                slider.setValue(100);
                slider.tag = trackNo;
                //
                // track SOLO
                //
                solo = canvas.stage.addElement(new Nez.UI.CheckBox("", new CheckBoxStyle(solooff, soloon, bf, Color.Black)));
                solo.setPosition(StartX, StartY + 180);
                solo.onClicked += MainGameScene.SoloBtn_onClicked;
                solo.isChecked = false;
                solo.tag = trackNo;
                MainGameScene.SoloBtnGroup[trackNo] = solo;         //add solo button to the group
                //
                // track MUTE
                //
                mute = canvas.stage.addElement(new Nez.UI.CheckBox("", new CheckBoxStyle(muteoff, muteon, bf, Color.Black)));
                mute.setPosition(StartX, StartY + 210);
                mute.onClicked += MainGameScene.Track_Selection;
                mute.isChecked = false;
                mute.tag = trackNo;
                //
                // On/Off button to give track instrument/channel number
                //
                switch (trackNo)
                {
                    case 0:
                        trkon = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk00On")));
                        trkoff = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk00Off")));
                        break;
                    case 1:
                        trkon = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk01On")));
                        trkoff = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk01Off")));
                        break;
                    case 2:
                        trkon = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk02On")));
                        trkoff = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk02Off")));
                        break;
                    case 3:
                        trkon = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk03On")));
                        trkoff = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk03Off")));
                        break;
                    case 4:
                        trkon = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk04On")));
                        trkoff = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk04Off")));
                        break;
                    case 5:
                        trkon = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk05On")));
                        trkoff = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk05Off")));
                        break;
                    case 6:
                        trkon = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk06On")));
                        trkoff = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk06Off")));
                        break;
                    case 7:
                        trkon = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk07On")));
                        trkoff = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk07Off")));
                        break;
                    case 8:
                        trkon = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk08On")));
                        trkoff = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk08Off")));
                        break;
                    case 9:
                        trkon = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk09On")));
                        trkoff = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk09Off")));
                        break;
                    case 10:
                        trkon = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk10On")));
                        trkoff = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk10Off")));
                        break;
                    case 11:
                        trkon = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk11On")));
                        trkoff = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk11Off")));
                        break;
                    case 12:
                        trkon = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk12On")));
                        trkoff = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk12Off")));
                        break;
                    case 13:
                        trkon = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk13On")));
                        trkoff = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk13Off")));
                        break;
                    case 14:
                        trkon = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk14On")));
                        trkoff = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk14Off")));
                        break;
                    case 15:
                        trkon = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk15On")));
                        trkoff = new SubtextureDrawable((scene.content.Load<Texture2D>("slider/Trk15Off")));
                        break;
                }
                trk = canvas.stage.addElement(new Nez.UI.CheckBox("", new CheckBoxStyle(trkoff, trkoff, bf, Color.Black)));
                trk.setPosition(StartX, StartY + 240);
                trk.onClicked += MainGameScene.TrackBtn_onClicked;
                trk.tag = trackNo;
                trk.isChecked = false;
                trk.setDisabled(!trkOnOff);                      //if !trkOnOff = true then track not in use

                StartX += 36.5f;
                trackNo += 1;
            }
        }
    }
}
