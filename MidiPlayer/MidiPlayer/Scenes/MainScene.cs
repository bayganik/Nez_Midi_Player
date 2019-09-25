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

using Sanford.Multimedia.Midi;

namespace MidiPlayer.Scenes
{
    /*
     * MIDI file has many tracks
     * MIDI file has 16 channels
     * Each track plays one instrument over a single channel
     * Each channel has a Program Message (instrument)
     * Channel 9 is used for drums
     * It is also possible to have more than one track using the same MIDI channel. 
     *   For example, say you want to record a piano part for your composition. 
     *   You may decide to have the right hand part on one track and 
     *   the left hand on another.
     * 
     * This program ASSUMES each track has ONE channel
     * 
     *private void UpdateMidiTimes()
        {
            _totalTicks = sequence1.GetLength();
            _tempo = sequence1.Tempo;
            TempoOrig = _tempo;
            _ppqn = sequence1.Division;
            _duration = _tempo * (_totalTicks / _ppqn) / 1000000; //seconds
            _bpm = GetBPM(_tempo);

            if (sequence1.Time != null)
                _measurelen = sequence1.Time.Measure;
            
            int Min = (int)(_duration / 60);
            int Sec = (int)(_duration - (Min * 60));

            string tx;
            tx = string.Format("Division: {0}", _ppqn) + "\n";
            tx += string.Format("Tempo: {0}", _tempo) + "\n";
            tx += string.Format("BPM: {0}", _bpm) + "\n";
            tx += string.Format("TotalTicks: {0}", _totalTicks) + "\n";
            tx += "Duration: " + string.Format("{0:00}:{1:00}", Min, Sec) + "\n";

            if (sequence1.Format != sequence1.OrigFormat)
                tx += "Midi Format: " + sequence1.Format.ToString() + " (Orig. Format: " + sequence1.OrigFormat.ToString() + ")";
            else
                tx += "Midi Format: " + sequence1.Format.ToString();
        }
     */
    public class MainScene : BaseScene
    {
        //
        // Sanford midi variables
        //
        Sequence sequence1;
        Sequencer sequencer1;
        OutputDevice outDevice;
        public event System.EventHandler Completed;
        public const int SCREEN_SPACE_RENDER_LAYER = 999;

        int outDeviceID = 0;
        //
        // Piano
        //
        int whitekeywidth = 15;
        int blackkeywidth = 9;
        int MaxKeys = 85;

        int MinNote = 24;
        int MaxNote = 99;
        Vector2 PianoPos = new Vector2(20, 300);
        Vector2 Size = new Vector2(600, 339);
        const int Black_Key_Layer = -8;

        //
        // Nez
        //
        SceneResolutionPolicy policy;
        NezSpriteFont font;
        Entity TextEntity;
        //
        // Midi player buttons
        //
        int StartPos = 50;
        Nez.UI.TextButton StepBtn { get; set; }
        Nez.UI.TextButton PBtn { get; set; }
        Nez.UI.Label TrackMsg { get; set; }
        Nez.UI.Label ChnlMsg {get;set;}
        Nez.UI.Label NoteMsg { get; set; }
        Nez.UI.Label InstrumentMsg { get; set; }
        //
        // Channel buttons
        //
        int AllChannels = -1;

        bool[] TracksInMidi = new bool[16];
        bool[] TracksPlaying = new bool[16];
        bool[] ChannelsPlaying = new bool[16];
        string[] TrackName = new string[16];
        string[] TrackInstrument = new string[16];

        //Nez.UI.Label[] TrackName = new Nez.UI.Label[16];
        //Nez.UI.Label[] TrackInstrument = new Nez.UI.Label[16];
        int[] TrackChanl = new int[16];
        //
        // player back ground 
        //
        Entity Background;
        //
        // Canvas
        //
        UICanvas canvas;
        Nez.UI.ProgressBar PgBar;
        //
        // Solo button
        //
        bool SoloPushedOn = false;
        public Nez.UI.CheckBox[] SoloBtnGroup = new Nez.UI.CheckBox[16];
        
        //
        // Player buttons
        //
        Nez.UI.ImageButton PlayBtn;
        SubtextureDrawable PlayImageUp;
        SubtextureDrawable PlayImageDn;

        Nez.UI.ImageButton StopBtn;
        SubtextureDrawable StopImageUp;
        SubtextureDrawable StopImageDn;

        Nez.UI.ImageButton LoadBtn;
        SubtextureDrawable LoadImageUp;
        SubtextureDrawable LoadImageDn;
        //
        // Mouse var, so we can track what it clicks on
        //
        Entity MouseCursor;
        Entity PrevKey;

        int PrevNote;

        Nez.Text txt;
        //znznznznznznznznznznznzn
        //      MainScene 
        //znznznznznznznznznznznzn
        public MainScene()
        {
            policy = Scene.SceneResolutionPolicy.ExactFit;
        }
        public override void initialize()
        {
            base.initialize();
            font = new NezSpriteFont(content.Load<SpriteFont>("Arial"));
            //
            // MIDI sequencer must have processes
            //
            sequencer1 = new Sanford.Multimedia.Midi.Sequencer();
            sequencer1.clock.Tick += onTick;
            this.sequencer1.PlayingCompleted        += new System.EventHandler(this.HandlePlayingCompleted);
            this.sequencer1.ChannelMessagePlayed    += new System.EventHandler<Sanford.Multimedia.Midi.ChannelMessageEventArgs>(this.HandleChannelMessagePlayed);
            this.sequencer1.SysExMessagePlayed      += new System.EventHandler<Sanford.Multimedia.Midi.SysExMessageEventArgs>(this.HandleSysExMessagePlayed);
            this.sequencer1.Chased                  += new System.EventHandler<Sanford.Multimedia.Midi.ChasedEventArgs>(this.HandleChased);
            this.sequencer1.Stopped                 += new System.EventHandler<Sanford.Multimedia.Midi.StoppedEventArgs>(this.HandleStopped);
            //
            // Default output device (usually 0)
            //
            outDevice = new OutputDevice(outDeviceID);

            //DialogBar db = new DialogBar(-1);

            //znznznznznznznznznznznznznznznznznznznznznznznznznznznznzn
            // mouse entity (used for tracking of clicks)
            //znznznznznznznznznznznznznznznznznznznznznznznznznznznznzn
            MouseCursor = createEntity("mouse");
            MouseCursor.addComponent(new BoxCollider());
            MouseCursor.addComponent(new MouseComponent());

            //znznznznznznznznznznznznznznznznznznznznznznznznznznznznzn
            // keyboard entity (playing piano manually)
            //znznznznznznznznznznznznznznznznznznznznznznznznznznznznzn
            MouseCursor = createEntity("keyboard");
            MouseCursor.addComponent(new KeyComponent());

            //znznznznznznznznznznznznznznznznznznznznznznznznznznznznzn
            // Text entity with component (Game name label)
            //znznznznznznznznznznznznznznznznznznznznznznznznznznznznzn
            TextEntity = createEntity("txt");
            TextEntity.transform.position = new Vector2(10, 20);
            TextEntity.transform.scale = new Vector2(1, 1);
            txt = new Text(Graphics.instance.bitmapFont, "MIDI Player", new Vector2(0, 0), Color.White);
            txt.setFont(font);
            TextEntity.addComponent(txt);

            //znznznznznznznznznznznznznznznznznznznznznznznznznznznznzn
            // Background, with high value render layer 
            //znznznznznznznznznznznznznznznznznznznznznznznznznznznznzn
            //Background = createEntity("background", new Vector2(0,0));
            //Background.tag = 90;
            //Background.addComponent(new Sprite(content.Load<Texture2D>("Background")).setOrigin(new Vector2(0,0)).setRenderLayer(99));

            //znznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznzn
            // Canvas 
            //znznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznzn
            //
            int StartX = 10;
            Entity uiCan = createEntity("ui");
            canvas = uiCan.addComponent(new UICanvas());
            canvas.isFullScreen = true;
            canvas.renderLayer = -1;//= SCREEN_SPACE_RENDER_LAYER;
            //
            // Step thru each note
            //
            //StepBtn = canvas.stage.addElement(new TextButton("Step!", Skin.createDefaultSkin()));
            //StepBtn.setPosition(StartX + 560, StartPos);
            //StepBtn.setSize(60f, 20f);
            //StepBtn.onClicked += StepBtn_onClicked;
            //
            // Display Piano 
            //
            //var pi00 = new PianoEntity(this, new Vector2(StartX + 200, StartPos), 0);
            //var pi00 = new PianoEntity(this, new Vector2(20, 250 + (80 * 7)), 5);
            //
            // Channel Massage Label
            //
            //CInst00 = canvas.stage.addElement(new Nez.UI.Label("All Instruments"));
            //CInst00.setPosition(PianoPos.X, PianoPos.Y - 90);
            //CInst00.setSize(100f, 50f);
            //
            // Display for track number
            //
            TrackMsg = canvas.stage.addElement(new Nez.UI.Label("All Tracks"));
            TrackMsg.setPosition(320, 310);
            TrackMsg.setSize(100f, 50f);
            //
            // Display for channel number
            //
            ChnlMsg = canvas.stage.addElement(new Nez.UI.Label("All Channels"));
            ChnlMsg.setPosition(320, 330);
            ChnlMsg.setSize(100f, 50f);
            AllChannels = -1;
            //
            // Display for instrument name
            //
            InstrumentMsg = canvas.stage.addElement(new Nez.UI.Label("All Channels"));
            InstrumentMsg.setPosition(320, 350);
            InstrumentMsg.setSize(100f, 50f);
            //
            // Dispaly for note number being played
            //
            NoteMsg = canvas.stage.addElement(new Nez.UI.Label("Piano note played"));
            NoteMsg.setPosition(320, 370);
            NoteMsg.setSize(100f, 50f);

            //
            // Track buttons and channels
            //
            for (int i=0; i < 16; i++)
            {
                TrackChanl[i] = -1;
                TracksInMidi[i] = false;                //is track number active in MIDI file
                TracksPlaying[i] = false;               //individual tracks playing (buttons)
                ChannelsPlaying[i] = true;              //they all are playing

            }

            //
            // turn off all buttons
            //
            //SetTrackButtonOff();
            //
            // Progress bar 
            //
            PgBar = canvas.stage.addElement(new Nez.UI.ProgressBar(0, 1000, 1, false, Nez.UI.ProgressBarStyle.create(Color.Green, Color.White)));
            PgBar.setPosition(10, StartPos);
            //
            // Play button
            //
            PlayImageUp = new SubtextureDrawable((content.Load<Texture2D>("Player/play_blk")));
            PlayImageDn = new SubtextureDrawable((content.Load<Texture2D>("Player/play_grn")));
            PlayBtn = canvas.stage.addElement(new ImageButton(PlayImageUp, PlayImageDn));
            PlayBtn.setPosition(StartX, StartPos + 30);
            PlayBtn.onClicked += Play;

            //
            // Stop button
            //
            StopImageUp = new SubtextureDrawable((content.Load<Texture2D>("Player/stop_blk")));
            StopImageDn = new SubtextureDrawable((content.Load<Texture2D>("Player/stop_grn")));
            StopBtn = canvas.stage.addElement(new ImageButton(StopImageUp, StopImageDn));
            StopBtn.setPosition(StartX + 71, StartPos + 30);
            StopBtn.onClicked += Stop;

            //
            // Load/Eject button (stop playing, look for another MIDI file)
            //
            LoadImageUp = new SubtextureDrawable((content.Load<Texture2D>("Player/eject_blk")));
            LoadImageDn = new SubtextureDrawable((content.Load<Texture2D>("Player/eject_grn")));
            LoadBtn = canvas.stage.addElement(new ImageButton(LoadImageUp, LoadImageDn));
            LoadBtn.setPosition(StartX + 110, StartPos + 30);
            LoadBtn.onClicked += Load;
            //
            //znznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznzn
            // Systems to process our requests
            //znznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznzn
            //
            this.addEntityProcessor(new MouseClickSystem(new Matcher().all(typeof(MouseComponent))));
            this.addEntityProcessor(new KeyClickSystem(new Matcher().all(typeof(KeyComponent))));
            this.addEntityProcessor(new PianoClickSystem(new Matcher().all(typeof(NoteComponent), typeof(PlayingComponent))));
            //
            // Start by loading a file
            //
            if (!LoadMidiFile())
                return;
            //
            // Create the Mixer image with buttons, sliders, etc
            //
            var Track = new TrackMixerEntity(this, new Vector2(310 , 20), canvas, TracksInMidi);
            //
            // Create the piano keys on bottom of mixer
            //
            var pi00 = new PianoEntity(this, new Vector2(StartX + 245, StartPos + 420), 0);
            //
            // Octave 4 is the middle C (note 60), color the keys
            //
            HandleOctaveColor(60);

        }
        public void Track_Selection(Nez.UI.Button btn)
        {
            //
            // Track selection check mark (on top of slider)
            // Mute also comes here for each track
            //
            if (TrackChanl[btn.tag] < 0)
            {
                return;
            }


            TracksPlaying[btn.tag] = !TracksPlaying[btn.tag];
            ChannelsPlaying[TrackChanl[btn.tag]] = !ChannelsPlaying[TrackChanl[btn.tag]];

            AllChannels = 0;
            sequencer1.Stop();
            sequencer1.Continue();
        }
        public void SoloBtn_onClicked(Nez.UI.Button btn)
        {
            SoloPushedOn = btn.isChecked;           //toggle on/off
            //
            // Solo comes here for each track
            //
            if (TrackChanl[btn.tag] < 0)
            {
                return;
            }
            if (SoloPushedOn)
            {
                //
                // all tracks are turned off except this one
                //
                for (int i = 0; i < 16; i++ )
                {
                    if (TracksInMidi[i])            //valid track
                    {
                        //
                        // turn off all tracks 
                        //
                        if (TrackChanl[i] >= 0)
                        {
                            TracksPlaying[i] = false;    
                            ChannelsPlaying[TrackChanl[i]] = false;
                        }
                        Nez.UI.CheckBox tmpCB = SoloBtnGroup[i];
                        tmpCB.isChecked = false;
                    }
                }
                //
                // turn on this track (btn)
                //
                btn.isChecked = true;
            }
            else
            {
                //
                // all tracks are back on
                //
                for (int i = 0; i < 16; i++)
                {
                    if (TracksInMidi[i])            //valid track
                    {
                        //
                        // turn on all tracks 
                        //
                        if (TrackChanl[i] >= 0)
                        {
                            TracksPlaying[i] = true;
                            ChannelsPlaying[TrackChanl[i]] = true;
                        }
                    }
                }
                //
                // turn off this track (btn)
                //
                btn.isChecked = false;
            }
            TracksPlaying[btn.tag] = true;
            ChannelsPlaying[TrackChanl[btn.tag]] = true;
            //
            // give info about the track
            //
            TrackBtn_onClicked(btn);

            AllChannels = 0;
            sequencer1.Stop();
            sequencer1.Continue();
        }
        public void TrackBtn_onClicked(Nez.UI.Button btn)
        {
            //
            // Track number button (on bottom of slider)
            //
            if (TrackChanl[btn.tag] < 0)
                ChnlMsg.setText("Channel:  ");
            else
                ChnlMsg.setText("Channel: " + TrackChanl[btn.tag].ToString("00"));
            //
            // Give channel number/instrument name
            //
            TrackMsg.setText("Track: " + btn.tag.ToString("00")); 
            InstrumentMsg.setText("Description: " + TrackInstrument[btn.tag]);
        }
        public void Vol_Changed(float slider)
        {
            //
            // Slider zero position is on TOP, we want it on BOTTOM
            // so the max value is subtracted from every volume change
            // to show the reverse of the slider movement
            // 
            float v = slider;
            float reverse = 100 - v;
            
        }


        private void PBtn_onClicked(Nez.UI.Button btn)
        {
            AllChannels = -1;
            ChnlMsg.setText("All Channels");
            
        }
        private void StepBtn_onClicked(Nez.UI.Button btn)
        {
            sequencer1.Continue();

            //if (!sequencer1.clock.IsRunning)
            //{
            //    sequencer1.clock.Start();
            //}
        }
        public bool LoadMidiFile()
        {
            //znznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznzn
            // Get a MIDI file to load 
            //znznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznznzn
            //
            string filePath = @"C:\temp\cest_lavie.mid";
            //OpenFileDialog sfd = new OpenFileDialog();
            //sfd.InitialDirectory = @"C:\";
            //sfd.Filter = "All files (*.mid)|*.mid";
            //sfd.Multiselect = false;
            //DialogResult result = sfd.ShowDialog();
            //if (result == DialogResult.OK)
            //{
            //    filePath = sfd.FileName;

            //    txt.setText(Path.GetFileNameWithoutExtension(filePath));
            //}
            //else
            //{
            //    txt.setText("No MIDI Files.");
            //    return;
            //}
            //znznznznznznznznznznznznznznznznznznznznznznznznznznznznzn
            // Get sequencer ready for a file
            //znznznznznznznznznznznznznznznznznznznznznznznznznznznznzn
            sequence1 = new Sanford.Multimedia.Midi.Sequence(filePath);
            //
            // Play all channels
            //
            AllChannels = -1;                                   //all channels flag
            SetTrackButtonOn();                                 //disp active channels

            sequencer1.Sequence = sequence1;
            PgBar.setMinMax(0, sequence1.GetLength());          //progress bar min/max

            //sequencer1.Start();
            return true;
        }
        public void ChangeChannelPan(int Channel, int Pan)
        {
            ChannelMessageBuilder builder = new ChannelMessageBuilder()
            {
                Command = ChannelCommand.Controller,
                MidiChannel = Channel,
                Data1 = (int)ControllerType.Pan,
                Data2 = Pan,
            };
            builder.Build();
            outDevice.Send(builder.Result);
        }
        public void ChangeChannelProgram(int Channel, int Instrument)
        {
            ChannelMessageBuilder builder = new ChannelMessageBuilder()
            {
                Command = ChannelCommand.ProgramChange,
                MidiChannel = Channel,
                Data1 = Instrument,
                Data2 = 0,
            };
            builder.Build();
            outDevice.Send(builder.Result);
        }
        public void ChangeChannelVol(int Channel, int Vol)
        {
            ChannelMessageBuilder builder = new ChannelMessageBuilder()
            {
                Command = ChannelCommand.Controller,
                MidiChannel = Channel,
                Data1 = (int)ControllerType.Volume,
                Data2 = Vol,
            };
            builder.Build();
            outDevice.Send(builder.Result);

            //if (v > 127)
            //    v = 127;
            //SendCC(nChannel, c, v);
            //
            // Volume Zero mutes the channel (remember what volume track was)
            //
            //Track track = sequence1.tracks[pTrack.Track];
            //track.RemoveVolume();
            //track.insertVolume(nChannel, v);
        }
        private void SetTrackButtonOn()
        {
            int x = 0;          //index of the control

            Vector2 StartPos = new Vector2(20, 250 + (80 * x));
            ChnlMsg.setText("All Tracks");
            //InstrumentMsg.setText("All Instruments");
            int trknum = 0;
            string trkname = "";
            GeneralMidiInstrument gmi;

            foreach (Track trk in sequence1.tracks)
            {
                float PosX = 14f;
                float PosY = 0;
                
                trkname = trk.Name;
                if (trk.Name == "" || trk.Name is null)
                    trkname = "Track " + trknum.ToString();
                if (trk.ProgramChange != 0)
                {
                    gmi = (GeneralMidiInstrument)trk.ProgramChange;
                    TrackInstrument[trknum] = gmi.ToString();
                    TrackChanl[trknum] = trk.MidiChannel;
                }
                else
                {
                    TrackInstrument[trknum] = "n/a";
                }

                switch (trknum)
                {
                    case 0:
                        TracksInMidi[trknum] = true;
                        x = 0;
                        break;
                    case 1:
                        TracksInMidi[trknum] = true;
                        x = 1;
                        break;
                    case 2:
                        TracksInMidi[trknum] = true;
                        x = 2;
                        break;
                    case 3:
                        TracksInMidi[trknum] = true;
                        x = 3;
                        break;
                    case 4:
                        TracksInMidi[trknum] = true;
                        x = 4;
                        break;
                    case 5:
                        TracksInMidi[trknum] = true;
                        x = 5;
                        break;
                    case 6:
                        TracksInMidi[trknum] = true;
                        x = 6;
                        break;
                    case 7:
                        TracksInMidi[trknum] = true;
                        x = 7;
                        break;
                    case 8:
                        TracksInMidi[trknum] = true;
                        x = 0;
                        PosX = 794;
                        break;
                    case 9:
                        TracksInMidi[trknum] = true;
                        x = 1;
                        PosX = 794;
                        break;
                    case 10:
                        TracksInMidi[trknum] = true;
                        x = 2;
                        PosX = 794;
                        break;
                    case 11:
                        TracksInMidi[trknum] = true;
                        x = 3;
                        PosX = 794;
                        break;
                    case 12:
                        TracksInMidi[trknum] = true;
                        x = 4;
                        PosX = 794;
                        break;
                    case 13:
                        TracksInMidi[trknum] = true;
                        x = 5;
                        PosX = 794;
                        break;
                    case 14:
                        TracksInMidi[trknum] = true;
                        x = 6;
                        PosX = 794;
                        break;
                    case 15:
                        TracksInMidi[trknum] = true;
                        x = 7;
                        PosX = 794;
                        break;
                }

                PosY = (200 + (80 * x));

                trknum += 1;
            }
        }
        private void onTick(object sender, EventArgs args)
        {
            MidiInternalClock obj = (MidiInternalClock)sender;
            float seqPos = (float)sequencer1.Position;
            PgBar.setValue(seqPos);

        }
        private void HandleChannelMessagePlayed(object sender, ChannelMessageEventArgs e)
        {
            //
            // All Channel Message during play 
            //
            int CurrentNote = e.Message.Data1;                     //note to play
            int velocity = e.Message.Data2;                 //velocity
            int chnl = e.Message.MidiChannel;               //channel num

            //if (e.Message.Command == ChannelCommand.ProgramChange)
            //{
            //    int inst = e.Message.Data1;
            //    GeneralMidiInstrument gmi = (GeneralMidiInstrument)inst;
            //    string instrument = gmi.ToString();
            //    TrackInstrument[chnl].setText(instrument);

            //}

            //
            // All channels (-1) 
            //
            if (AllChannels < 0)
            {
                outDevice.Send(e.Message);              //all messages
                return;
            }
            //else if (AllChannels >= 0 && chnl == AllChannels)     //specific channel messages
            //
            // one or more specific channels
            //
            if (ChannelsPlaying[chnl])                      //if channel is playing
            {
                if (e.Message.Command == ChannelCommand.NoteOn)
                {
                    //if (CurrentNote < MinNote || CurrentNote > MaxNote)
                    //{
                    //    throw new System.InvalidOperationException("Note is out of range.");
                    //}

                    //HandleChannelMessageKeys(chnl, PrevNote, velocity, false);
                    //PrevNote = CurrentNote;
                    //HandleChannelMessageKeys(chnl, CurrentNote, velocity, true);

                    outDevice.Send(e.Message);

                }
                //else if (e.Message.Command == ChannelCommand.NoteOff)
                //        HandleChannelMessageKeys(chnl, CurrentNote, 0,  false);
                //    else
                else
                        outDevice.Send(e.Message);                          //only on this channel msg goes out

            }

        }
        public void HandleOctaveColor(int NoteID)
        {
            Color clr = Color.White;
            Entity CurrentKey;                  //the key entity
            Sprite sp;                          //the key sprite
            //
            // reset all keys to white
            //
            int MinNote = 24;
            int MaxNote = 99;
            for (int i=MinNote; i <= MaxNote; i++)
            {          
                CurrentKey = this.findEntity("pkey" + i.ToString());
                if (CurrentKey == null)
                    continue;

                sp = CurrentKey.getComponent<Sprite>();
                sp.color = Color.White;                                         //current key turned white
            }
            //
            // turn the octave keys to yellow starting at NoteID
            //
            MinNote = NoteID;
            MaxNote = MinNote + 12;
            for (int i = MinNote; i <= MaxNote; i++)
            {
                CurrentKey = this.findEntity("pkey" + i.ToString());
                if (CurrentKey == null)
                    continue;

                sp = CurrentKey.getComponent<Sprite>();
                sp.color = Color.Yellow;                                         //current key turned white
            }

        }
        public void HandleChannelMessageKeys(int chnl, int NoteID, int Velocity, bool NoteOn = true)
        {
            //
            // Keyboard Key Pressed Channel Messages
            //
            if (NoteID < MinNote || NoteID > MaxNote)
                return;

            NoteMsg.setText(NoteID.ToString());                     //display note value
            Entity CurrentKey;                                      //get the key entity
            CurrentKey = this.findEntity("pkey"  + NoteID.ToString());
            if (CurrentKey == null)
                return;

            Sprite sp = CurrentKey.getComponent<Sprite>();
            //sp.renderLayer = -8;                    //same render layer for black or white key
            if (NoteOn)
            {
                outDevice.Send(new ChannelMessage(ChannelCommand.NoteOn, 0, NoteID, Velocity));
                sp.color = Color.Blue;                                          //current key turned blue
            }
            else
            {
                outDevice.Send(new ChannelMessage(ChannelCommand.NoteOff, 0, NoteID, Velocity));
                sp.color = Color.Yellow;                                          //current key turned white
            }
        }
        private void HandleChased(object sender, ChasedEventArgs e)
        {
            foreach (ChannelMessage message in e.Messages)
            {
                outDevice.Send(message);
            }
        }

        private void HandleSysExMessagePlayed(object sender, SysExMessageEventArgs e)
        {
            //     outDevice.Send(e.Message); Sometimes causes an exception to be thrown because the output device is overloaded.
        }

        private void HandleStopped(object sender, StoppedEventArgs e)
        {
            foreach (ChannelMessage message in e.Messages)
            {
                outDevice.Send(message);
                //pianoControl1.Send(message);
            }
        }

        private void HandlePlayingCompleted(object sender, EventArgs e)
        {
            if (Completed != null)
                Completed(this, null); //timer1.Stop();
        }
        public void Play(Nez.UI.Button btn)
        {
            //sequencer1.Start();
            sequencer1.Continue();

            if (!sequencer1.clock.IsRunning)
            {
                sequencer1.clock.Start();
            }
        }
        public void Continue()
        {
            //sequencer1.Start();
            sequencer1.Continue();

            if (!sequencer1.clock.IsRunning)
            {
                sequencer1.clock.Start();
            }
        }
        public void Stop(Nez.UI.Button btn)
        {
            sequencer1.Stop();
            sequencer1.clock.Stop();
        }
        public void Load(Nez.UI.Button btn)
        {
            sequencer1.Stop();
            sequencer1.clock.Stop();

            LoadMidiFile();
        }
        public bool IsRunning { get { return sequencer1.clock.IsRunning; } }
    }
}
