using SmallDemoManager.HelperClass;
using ReaLTaiizor.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SmallDemoManager.UtilClass
{
    /// <summary>
    /// Provides static helper methods to configure and attach context menus to MaterialListView 
    /// and MaterialListBox controls. 
    /// Includes menu setups for player-related actions (copying Steam IDs, opening profile links) 
    /// and audio file operations (copying saved voice recordings).
    /// </summary>
    public static class CostumContextMenu
    {
        /// <summary>
        /// Used to create spacing between icons and text in menu items
        /// because of a visual overlap issue.
        /// </summary>
        private static string _spaceIcon = "       "; // workaround for icon/text overlap

        /// <summary>
        /// Configures the context menu for a <see cref="MaterialListView"/> with player actions.
        /// Includes options for copying the player's SteamID64 and opening profile links 
        /// on various external platforms.
        /// </summary>
        /// <param name="listView">The target MaterialListView control.</param>
        /// <param name="playerList">The list of players (snapshots) used to map context menu items.</param>
        /// <param name="steamProfileLink">Base URL for Steam profile links.</param>
        /// <param name="cswatchProfileLink">Base URL for cswatch.in profile links.</param>
        /// <param name="leetifyProfileLink">Base URL for leetify.com profile links.</param>
        /// <param name="csStatsProfileLink">Base URL for csstats.gg profile links.</param>
        /// <param name="left">Indicates whether the player name is stored in the left or right sub-item column.</param>
        public static void ConfigureContextMenuMainGrid(
            Form owner,
            MaterialListView listView,
            List<PlayerSnapshot> playerList,
            string steamProfileLink,
            string cswatchProfileLink,
            string leetifyProfileLink,
            string csStatsProfileLink,
            bool left)
        {
            var cms = new MaterialContextMenuStrip
            {
                ImageScalingSize = new Size(24, 24),
                MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER,
                AutoSize = true
            };

            // Menu header (player name)
            var playerHeader = new MaterialToolStripMenuItem
            {
                Text = "Player",
                Enabled = false,
                Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold),
                Image = Properties.Resources.iconPlayer,
                AutoSize = true
            };

            // Copy SteamID option
            var copySteamId = new MaterialToolStripMenuItem
            {
                Text = "Copy SteamID64",
                Image = Properties.Resources.iconSteam,
                AutoSize = true
            };

            // External profile link definitions
            var profileDefinitions = new (string Label, string UrlPrefix, Image Icon)[]
            {
                ("Open Steam Profile", steamProfileLink, Properties.Resources.iconSteam),
                ("Open cswatch.in Profile", cswatchProfileLink, Properties.Resources.iconCsWatch),
                ("Open leetify.com Profile", leetifyProfileLink, Properties.Resources.iconLeetify),
                ("Open csstats.gg Profile", csStatsProfileLink, Properties.Resources.iconCsStats)
            };

            // Generate menu items for profile links
            var profileItems = profileDefinitions.Select(def =>
            {
                var item = new MaterialToolStripMenuItem
                {
                    Text = _spaceIcon + def.Label,
                    Image = def.Icon,
                    AutoSize = true
                };
                return item;
            }).ToList();

            // Add items to context menu
            cms.Items.Add(playerHeader);
            cms.Items.Add(new ToolStripSeparator());
            cms.Items.Add(copySteamId);
            cms.Items.Add(new ToolStripSeparator());
            cms.Items.AddRange(profileItems.ToArray());

            listView.ContextMenuStrip = cms;

            // Copy SteamID handler
            copySteamId.Click += (s, e) =>
            {
                if (copySteamId.Tag is PlayerSnapshot p && p.PlayerSteamID.HasValue)
                {
                    Clipboard.SetText(p.PlayerSteamID.ToString() ?? "Error reading SteamID");
                    MaterialUiHelper.ShowSnack(owner, $"SteamID64 for {p.PlayerName} copied to clipboard.", false);
                }
            };

            // Profile link handlers
            foreach (var (menuItem, def) in profileItems.Zip(profileDefinitions))
            {
                menuItem.Click += (s, e) =>
                {
                    if (menuItem.Tag is PlayerSnapshot p && p.PlayerSteamID.HasValue)
                    {
                        string url = def.UrlPrefix + p.PlayerSteamID;
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = url,
                            UseShellExecute = true
                        });
                    }
                };
            }

            // Handle right-click behavior
            listView.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    ListViewItem clickedItem = listView.GetItemAt(e.X, e.Y);
                    if (clickedItem != null)
                    {
                        // Prevent context menu on last "dummy" row
                        if (clickedItem.Index == listView.Items.Count - 1)
                        {
                            listView.ContextMenuStrip = null;
                            return;
                        }

                        listView.SelectedItems.Clear();
                        clickedItem.Selected = true;

                        // Determine player name based on column layout
                        string playerName = left ? clickedItem.SubItems[2].Text : clickedItem.SubItems[1].Text;
                        var player = playerList.FirstOrDefault(p => p.PlayerName == playerName);

                        if (player != null)
                        {
                            playerHeader.Text = _spaceIcon + player.PlayerName;
                            copySteamId.Text = _spaceIcon + "Copy SteamID64";
                            copySteamId.Tag = player;
                            profileItems.ForEach(i => i.Tag = player);
                        }
                        else
                        {
                            playerHeader.Text = _spaceIcon + "Unknown Player";
                            copySteamId.Text = _spaceIcon + "Player not found";
                            copySteamId.Tag = null;
                            profileItems.ForEach(i =>
                            {
                                i.Text = "-";
                                i.Tag = null;
                            });
                        }

                        listView.ContextMenuStrip = cms;
                    }
                    else
                    {
                        // No item clicked → remove context menu
                        listView.ContextMenuStrip = null;
                    }
                }
            };
        }

        /// <summary>
        /// Configures a context menu for the provided <see cref="MaterialListBox"/> 
        /// to allow copying audio/voice files into a designated folder.  
        /// The new filename is constructed from the player's name, round number, and timestamp.
        /// </summary>
        /// <param name="listBox">The target MaterialListBox containing audio entries.</param>
        /// <param name="audioEntries">The list of audio entries corresponding to the items in the list box.</param>
        /// <param name="selectedPlayerVoicePlayer">The name of the player whose voice file is selected.</param>
        /// <param name="demoFileName">The original demo file name, used for deriving the output folder name.</param>
        public static void ConfigureContextMenuAudioFileCopy(
            Form owner,
            MaterialListBox listBox,
            List<AudioEntry> audioEntries,
            string selectedPlayerVoicePlayer,
            string demoFileName,
            Action? onFileCopied = null)
        {
            var cms = new MaterialContextMenuStrip
            {
                ImageScalingSize = new Size(24, 24),
                MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER,
                AutoSize = true
            };

            var copyFileItem = new MaterialToolStripMenuItem
            {
                Text = _spaceIcon + "Copy Voice-File",
                Image = Properties.Resources.iconCopy,
                AutoSize = true
            };

            cms.Items.Add(copyFileItem);
            listBox.ContextMenuStrip = cms;

            // Copy selected file handler
            copyFileItem.Click += (s, e) =>
            {
                int index = listBox.SelectedIndex;
                if (index >= 0 && index < audioEntries.Count)
                {
                    var entry = audioEntries[index];

                    try
                    {
                        string getTrueName = GetTrueName(demoFileName);

                        // Ask user for destination path
                        string savedVoiceRootPath = PathPicker.GetPath(owner,
                            "Select the path where the copied voice files should be saved.",
                            "SavedVoiceFilesPath");

                        // Ensure destination folder exists
                        string destinationFolder = Path.Combine(savedVoiceRootPath, "Saved-Voice-Files", getTrueName);
                        Directory.CreateDirectory(destinationFolder);

                        // Build new filename
                        string playerName = selectedPlayerVoicePlayer;
                        string round = "_-_Round " + entry.Round + "_-_";
                        string minute = "DemoTime " + entry.Time.ToString(@"hh\-mm\-ss");
                        string newFileName = playerName + round + minute + Path.GetExtension(entry.FilePath);

                        // Copy file
                        string destFile = Path.Combine(destinationFolder, newFileName);
                        File.Copy(entry.FilePath, destFile, true);

                        MaterialUiHelper.ShowSnack(owner, $"File copied to Saved-Audio-FilePath", false);
                        // After copy call callback
                        onFileCopied?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        MaterialUiHelper.ShowLongMessageBox("Error during copying", $"{ex.Message}", MessageBoxButtons.OK);
                    }
                }
            };
        }

        /// <summary>
        /// Extracts the "true" demo name by removing the first prefix up to "_-_" if present.
        /// </summary>
        /// <param name="filePath">The input demo file path.</param>
        /// <returns>The cleaned demo name without extension.</returns>
        static string GetTrueName(string filePath)
        {
            string name = Path.GetFileNameWithoutExtension(filePath);
            int index = name.IndexOf("_-_");
            return index >= 0 ? name[(index + 3)..] : name;
        }
    }
}
