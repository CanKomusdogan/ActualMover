using System.Collections.ObjectModel;
using ActualMover.Models;

namespace ActualMover.Shared;

public static class IOHelper
{
    /// <returns>The amount of moved files</returns>
    public static int MoveAll(string dest, FileInfo[] files, ObservableCollection<ExtensionItem>? extensions,
        bool overwrite = false, ProgressBar? progressBar = null)
    {
        int totalFiles = files.Length;
        int filesMoved = 0;

        foreach (var file in files)
        {
            if (extensions is not null)
                if (extensions.All(x => x.Name != file.Extension))
                    continue;

            file.MoveTo(Path.Combine(dest, file.Name), overwrite);

            filesMoved++;

            if (progressBar is null) continue;

            double newProgress = (double)filesMoved / totalFiles;
            UIHelper.UpdateUI(progressBar, newProgress, "Progress");
        }

        return filesMoved;
    }

    /// <returns>The amount of moved files</returns>
    public static int CopyAll(string dest, FileInfo[] files, ObservableCollection<ExtensionItem>? extensions,
        bool overwrite = false, ProgressBar? progressBar = null)
    {
        int filesMoved = 0;

        foreach (var file in files)
        {
            if (extensions is not null)
                if (extensions.All(x => x.Name != file.Extension))
                    continue;

            file.CopyTo(Path.Combine(dest, file.Name), overwrite);

            filesMoved++;
        }

        return filesMoved;
    }
}