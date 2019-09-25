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
        public override void process(Entity entity)
        {
            //
            // ONLY MOUSE entity comes here
            //
            MainGameScene = entity.scene as MainScene;              //hand entity belongs to MainScene
            CurrentMouse = Mouse.GetState();
            MousePos = new Vector2(CurrentMouse.Position.X, CurrentMouse.Position.Y);
            entity.transform.position = scene.camera.screenToWorldPoint(new Vector2(CurrentMouse.Position.X, CurrentMouse.Position.Y));

            MouseCollider = entity.getComponent<BoxCollider>();     //did we click on a BoxCollider
            if (Input.leftMouseButtonPressed)
            {
                if (MouseCollider.collidesWithAny(out CollisionResult collisionResult))
                {
                    //KeyPressed = collisionResult.collider.entity;
                    //Sprite sp = KeyPressed.getComponent<Sprite>();
                    //sp.color = Color.Blue;
                }
            }
            if (Input.leftMouseButtonReleased)
            {
                
                //Sprite sp = KeyPressed.getComponent<Sprite>();
                //sp.color = Color.White;
            }

        }
    }
}
