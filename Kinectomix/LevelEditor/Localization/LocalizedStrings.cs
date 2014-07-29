using System.Globalization;

namespace Mach.Kinectomix.LevelEditor.Localization
{
    /// <summary>
    /// Provides access to localized texts.
    /// </summary>
    public class LocalizedStrings
    {
        private static EditorResources _editor = new EditorResources();
        private static TileResources _tile = new TileResources();
        private static DialogResources _dialog = new DialogResources();

        static LocalizedStrings()
        {
            EditorResources.Culture = CultureInfo.CurrentCulture; //.CurrentCulture;
            TileResources.Culture = CultureInfo.CurrentCulture; //.CurrentCulture;
            DialogResources.Culture = CultureInfo.CurrentCulture; //.CurrentCulture;
        }

        /// <summary>
        /// Gets the localized strings for editor.
        /// </summary>
        /// <value>
        /// The localized strings for editor.
        /// </value>
        public EditorResources Editor { get { return _editor; } }

        /// <summary>
        /// Gets the localized strings for tiles.
        /// </summary>
        /// <value>
        /// The localized strings for tiles.
        /// </value>
        public TileResources Tile { get { return _tile; } }

        /// <summary>
        /// Gets the localized strings for dialogs.
        /// </summary>
        /// <value>
        /// The localized strings for dialogs.
        /// </value>
        public DialogResources Dialog { get { return _dialog; } }
    }
}
