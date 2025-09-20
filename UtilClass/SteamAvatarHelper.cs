using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmallDemoManager.UtilClass
{
    /// <summary>
    /// Static helper class to download and save Steam user avatars.
    /// </summary>
    /// <remarks>
    /// This class provides a method that takes a SteamID64, retrieves the avatar URL
    /// from the Steam Community profile (supports JSON and XML fallback), and saves the
    /// image into a local folder named "AvatarImage". The method returns true on success,
    /// false otherwise.
    /// </remarks>
    public static class SteamAvatarHelper
    {
        private static readonly HttpClient httpClient = new HttpClient();

        /// <summary>
        /// Downloads the avatar of the given SteamID64 and saves it to the "AvatarImage" folder.
        /// </summary>
        /// <param name="steamId">The 64-bit SteamID of the user.</param>
        /// <returns>True if the avatar was successfully downloaded and saved, false otherwise.</returns>
        public static async Task<bool> SaveAvatarAsync(string steamId)
        {
            try
            {
                // Retrieve the avatar URL from Steam community
                string avatarUrl = await GetSteamAvatarUrlAsync(steamId);

                if (string.IsNullOrEmpty(avatarUrl) || !Uri.IsWellFormedUriString(avatarUrl, UriKind.Absolute))
                    return false;

                // Download the image data
                var data = await httpClient.GetByteArrayAsync(avatarUrl);

                // Ensure the AvatarImage directory exists
                string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AvatarImage");
                Directory.CreateDirectory(folderPath);

                // Build file path (SteamID as file name)
                string filePath = Path.Combine(folderPath, $"{steamId}.jpg");

                // Save the file
                await File.WriteAllBytesAsync(filePath, data);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieves the avatar image URL for the specified SteamID64.
        /// Attempts JSON first, then falls back to XML.
        /// </summary>
        /// <param name="steamId">The 64-bit SteamID of the user.</param>
        /// <returns>The avatar image URL if found, otherwise null.</returns>
        private static async Task<string> GetSteamAvatarUrlAsync(string steamId)
        {
            try
            {
                // Try JSON format
                string url = $"https://steamcommunity.com/profiles/{steamId}?format=json";
                string json = await httpClient.GetStringAsync(url);

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (root.TryGetProperty("avatar_full", out var avatarFull))
                {
                    return avatarFull.GetString();
                }
            }
            catch
            {
                // If JSON fails, fallback to XML
                try
                {
                    string url = $"https://steamcommunity.com/profiles/{steamId}?xml=1";
                    string xml = await httpClient.GetStringAsync(url);

                    int start = xml.IndexOf("<avatarFull>") + "<avatarFull>".Length;
                    int end = xml.IndexOf("</avatarFull>", start);
                    if (start > 0 && end > start)
                    {
                        string raw = xml.Substring(start, end - start);
                        // Remove CDATA if present
                        raw = raw.Replace("<![CDATA[", string.Empty).Replace("]]>", string.Empty).Trim();
                        return raw;
                    }
                }
                catch { }
            }

            return null;
        }
    }
}
