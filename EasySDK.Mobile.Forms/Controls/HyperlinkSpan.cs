using System;
using System.Windows.Input;
using EasySDK.Mobile.ViewModels.Extensions;
using Xamarin.Forms;

namespace EasySDK.Mobile.Forms.Controls;

public class HyperlinkSpan : Span
{
    #region Properties

    #region Command

    public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(HyperlinkSpan));

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    #endregion

    #region CommandParameter

    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(HyperlinkSpan), null);

    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    #endregion

    #endregion

    #region ctor

    public HyperlinkSpan()
    {
        var tapGestureRecognizer = new TapGestureRecognizer();

        tapGestureRecognizer.Tapped += TapGestureRecognizerOnTapped;

        GestureRecognizers.Add(tapGestureRecognizer);
    }

    #endregion

    #region Private methods

    private void TapGestureRecognizerOnTapped(object sender, EventArgs e)
    {
        Command.TryExecute(CommandParameter);
    }

    #endregion
}