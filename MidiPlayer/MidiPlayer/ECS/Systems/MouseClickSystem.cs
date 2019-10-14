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
    public class MouseClickSystem : EntityProcessingSystem
    {
        /*
         * System to manage the mouse click
         * typically used for pressing of piano keys
         */
        MouseState CurrentMouse;
        Vector2 MousePos;
        BoxCollider MouseCollider;

        MainScene MainGameScene;
        Entity KeyPressed;

        public MouseClickSystem(Matcher matcher) : base(matcher)
        {

        }
        public override void Process(Entity entity)
        {
            //
            // ONLY MOUSE entity comes here
            //
            MainGameScene = entity.Scene as MainScene;              //hand entity belongs to MainScene
            CurrentMouse = Mouse.GetState();
            MousePos = new Vector2(CurrentMouse.Position.X, CurrentMouse.Position.Y);
            entity.Transform.Position = Scene.Camera.ScreenToWorldPoint(new Vector2(CurrentMouse.Position.X, CurrentMouse.Position.Y));

            MouseCollider = entity.GetComponent<BoxCollider>();     //did we click on a BoxCollider
            if (Input.LeftMouseButtonPressed)
            {
                if (MouseCollider.CollidesWithAny(out CollisionResult collisionResult))
                {
                    //KeyPressed = collisionResult.collider.entity;
                    //Sprite sp = KeyPressed.GetComponent<SpriteRenderer>();
                    //sp.Color = Color.Blue;
                }
            }
            if (Input.LeftMouseButtonReleased)
            {
                
                //Sprite sp = KeyPressed.GetComponent<SpriteRenderer>();
                //sp.Color = Color.White;
            }

        }
    }
}
