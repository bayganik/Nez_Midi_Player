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

namespace MidiPlayer.ECS.Systems
{
    public class KeyClickSystem : EntityProcessingSystem
    {
        /*
         * System for pressing of individual keys on keyboard
         * 1 - 7 determines the octave of the key of C
         * 
         * A = key of C             W = key of C#
         * S = key of D             E = key of D#
         * D = key of E
         * F = key of F
         * J = key of G             I = key of F#
         * K = key of A             O = key of G#
         * L = key of B             P = key of A#
         * 
         * ; = key of C    next octave
         */
        MouseState CurrentMouse;
        Vector2 MousePos;
        BoxCollider MouseCollider;

        MainScene MainGameScene;
        Entity KeyPressed;
        //
        // Our piano starts with 1st octave C = 24
        //
        int CurrentC = 60;                  //mid C octave
        public KeyClickSystem(Matcher matcher) : base(matcher)
        {

        }
        public override void Process(Entity entity)
        {
            //
            // ONLY keyboard entity comes here
            //
            MainGameScene = entity.Scene as MainScene;              //hand entity belongs to MainScene
            
            KeyboardState kbs = Input.CurrentKeyboardState;
            //
            // Change current octave value of key of C (1 - 7)
            //     where 4 is the middle C octave
            //
            bool octOnly = false;
            foreach(Keys kys in kbs.GetPressedKeys())
            {
                switch (kys)
                {
                    case Keys.D1:
                        CurrentC = (0 * 12) + 24;
                        octOnly = true;
                        break;
                    case Keys.D2:
                        CurrentC = (1 * 12) + 24;
                        octOnly = true;
                        break;
                    case Keys.D3:
                        CurrentC = (2 * 12) + 24; 
                        octOnly = true;
                        break;
                    case Keys.D4:                               //middle C octave
                        CurrentC = (3 * 12) + 24; 
                        octOnly = true;
                        break;
                    case Keys.D5:
                        CurrentC = (4 * 12) + 24; 
                        octOnly = true;
                        break;
                    case Keys.D6:
                        CurrentC = (5 * 12) + 24; 
                        octOnly = true;
                        break;
                    case Keys.D7:
                        CurrentC = (6 * 12) + 24; 
                        octOnly = true;
                        break;
                }
            }
            //
            // User hit 1 - 7 for octave change
            // Change key colors to yellow on screen
            //
            if (octOnly)
            {
                MainGameScene.HandleOctaveColor(CurrentC);
                return;
            }
            //
            // Handle each key
            // left hand ON (A, S, D, F  and W, E)
            //
            int velocity = 127;
            if (Input.IsKeyPressed(Keys.A))
                MainGameScene.HandleChannelMessageKeys(0, CurrentC, velocity, true);
            else if (Input.IsKeyPressed(Keys.S))
                MainGameScene.HandleChannelMessageKeys(0, CurrentC + 2, velocity, true);
            else if (Input.IsKeyPressed(Keys.D))
                MainGameScene.HandleChannelMessageKeys(0, CurrentC + 4, velocity, true);
            else if (Input.IsKeyPressed(Keys.F))
                MainGameScene.HandleChannelMessageKeys(0, CurrentC + 5, velocity, true);
            else if (Input.IsKeyPressed(Keys.W))
                MainGameScene.HandleChannelMessageKeys(0, CurrentC + 1, velocity, true);
            else if (Input.IsKeyPressed(Keys.E))
                MainGameScene.HandleChannelMessageKeys(0, CurrentC + 3, velocity, true);
            //
            // left hand OFF
            //
            if (Input.IsKeyReleased(Keys.A))
                MainGameScene.HandleChannelMessageKeys(0, CurrentC, velocity, false);
            else if (Input.IsKeyReleased(Keys.S))
                MainGameScene.HandleChannelMessageKeys(0, CurrentC + 2, velocity, false);
            else if (Input.IsKeyReleased(Keys.D))
                MainGameScene.HandleChannelMessageKeys(0, CurrentC + 4, velocity, false);
            else if (Input.IsKeyReleased(Keys.F))
                MainGameScene.HandleChannelMessageKeys(0, CurrentC + 5, velocity, false);
            else if (Input.IsKeyReleased(Keys.W))
                MainGameScene.HandleChannelMessageKeys(0, CurrentC + 1, velocity, false);
            else if (Input.IsKeyReleased(Keys.E))
                MainGameScene.HandleChannelMessageKeys(0, CurrentC + 3, velocity, false);

            //
            // right hand ON (J, K, L, ; and I, O, P)
            //
            if (Input.IsKeyPressed(Keys.J))
                MainGameScene.HandleChannelMessageKeys(0, CurrentC + 7, velocity, true);
            else if (Input.IsKeyPressed(Keys.K))
                MainGameScene.HandleChannelMessageKeys(0, CurrentC + 9, velocity, true);
            else if (Input.IsKeyPressed(Keys.L))
                MainGameScene.HandleChannelMessageKeys(0, CurrentC + 11, velocity, true);
            else if (Input.IsKeyPressed(Keys.OemSemicolon))
                MainGameScene.HandleChannelMessageKeys(0, CurrentC + 12, velocity, true);
            else if (Input.IsKeyPressed(Keys.I))
                MainGameScene.HandleChannelMessageKeys(0, CurrentC + 6, velocity, true);
            else if (Input.IsKeyPressed(Keys.O))
                MainGameScene.HandleChannelMessageKeys(0, CurrentC + 8, velocity, true);
            else if (Input.IsKeyPressed(Keys.P))
                MainGameScene.HandleChannelMessageKeys(0, CurrentC + 10, velocity, true);
            //
            // right hand OFF
            //
            if (Input.IsKeyReleased(Keys.J))
                MainGameScene.HandleChannelMessageKeys(0, CurrentC + 7, velocity, false);
            else if (Input.IsKeyReleased(Keys.K))
                MainGameScene.HandleChannelMessageKeys(0, CurrentC + 9, velocity, false);
            else if (Input.IsKeyReleased(Keys.L))
                MainGameScene.HandleChannelMessageKeys(0, CurrentC + 11, velocity, false);
            else if (Input.IsKeyReleased(Keys.OemSemicolon))
                MainGameScene.HandleChannelMessageKeys(0, CurrentC + 12, velocity, false);
            else if (Input.IsKeyReleased(Keys.I))
                MainGameScene.HandleChannelMessageKeys(0, CurrentC + 6, velocity, false);
            else if (Input.IsKeyReleased(Keys.O))
                MainGameScene.HandleChannelMessageKeys(0, CurrentC + 8, velocity, false);
            else if (Input.IsKeyReleased(Keys.P))
                MainGameScene.HandleChannelMessageKeys(0, CurrentC + 10, velocity, false);
        }
    }
}
