using Microsoft.Win32;
using SmallDemoManager.HelperClass;

namespace SmallDemoManager.UtilClass
{
    /// <summary>
    /// Provides functionality to add, remove, and validate a custom shell context menu entry 
    /// in Windows Explorer for specific file extensions.
    /// </summary>
    internal static class AddShellContextMenu
    {
        /// <summary>
        /// The file extensions for which the context menu integration should be applied.
        /// </summary>
        private static readonly string[] EXTENSIONS = new[] { ".dem" };

        /// <summary>
        /// The full path to the executable that should be launched when the context menu is used.
        /// </summary>
        private static readonly string EXE_PATH = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SmallDemoManager.exe"));

        /// <summary>
        /// The display name of the custom context menu entry.
        /// </summary>
        private const string MENU_NAME = "Open with Small Demo Manager";

        /// <summary>
        /// Adds the shell context menu integration for the defined file extensions.
        /// Creates or updates the required registry keys under <c>HKEY_CURRENT_USER</c>.
        /// </summary>
        /// <param name="fromCheck">
        /// Indicates whether this method was called from a validation check. 
        /// If true, no success message is shown to the user.
        /// </param>
        public static void AddShellIntegration(Form owner, bool fromCheck = false)
        {
            foreach (var ext in EXTENSIONS)
            {
                try
                {
                    string baseKeyPath = $"Software\\Classes\\SystemFileAssociations\\{ext}\\shell\\{MENU_NAME}";
                    string commandKeyPath = Path.Combine(baseKeyPath, "command");

                    // Create or update the base registry key for the context menu
                    using (var baseKey = Registry.CurrentUser.CreateSubKey(baseKeyPath))
                    {
                        baseKey?.SetValue(string.Empty, MENU_NAME);
                        baseKey?.SetValue("Icon", EXE_PATH + ",0");
                    }

                    // Create or update the command key to execute the program with the selected file as argument
                    using (var commandKey = Registry.CurrentUser.CreateSubKey(commandKeyPath))
                    {
                        commandKey?.SetValue(string.Empty, $"\"{EXE_PATH}\" \"%1\"");
                    }

                    if (!fromCheck)
                    {
                        MaterialUiHelper.ShowSnack(owner, "Shell integration added", false);
                    }
                }
                catch (Exception ex)
                {
                    MaterialUiHelper.ShowLongMessageBox($"Error adding registry key for {ext}", $"{ex.Message}", MessageBoxButtons.OK);
                }
            }
        }

        /// <summary>
        /// Removes the previously added shell context menu integration for the defined file extensions.
        /// Deletes the associated registry keys under <c>HKEY_CURRENT_USER</c>.
        /// </summary>
        public static void RemoveShellIntegration(Form owner)
        {
            foreach (var ext in EXTENSIONS)
            {
                try
                {
                    string baseKeyPath = $"Software\\Classes\\SystemFileAssociations\\{ext}\\shell\\{MENU_NAME}";
                    Registry.CurrentUser.DeleteSubKey(Path.Combine(baseKeyPath, "command"), false);
                    Registry.CurrentUser.DeleteSubKey(baseKeyPath, false);

                    MaterialUiHelper.ShowSnack(owner, "Removed shell integration for dem file.", false);
                }
                catch (Exception ex)
                {
                    MaterialUiHelper.ShowLongMessageBox($"Error removing registry key for {ext}", $"{ex.Message}", MessageBoxButtons.OK);
                }
            }
        }

        /// <summary>
        /// Validates whether the current shell context menu entry is present and up to date 
        /// for all configured file extensions. 
        /// 
        /// Behavior:
        /// - If no entry is found, the method returns false. 
        /// - If an outdated entry is found, the correct entry is written again (using AddShellIntegration with force = true) 
        ///   and the method returns true. 
        /// - If a valid entry is already present, the method simply returns true.
        /// </summary>
        public static bool ValidateShellIntegration(Form owner)
        {
            bool foundAny = false;     // Tracks whether at least one entry was found
            bool needsFix = false;     // Tracks whether any entry is outdated or incorrect

            string expectedCommand = $"\"{EXE_PATH}\" \"%1\"";

            foreach (var ext in EXTENSIONS)
            {
                string cmdPath = $@"Software\Classes\SystemFileAssociations\{ext}\shell\{MENU_NAME}\command";

                using (var key = Registry.CurrentUser.OpenSubKey(cmdPath, writable: false))
                {
                    if (key == null)
                        continue; // No entry exists for this extension -> continue checking

                    foundAny = true;

                    string existing = key.GetValue(string.Empty) as string ?? string.Empty;

                    if (!existing.Equals(expectedCommand, StringComparison.OrdinalIgnoreCase))
                        needsFix = true; // Entry exists but does not match expected command
                }
            }

            if (!foundAny)
                return false;              // Case 1: No entry found

            if (needsFix)
            {
                // Case 2: At least one entry is outdated -> rewrite entries without showing messages
                AddShellIntegration(owner, true);
            }

            return true;                   // Case 3: Entry is present (and possibly just updated)
        }


    }
}