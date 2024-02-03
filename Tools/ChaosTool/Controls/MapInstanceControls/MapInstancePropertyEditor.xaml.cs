using System.Windows;
using System.Windows.Controls;
using Chaos.Wpf.Observables;
using ChaosTool.Definitions;
using ChaosTool.Extensions;
using ChaosTool.ViewModel;

namespace ChaosTool.Controls.MapInstanceControls;

public sealed partial class MapInstancePropertyEditor
{
    private MapInstanceViewModel ViewModel
        => DataContext as MapInstanceViewModel
           ?? throw new InvalidOperationException($"DataContext is not of type {nameof(MapInstanceViewModel)}");

    public MapInstancePropertyEditor() => InitializeComponent();

    #region Tbox Validation
    private void TemplateKeyTbox_OnTextChanged(object sender, TextChangedEventArgs e)
        => Validators.TemplateKeyMatchesFileName(TemplateKeyTbox, PathTbox);
    #endregion

    private void UserControl_Initialized(object sender, EventArgs e)
    {
        //TODO: Tooltips
    }

    #region Buttons
    private void RevertBtn_Click(object sender, RoutedEventArgs e) => ViewModel.RejectChanges();

    private void SaveBtn_Click(object sender, RoutedEventArgs e) => ViewModel.AcceptChanges();

    private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var parentList = this.FindVisualParent<MapInstanceListView>();

        parentList?.Items.Remove(ViewModel);

        ViewModel.IsDeleted = true;
        ViewModel.AcceptChanges();
    }
    #endregion

    #region ScriptKeys Controls
    private void AddScriptKeyBtn_Click(object sender, RoutedEventArgs e)
    {
        //ScriptKeysViewItems.Add(string.Empty);
    }

    private void DeleteScriptKeyBtn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not BindableString scriptKey)
            return;

        //ScriptKeysViewItems.Remove(scriptKey);
    }
    #endregion
}