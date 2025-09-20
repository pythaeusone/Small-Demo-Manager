using Microsoft.WindowsAPICodePack.Dialogs;
using SmallDemoManager.HelperClass;

namespace SmallDemoManager.UtilClass
{
    /// <summary>
    /// Handles selection and storage of the CS2 demo folder path using the JsonClass utility.
    /// </summary>
    internal static class PathPicker
    {
        /// <summary>
        /// Checks whether a non-empty path is already stored in the JSON config.
        /// </summary>
        public static bool HasPath(Form owner, string pathKey)
        {
            if (!JsonClass.KeyExists(pathKey))
                return false;

            var storedPath = JsonClass.ReadJson<string>(pathKey);
            return !string.IsNullOrWhiteSpace(storedPath);
        }

        /// <summary>
        /// Prompts the user with a folder-picker dialog to select the folder.
        /// Saves the chosen path to config.
        /// </summary>
        public static string EnsurePathConfigured(Form owner, string title, string pathKey)
        {
            while (true)
            {
                var dialog = new CommonOpenFileDialog
                {
                    IsFolderPicker = true,
                    Title = title,
                    InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    EnsurePathExists = true,
                    Multiselect = false
                };

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    string selectedPath = dialog.FileName;

                    if (!Directory.Exists(selectedPath))
                    {
                        MaterialUiHelper.ShowMaterialMsgBox(owner, "Invalid Path", "The selected path is not a valid directory.", "OK", false, "Cancel");
                        continue;
                    }

                    JsonClass.WriteJson(pathKey, selectedPath);
                    return selectedPath;
                }

                return null; // User cancelled
            }
        }

        /// <summary>
        /// Retrieves the stored folder path. If no path is stored,
        /// prompts the user to select one.
        /// </summary>
        public static string GetPath(Form owner, string title, string pathKey)
        {
            if (HasPath(owner, pathKey))
            {
                return JsonClass.ReadJson<string>(pathKey);
            }

            return EnsurePathConfigured(owner, title, pathKey);
        }

        /// <summary>
        /// Retrieves the stored folder path. If no path is stored,
        /// prompts the user to select one.
        /// </summary>
        public static string ReadPath(Form owner, string pathKey)
        {
            if (HasPath(owner, pathKey))
            {
                return JsonClass.ReadJson<string>(pathKey);
            }
            else
            {
                return null;
            }
        }
    }
}
