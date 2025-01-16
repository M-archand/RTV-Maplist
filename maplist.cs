using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;

namespace maplist
{
    public class Maplist : BasePlugin
    {
        public override string ModuleName => "RTV Maplist";
        public override string ModuleAuthor => "小彩旗";
        public override string ModuleVersion => "0.0.1";

        private string maplistPath = string.Empty;
        private Dictionary<ulong, DateTime> _lastCommandUse = new Dictionary<ulong, DateTime>();
        private const int CommandCooldown = 5; // Cooldown time (seconds)

        public override void Load(bool hotReload)
        {
            // Construct the full path for the RockTheVote plugin map list
            string rtvPath = Path.Combine(Server.GameDirectory, "csgo", "addons", "counterstrikesharp", "plugins", "RockTheVote", "maplist.txt");
            maplistPath = rtvPath;
            Console.WriteLine($"RTV map list file path: {maplistPath}");
        }

        [ConsoleCommand("css_maps", "Displays the available map list")]
        [ConsoleCommand("css_maplist", "Displays the available map list")]
        public void OnMaplistCommand(CCSPlayerController? player, CommandInfo command)
        {
            try
            {
                // Check cooldown (applies only to players, console is not restricted)
                if (player != null)
                {
                    ulong steamId = player.SteamID;
                    if (_lastCommandUse.TryGetValue(steamId, out DateTime lastUse))
                    {
                        int secondsLeft = CommandCooldown - (int)(DateTime.Now - lastUse).TotalSeconds;
                        if (secondsLeft > 0)
                        {
                            player.PrintToChat($"{ChatColors.Blue}[Maplist]{ChatColors.Default} Please wait {secondsLeft} seconds before using this command again");
                            return;
                        }
                    }
                    _lastCommandUse[steamId] = DateTime.Now;
                }

                if (!File.Exists(maplistPath))
                {
                    ReplyToCommand(player, "The map list file does not exist!");
                    return;
                }

                if (player != null)
                {
                    string[] maps = File.ReadAllLines(maplistPath);

                    player.PrintToChat($" {ChatColors.LightRed}[Maplist]{ChatColors.Default} Maps have been printed to the console.");
                    player.PrintToConsole("====================================");
                    player.PrintToConsole("             Server Map List");
                    player.PrintToConsole($"             Total Maps: {maps.Length}");
                    player.PrintToConsole("====================================");

                    foreach (string map in maps)
                    {
                        if (!string.IsNullOrWhiteSpace(map))
                        {
                            string mapName = map.Split(':')[0];
                            player.PrintToConsole(mapName);
                        }
                    }

                    player.PrintToConsole("====================================");
                }
                else
                {
                    string[] maps = File.ReadAllLines(maplistPath);

                    Console.WriteLine("====================================");
                    Console.WriteLine("             Server Map List");
                    Console.WriteLine($"             Total Maps: {maps.Length}");
                    Console.WriteLine("====================================");

                    foreach (string map in maps)
                    {
                        if (!string.IsNullOrWhiteSpace(map))
                        {
                            string mapName = map.Split(':')[0];
                            Console.WriteLine(mapName);
                        }
                    }

                    Console.WriteLine("====================================");
                }
            }
            catch (Exception ex)
            {
                ReplyToCommand(player, $"An error occurred while reading the map list: {ex.Message}");
            }
        }

        private void ReplyToCommand(CCSPlayerController? player, string message)
        {
            if (player == null)
            {
                // If the command is executed from the server console
                Console.WriteLine(message);
            }
            else
            {
                // If the command is executed by a player
                player.PrintToChat($" {ChatColors.LightRed}[Maplist]{ChatColors.Default} {message}");
            }
        }
    }
}