using Microsoft.Xna.Framework;
using Nez;
using Nez.UI;

namespace MidiPlayer.Scenes
{
    /// <summary>
    /// Set up a general purpose canvas and table to draw stuff with
    /// </summary>
    class BaseUI : Component
    {
        private int _renderLayer;
        protected Skin _skin;
        protected UICanvas ui;
        protected Table table;

        /// <summary>
        /// All we need when setting up is the renderlayer.  This assumes that you've already set up a ScreenSpaceRenderer and set its renderLayer.
        /// </summary>
        /// <param name="renderLayer">The renderLayer of your ScreenSpaceRenderer</param>
        /// <param name="skin">The skin to store in our class</param>
        public BaseUI(int renderLayer, Skin skin = null)
        {
            _renderLayer = renderLayer;

            // if we didn't pass in a skin, we'll just use a default one
            // we don't actually use this anywhere in the base class but it can be useful to store it here
            if (skin == null)
                _skin = Skin.CreateDefaultSkin();
        }

        /// <summary>
        /// Do our setup here.  We'll add our UICanvas and a table for starters.
        /// </summary>
        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();

            // add the canvas to our component
            ui = Entity.AddComponent(new UICanvas());
            ui.RenderLayer = _renderLayer;

            // add a new table layout
            table = ui.Stage.AddElement(new Table());

            // after our canvas is prepped, create our UI elements
            Setup();

            // when we're done setting up, position our elements for the first time
            PositionElements();

            // add events to listen for screen-size changing and scene changes
            Core.Emitter.AddObserver(CoreEvents.GraphicsDeviceReset, PositionElements);
            Core.Emitter.AddObserver(CoreEvents.SceneChanged, OnSceneChanged);
        }

        /// <summary>
        /// Sets the background color
        /// </summary>
        /// <param name="color">The color to make our background</param>
        protected void SetBackgroundColor(Color color)
        {
            if (table != null)
                table.SetBackground(new PrimitiveDrawable(color));
        }

        /// <summary>
        /// Override this method to add elements to your table property
        /// </summary>
        protected virtual void Setup()
        {

        }

        /// <summary>
        /// Override this method to resize your UI when the screen size changes and when we setup intially
        /// </summary>
        protected virtual void PositionElements()
        {

        }

        /// <summary>
        /// Override this method to do special stuff when the scene changes.  You probably don't need this.
        /// </summary>
        protected virtual void OnSceneChanged()
        {

        }
    }
}