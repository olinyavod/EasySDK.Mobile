using System.Collections;
using System.Linq;
using EasySDK.Mobile.ViewModels.Effects;
using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels.Behaviors;

class DataFieldBehavior : DataFieldBehaviorBase
{
	#region ctpr

	public DataFieldBehavior(Element element) 
		: base(element)
	{
	}

	#endregion

	#region Proptected methods

	protected override void ErrorOnChanged(Element element, IEnumerable errors)
	{
		var errorMessage = errors
			.OfType<object>()
			.Select(i => i.ToString())
			.FirstOrDefault() ?? string.Empty;

		DataErrorEffect.SetError(element, errorMessage);
	}

	#endregion
}