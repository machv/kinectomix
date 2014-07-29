namespace Mach.Kinectomix.LevelEditor.ViewModel
{
    public partial class LevelViewModel
    {
        protected class BuildAsset
        {
            public string AssetName { get; set; }
            public BoardTileViewModel Template { get; set; }
            public bool RenderWithBonds { get; set; }

            public BuildAsset(string asset, BoardTileViewModel tile, bool renderWithBonds)
            {
                AssetName = asset;
                Template = tile;
                RenderWithBonds = renderWithBonds;
            }
        }
    }
}
