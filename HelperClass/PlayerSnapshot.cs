/// <summary>
/// Holds basic player information captured at round start.
/// </summary>
namespace SmallDemoManager.HelperClass
{
    public class PlayerSnapshot
    {
        public int UserId { get; set; }                 // User-ID + 1 = spec_player ID
        public string? PlayerName { get; set; }         // PlayerName = Name of the Player in this Game.
        public int TeamNumber { get; set; }             // TeamNumber = 2 = T-Side, 3 = CT-Side
        public string? TeamName { get; set; }           // TeamName = TeamClanName from Faceit team_xxxxx
        public ulong? PlayerSteamID { get; set; }       // Get the SteamID64 number
        public int Kills { get; set; }                  // Get Player kills
        public int Death { get; set; }                  // Get Player death
        public int Assists { get; set; }                // Get Assists
        public int HeadShotKill { get; set; }           // Get HS kills
        public double HeadShotPerecent { get; set; }    // Get HS kills %
        public double Kd { get; set; }                  // K/D
        public int Damage { get; set; }                 // Get Total Damage
        public int MVP { get; set; }                    // Get MVPs
        public int ThreeK { get; set; }                 // Get 3k
        public int FourK { get; set; }                  // Get 4k
        public int FiveK { get; set; }                  // Get 5k
        public int EndScore { get; set; }               // Get the EndScore

        public override string ToString()
        {
            return PlayerName ?? "Unknown Player";
        }
    }
}
