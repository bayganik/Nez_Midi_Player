using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nez;
namespace MidiPlayer.ECS.Components
{
    public class NoteComponent : Component
    {
        //
        // entity for a piano key
        //
        public bool IsOn = false;
        public int NoteID = 60;             //mid C is 60

        public NoteComponent()
        {

        }
        public override void onAddedToEntity()
        {
            base.onAddedToEntity();


        }
    }
}
