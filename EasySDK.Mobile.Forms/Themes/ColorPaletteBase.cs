using Xamarin.Forms;

namespace EasySDK.Mobile.Forms.Themes;

public abstract class ColorPaletteBase : ResourceDictionary
{
	protected void SetColor(string key, Color color)
	{
		Remove(key);

		Add(key, color);
	}

	protected void SetColor(string key, string hex)
	{
		SetColor(key, Color.FromHex(hex));
	}
}