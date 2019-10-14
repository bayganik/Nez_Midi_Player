using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nez;
namespace MidiPlayer.ECS.Components
{
    public class KeyComponent : Component
    {
        //
        // Entity for Keyboard keys being pressed
        //
        public KeyComponent()
        {

        }
        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();


        }
    }
}
