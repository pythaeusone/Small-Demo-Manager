using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SmallDemoManager.HelperClass
{
    /// <summary>
    /// Provides helper methods for working with player-specific audio folders.
    /// </summary>
    public static class PlayerAudioHelper
    {
        /// <summary>
        /// Represents a player and the associated audio folder information.
        /// </summary>
        public class PlayerAudioItem
        {
            /// <summary>
            /// The Steam ID of the player, which is also used as the folder name.
            /// </summary>
            public string SteamId { get; set; } = string.Empty;

            /// <summary>
            /// The display name of the player. 
            /// If no name is found in the snapshot, the Steam ID will be used instead.
            /// </summary>
            public string DisplayName { get; set; } = string.Empty;
        }

        /// <summary>
        /// Loads all player folders from the given audio folder path and maps them to player information.
        /// </summary>
        /// <param name="snapshot">
        /// A collection of player snapshots that contains player metadata such as Steam IDs and player names.
        /// </param>
        /// <param name="audioFolderPath">
        /// The base directory path where each subfolder corresponds to a player (named by their Steam ID).
        /// </param>
        /// <returns>
        /// A list of <see cref="PlayerAudioItem"/> objects containing Steam IDs and display names,
        /// ordered alphabetically by display name.
        /// </returns>
        public static List<PlayerAudioItem> LoadPlayerFolders(IEnumerable<PlayerSnapshot>? snapshot, string? audioFolderPath)
        {
            // Return empty list if path is invalid or folder does not exist
            if (string.IsNullOrEmpty(audioFolderPath) || !Directory.Exists(audioFolderPath))
            {
                return new List<PlayerAudioItem>();
            }

            // Get all subfolder names (assumed to be Steam IDs)
            var directories = Directory.GetDirectories(audioFolderPath)
                .Select(Path.GetFileName)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToArray();

            // Map each folder to a PlayerAudioItem, looking up the display name from the snapshot if available
            return directories
                .Select(dir => new PlayerAudioItem
                {
                    SteamId = dir!,
                    DisplayName = snapshot?.FirstOrDefault(p => p.PlayerSteamID?.ToString() == dir)?.PlayerName ?? dir!
                })
                .OrderBy(p => p.DisplayName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }
}
