using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sanford.Multimedia.Midi;

using Nez;
namespace MidiPlayer.ECS.Components
{
    public class PlayingComponent : Component
    {
        //
        // Entity for piano keys being pressed
        //
        public ChannelMessage ChannelMsg { get; set; }
        public PlayingComponent()
        {

        }
        public override void onAddedToEntity()
        {
            base.onAddedToEntity();


        }
    }
}
