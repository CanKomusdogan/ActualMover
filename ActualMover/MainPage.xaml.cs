using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Storage;
using ActualMover.Shared;
using ActualMover.Models;

// ReSharper disable PossibleLossOfFraction

namespace ActualMover;

public partial class MainPage : ContentPage
{
    private readonly CancellationToken _emptyCt = CancellationToken.None;

    private readonly IFolderPicker folderPicker;
    private Folder? _srcFolder;
    private Folder? _dstFolder;
    private bool _overwrite = false;
    private bool _copyNotMove = false;
    private ObservableCollection<ExtensionItem> _extensions = [];

    public MainPage(IFolderPicker folderPicker)
    {
        InitializeComponent();
        ExtList.ItemsSource = _extensions;
        this.folderPicker = folderPicker;
    }

    private async Task<Folder?> PickFolder(CancellationToken ct)
    {
        var result = await folderPicker.PickAsync(ct);
        result.EnsureSuccess();

        await ToastHelper.Show($"Folder picked: {result.Folder.Name}, Path - {result.Folder.Path}",
            CancellationToken.None);

        return result.Folder;
    }

    private async void PickSrcFolder(object? sender, EventArgs e)
    {
        _srcFolder = await PickFolder(_emptyCt);
    }

    private async void PickDstFolder(object? sender, EventArgs e)
    {
        _dstFolder = await PickFolder(_emptyCt);
    }

    private async void SubmitBtn_OnClicked(object? sender, EventArgs e)
    {
        try
        {
            if (_srcFolder is null || _dstFolder is null || !Directory.Exists(_srcFolder.Path) ||
                !Directory.Exists(_dstFolder.Path))
            {
                await ToastHelper.Show("Please select both source and destination folders", CancellationToken.None);
                return;
            }

            DirectoryInfo srcDir = new(_srcFolder.Path);
            DirectoryInfo dstDir = new(_dstFolder.Path);
            var files = srcDir.GetFiles();

            ProgressBar.IsVisible = true;

            int filesMoved = 0;
            filesMoved = !_copyNotMove
                ? IOHelper.MoveAll(dstDir.FullName, files, _extensions, _overwrite)
                : IOHelper.CopyAll(dstDir.FullName, files, _extensions, _overwrite);

            await ToastHelper.Show($"{filesMoved} files moved.", CancellationToken.None);
            ProgressBar.IsVisible = false;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void OverwriteCheckbox_OnCheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        _overwrite = e.Value;
    }

    private void CopyNotMoveCheckbox_OnCheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        _copyNotMove = e.Value;
    }

    private void AddExtBtn_OnClicked(object? sender, EventArgs e)
    {
        string ext = (ExtInp.Text[0] == '.' ? char.MinValue : '.') + ExtInp.Text;
        var extensionItem = new ExtensionItem() { Name = ext };

        if (_extensions.All(x => x.Name != ext))
        {
            _extensions.Add(extensionItem);
        }

        ExtInp.Text = string.Empty;
    }

    private void DelExtBtn_OnClicked(object? sender, EventArgs e)
    {
        foreach (var item in ExtList.SelectedItems.Cast<ExtensionItem>())
        {
            _extensions.Remove(item);
        }
    }
}