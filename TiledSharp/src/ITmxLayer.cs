namespace TiledSharp
{
	public interface ITmxLayer: ITmxElement
	{
		double? OffsetX { get; }
		double? OffsetY { get; }
		double Opacity { get; }
		PropertyDict Properties { get; }
		bool Visible { get; }
	}
}