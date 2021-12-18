using System;
using System.Linq;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using System.Security.Cryptography;

namespace CNPC
{
    [ApiVersion(2, 1)]
    public class CNPC : TerrariaPlugin
    {
        public override string Name => "TerraRoyale";
        public override string Author => "xXCrypticNightXx - D.RUSS#2430";
        public override string Description => "A Terraria Based Battle Royale. CPU Orginized Team Fighting.";
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        public CNPC(Main game) : base(game)
        {
        }

        // Game Hook
        public override void Initialize()
        {
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
        }

        // Hook Disposing
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // deregister the getdata hook
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
            }
            base.Dispose(disposing);
        }

        // .DLL Loaded In Console
        private void OnInitialize(EventArgs args)
        {
            // Add Command To Console
            Commands.ChatCommands.Add(new Command("battlestart.use", Battlestart, "battlestart") { HelpText = "Usage: /battlestart" });
        }

        // Get Median Of Array
        public float GetMedian(int[] ArrayData)
        {
            int count = ArrayData.Length;
            Array.Sort(ArrayData);
            decimal medianValue = 0;
            if (count % 2 == 0)
            {
                // count is even, need to get the middle two elements, add them together, then divide by 2
                int middleElement1 = ArrayData[(count / 2) - 1];
                int middleElement2 = ArrayData[(count / 2)];
                medianValue = (middleElement1 + middleElement2) / 2;
            }
            else
            {
                // count is odd, simply get the middle element.
                medianValue = ArrayData[(count / 2)];
            }

            // Return Middle Number
            return (float)medianValue;
        }

        // Get True Random Number
        public static int Next(int min, int max)
        {
            if (min > max)
            {
                throw new ArgumentOutOfRangeException("min");
            }

            if (min == max)
            {
                return min;
            }

            int result;
            using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                byte[] array = new byte[4];
                rngcryptoServiceProvider.GetBytes(array);
                int num = Math.Abs(BitConverter.ToInt32(array, 0));
                int num2 = max - min;
                int num3 = num % num2;
                result = min + num3;
            }

            return result;
        }

        // Command To Generate Player Teams
        private void Battlestart(CommandArgs args)
        {
            // Count Active Players In Server
            int playercount = 0;
            foreach (TSPlayer player in TShock.Players)
            {
                if (player == null || !player.Active)
                    continue;
                playercount++;
            }

            args.Player.SendErrorMessage(playercount.ToString());

            // Ensure Players Exist
            if (playercount <= 1)
            {
                args.Player.SendErrorMessage("[TerraRoyale] Less then two players exist! Aborting.");
                return;
            }

            // Add each players team to an array
            int[] teamlist = new int[playercount];
            foreach (TSPlayer player in TShock.Players)
            {
                if (player == null || !player.Active)
                    continue;
                
                // Attempt To Throw Unteamed Player Into Team
                if (player.Team == 0)
                {
                    // Using Random Number 1-5, Add Player To Team
                    player.SetTeam(Next(1, 5));
                    player.SendMessage("[TerraRoyale] You have been added to a party!", Microsoft.Xna.Framework.Color.Lime);
                }

                // Update Array Builder With Players Team
                teamlist[player.Index] = player.Team;
            }

            // Challenge the array to check if all players are in a team
            if (teamlist.Contains(0))
            {
                TShock.Log.ConsoleInfo("[TerraRoyale] Not All Players Are In A Team!");
                args.Player.SendErrorMessage("[TerraRoyale] Not All Players Are In A Team!");
                return;
            }

            // Challenge the array to check if at least two players have unique teams
            if (teamlist.Skip(1).All(s => string.Equals(teamlist[0].ToString(), s.ToString(), StringComparison.InvariantCultureIgnoreCase)))
            {
                TShock.Log.ConsoleInfo("[TerraRoyale] All players are on the same team! Aborting.");
                args.Player.SendErrorMessage("[TerraRoyale] All players are on the same team! Aborting.");
                return;
            }

            // Setup Vars
            int TeamOnesColor = 0;
            int TeamTwosColor = 0;
            int TeamThreesColor = 0;
            int TeamFoursColor = 0;
            int TeamFivesColor = 0;

            // Get Each Players team
            foreach (TSPlayer player in TShock.Players)
            {
                try
                {
                    // Get Team Block Color
                    short PaintType = 0; // Defualt Team
                    if (player.Team == 0)
                    {
                        // Player Not In A Team
                        args.Player.SendErrorMessage("[TerraRoyale] A player found not in a team! Aborting.");
                        break;
                    }
                    else if (player.Team == 1) // Deep Red
                    {
                        PaintType = 13;
                    }
                    else if (player.Team == 2) // Deep Green
                    {
                        PaintType = 17;
                    }
                    else if (player.Team == 3) // Deep Blue
                    {
                        PaintType = 21;
                    }
                    else if (player.Team == 4) // Deep Yellow
                    {
                        PaintType = 15;
                    }
                    else if (player.Team == 5) // Deep Pink
                    {
                        PaintType = 24;
                    }

                    #region Team One
                    // Set This Teams Fixed Color
                    if (TeamOnesColor == 0)
                    {
                        TeamOnesColor = player.Team;
                    }
                    // Only This x Color Goes Here
                    if (player.Team == TeamOnesColor)
                    {
                        // Place Team Ones Platform
                        if (WorldGen.lastMaxTilesX == 4200 || WorldGen.lastMaxTilesX == 6400 || WorldGen.lastMaxTilesX == 8400)
                        {
                            // Get Platform Height
                            int LocationsY = 0;
                            for (int Height = 1; Height < 2350 + 1; Height++)
                            {
                                // Starting From Space, Search For Ground
                                if (Terraria.Main.tile[83, Height].active() || Terraria.Main.tile[83, Height].liquid != 0)
                                {
                                    // Add 10 Above Ground And Log Value
                                    LocationsY = Height - 10;
                                    break;
                                }
                            }

                            int[] TeamOneLocation = new int[10] { 83, 84, 85, 86, 87, 88, 89, 90, 91, 92 }; // Platofrom Locations
                            foreach (int LocationsX in TeamOneLocation)
                            {
                                if (Terraria.Main.tile[LocationsX, LocationsY + 10].type != Terraria.ID.TileID.Platforms) // Skip Placing If Already Exists
                                {
                                    // Place Tiles Into World
                                    Terraria.Main.tile[LocationsX, LocationsY].type = 19; // 19 Platform ID
                                    Terraria.Main.tile[LocationsX, LocationsY].sTileHeader = PaintType; // Paint Block
                                    Terraria.Main.tile[LocationsX, LocationsY].active(true);
                                }
                                else
                                {
                                    // Change Color of Existing Platform
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].type = 19; // 19 Platform ID
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].sTileHeader = PaintType; // Paint Block
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].active(true);
                                }
                            }

                            // Teleport Player To Respected Platform
                            player.Teleport(GetMedian(TeamOneLocation) * 16, LocationsY * 16 - 48, 1);

                            // Enable Players PvP Status
                            player.SetPvP(true, false);
                            player.SendMessage("[TerraRoyale] Your PvP Was Enabled!", Microsoft.Xna.Framework.Color.Lime);

                            goto NextPlayer;
                        }
                    }
                    #endregion

                    #region Team Two
                    // Set This Teams Fixed Color
                    if (TeamTwosColor == 0)
                    {
                        TeamTwosColor = player.Team;
                    }
                    // Only This x Color Goes Here
                    if (player.Team == TeamTwosColor)
                    {
                        // Place Team Two's Platform
                        if (WorldGen.lastMaxTilesX == 4200) // Small World
                        {
                            // Get Platform Height
                            int LocationsY = 0;
                            for (int Height = 1; Height < 2350 + 1; Height++)
                            {
                                // Starting From Space, Search For Ground
                                if (Terraria.Main.tile[83, Height].active() || Terraria.Main.tile[83, Height].liquid != 0)
                                {
                                    // Add 10 Above Ground And Log Value
                                    LocationsY = Height - 10;
                                    break;
                                }
                            }

                            int[] TeamOneLocation = new int[10] { 4117, 4116, 4115, 4114, 4113, 4112, 4111, 4110, 4109, 4108 }; // Platofrom Locations Small World
                            foreach (int LocationsX in TeamOneLocation)
                            {
                                if (Terraria.Main.tile[LocationsX, LocationsY + 10].type != Terraria.ID.TileID.Platforms) // Skip Placing If Already Exists
                                {
                                    // Place Tiles Into World
                                    Terraria.Main.tile[LocationsX, LocationsY].type = 19; // 19 Platform ID
                                    Terraria.Main.tile[LocationsX, LocationsY].sTileHeader = PaintType; // Paint Block
                                    Terraria.Main.tile[LocationsX, LocationsY].active(true);
                                }
                                else
                                {
                                    // Change Color of Existing Platform
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].type = 19; // 19 Platform ID
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].sTileHeader = PaintType; // Paint Block
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].active(true);
                                }
                            }

                            // Teleport Player To Respected Platform
                            player.Teleport(GetMedian(TeamOneLocation) * 16, LocationsY * 16 - 48, 1);

                            // Enable Players PvP Status
                            player.SetPvP(true, false);
                            player.SendMessage("[TerraRoyale] Your PvP Was Enabled!", Microsoft.Xna.Framework.Color.Lime);

                            goto NextPlayer;
                        }
                        else if (WorldGen.lastMaxTilesX == 6400) // Medium World
                        {
                            // Get Platform Height
                            int LocationsY = 0;
                            for (int Height = 1; Height < 2350 + 1; Height++)
                            {
                                // Starting From Space, Search For Ground
                                if (Terraria.Main.tile[83, Height].active() || Terraria.Main.tile[83, Height].liquid != 0)
                                {
                                    // Add 10 Above Ground And Log Value
                                    LocationsY = Height - 10;
                                    break;
                                }
                            }

                            int[] TeamOneLocation = new int[10] { 6317, 6316, 6315, 6314, 6313, 6312, 6311, 6310, 6309, 6308 }; // Platofrom Locations Medium World
                            foreach (int LocationsX in TeamOneLocation)
                            {
                                if (Terraria.Main.tile[LocationsX, LocationsY + 10].type != Terraria.ID.TileID.Platforms) // Skip Placing If Already Exists
                                {
                                    // Place Tiles Into World
                                    Terraria.Main.tile[LocationsX, LocationsY].type = 19; // 19 Platform ID
                                    Terraria.Main.tile[LocationsX, LocationsY].sTileHeader = PaintType; // Paint Block
                                    Terraria.Main.tile[LocationsX, LocationsY].active(true);
                                }
                                else
                                {
                                    // Change Color of Existing Platform
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].type = 19; // 19 Platform ID
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].sTileHeader = PaintType; // Paint Block
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].active(true);
                                }
                            }

                            // Teleport Player To Respected Platform
                            player.Teleport(GetMedian(TeamOneLocation) * 16, LocationsY * 16 - 48, 1);

                            // Enable Players PvP Status
                            player.SetPvP(true, false);
                            player.SendMessage("[TerraRoyale] Your PvP Was Enabled!", Microsoft.Xna.Framework.Color.Lime);

                            goto NextPlayer;
                        }
                        else if (WorldGen.lastMaxTilesX == 8400) // Large World
                        {
                            // Get Platform Height
                            int LocationsY = 0;
                            for (int Height = 1; Height < 2350 + 1; Height++)
                            {
                                // Starting From Space, Search For Ground
                                if (Terraria.Main.tile[83, Height].active() || Terraria.Main.tile[83, Height].liquid != 0)
                                {
                                    // Add 10 Above Ground And Log Value
                                    LocationsY = Height - 10;
                                    break;
                                }
                            }

                            int[] TeamOneLocation = new int[10] { 8317, 8316, 8315, 8314, 8313, 8312, 8311, 8310, 8309, 8308 }; // Platofrom Locations Large World
                            foreach (int LocationsX in TeamOneLocation)
                            {
                                if (Terraria.Main.tile[LocationsX, LocationsY + 10].type != Terraria.ID.TileID.Platforms) // Skip Placing If Already Exists
                                {
                                    // Place Tiles Into World
                                    Terraria.Main.tile[LocationsX, LocationsY].type = 19; // 19 Platform ID
                                    Terraria.Main.tile[LocationsX, LocationsY].sTileHeader = PaintType; // Paint Block
                                    Terraria.Main.tile[LocationsX, LocationsY].active(true);
                                }
                                else
                                {
                                    // Change Color of Existing Platform
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].type = 19; // 19 Platform ID
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].sTileHeader = PaintType; // Paint Block
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].active(true);
                                }
                            }

                            // Teleport Player To Respected Platform
                            player.Teleport(GetMedian(TeamOneLocation) * 16, LocationsY * 16 - 48, 1);

                            // Enable Players PvP Status
                            player.SetPvP(true, false);
                            player.SendMessage("[TerraRoyale] Your PvP Was Enabled!", Microsoft.Xna.Framework.Color.Lime);

                            goto NextPlayer;
                        }
                    }
                    #endregion

                    #region Team Three
                    // Set This Teams Fixed Color
                    if (TeamThreesColor == 0)
                    {
                        TeamThreesColor = player.Team;
                    }
                    // Only This x Color Goes Here
                    if (player.Team == TeamThreesColor)
                    {
                        // Place Team Two's Platform
                        if (WorldGen.lastMaxTilesX == 4200) // Small World
                        {
                            // Get Platform Height
                            int LocationsY = 0;
                            for (int Height = 1; Height < 2350 + 1; Height++)
                            {
                                // Starting From Space, Search For Ground
                                if (Terraria.Main.tile[83, Height].active() || Terraria.Main.tile[83, Height].liquid != 0)
                                {
                                    // Add 10 Above Ground And Log Value
                                    LocationsY = Height - 10;
                                    break;
                                }
                            }

                            int[] TeamOneLocation = new int[10] { 2058, 2059, 2060, 2061, 2062, 2063, 2064, 2065, 2066, 2067 }; // Platofrom Locations Small World
                            foreach (int LocationsX in TeamOneLocation)
                            {
                                if (Terraria.Main.tile[LocationsX, LocationsY + 10].type != Terraria.ID.TileID.Platforms) // Skip Placing If Already Exists
                                {
                                    // Place Tiles Into World
                                    Terraria.Main.tile[LocationsX, LocationsY].type = 19; // 19 Platform ID
                                    Terraria.Main.tile[LocationsX, LocationsY].sTileHeader = PaintType; // Paint Block
                                    Terraria.Main.tile[LocationsX, LocationsY].active(true);
                                }
                                else
                                {
                                    // Change Color of Existing Platform
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].type = 19; // 19 Platform ID
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].sTileHeader = PaintType; // Paint Block
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].active(true);
                                }
                            }

                            // Teleport Player To Respected Platform
                            player.Teleport(GetMedian(TeamOneLocation) * 16, LocationsY * 16 - 48, 1);

                            // Enable Players PvP Status
                            player.SetPvP(true, false);
                            player.SendMessage("[TerraRoyale] Your PvP Was Enabled!", Microsoft.Xna.Framework.Color.Lime);

                            goto NextPlayer;
                        }
                        else if (WorldGen.lastMaxTilesX == 6400) // Medium World
                        {
                            // Get Platform Height
                            int LocationsY = 0;
                            for (int Height = 1; Height < 2350 + 1; Height++)
                            {
                                // Starting From Space, Search For Ground
                                if (Terraria.Main.tile[83, Height].active() || Terraria.Main.tile[83, Height].liquid != 0)
                                {
                                    // Add 10 Above Ground And Log Value
                                    LocationsY = Height - 10;
                                    break;
                                }
                            }

                            int[] TeamOneLocation = new int[10] { 3158, 3159, 3160, 3161, 3162, 3163, 3164, 3165, 3166, 3167 }; // Platofrom Locations Medium World
                            foreach (int LocationsX in TeamOneLocation)
                            {
                                if (Terraria.Main.tile[LocationsX, LocationsY + 10].type != Terraria.ID.TileID.Platforms) // Skip Placing If Already Exists
                                {
                                    // Place Tiles Into World
                                    Terraria.Main.tile[LocationsX, LocationsY].type = 19; // 19 Platform ID
                                    Terraria.Main.tile[LocationsX, LocationsY].sTileHeader = PaintType; // Paint Block
                                    Terraria.Main.tile[LocationsX, LocationsY].active(true);
                                }
                                else
                                {
                                    // Change Color of Existing Platform
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].type = 19; // 19 Platform ID
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].sTileHeader = PaintType; // Paint Block
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].active(true);
                                }
                            }

                            // Teleport Player To Respected Platform
                            player.Teleport(GetMedian(TeamOneLocation) * 16, LocationsY * 16 - 48, 1);

                            // Enable Players PvP Status
                            player.SetPvP(true, false);
                            player.SendMessage("[TerraRoyale] Your PvP Was Enabled!", Microsoft.Xna.Framework.Color.Lime);

                            goto NextPlayer;
                        }
                        else if (WorldGen.lastMaxTilesX == 8400) // Large World
                        {
                            // Get Platform Height
                            int LocationsY = 0;
                            for (int Height = 1; Height < 2350 + 1; Height++)
                            {
                                // Starting From Space, Search For Ground
                                if (Terraria.Main.tile[83, Height].active() || Terraria.Main.tile[83, Height].liquid != 0)
                                {
                                    // Add 10 Above Ground And Log Value
                                    LocationsY = Height - 10;
                                    break;
                                }
                            }

                            int[] TeamOneLocation = new int[10] { 4158, 4159, 4160, 4161, 4162, 4163, 4164, 4165, 4166, 4167 }; // Platofrom Locations Large World
                            foreach (int LocationsX in TeamOneLocation)
                            {
                                if (Terraria.Main.tile[LocationsX, LocationsY + 10].type != Terraria.ID.TileID.Platforms) // Skip Placing If Already Exists
                                {
                                    // Place Tiles Into World
                                    Terraria.Main.tile[LocationsX, LocationsY].type = 19; // 19 Platform ID
                                    Terraria.Main.tile[LocationsX, LocationsY].sTileHeader = PaintType; // Paint Block
                                    Terraria.Main.tile[LocationsX, LocationsY].active(true);
                                }
                                else
                                {
                                    // Change Color of Existing Platform
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].type = 19; // 19 Platform ID
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].sTileHeader = PaintType; // Paint Block
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].active(true);
                                }
                            }

                            // Teleport Player To Respected Platform
                            player.Teleport(GetMedian(TeamOneLocation) * 16, LocationsY * 16 - 48, 1);

                            // Enable Players PvP Status
                            player.SetPvP(true, false);
                            player.SendMessage("[TerraRoyale] Your PvP Was Enabled!", Microsoft.Xna.Framework.Color.Lime);

                            goto NextPlayer;
                        }
                    }
                    #endregion

                    #region Team Four
                    // Set This Teams Fixed Color
                    if (TeamFoursColor == 0)
                    {
                        TeamFoursColor = player.Team;
                    }
                    // Only This x Color Goes Here
                    if (player.Team == TeamFoursColor)
                    {
                        // Place Team Two's Platform
                        if (WorldGen.lastMaxTilesX == 4200) // Small World
                        {
                            // Get Platform Height
                            int LocationsY = 0;
                            for (int Height = 1; Height < 2350 + 1; Height++)
                            {
                                // Starting From Space, Search For Ground
                                if (Terraria.Main.tile[83, Height].active() || Terraria.Main.tile[83, Height].liquid != 0)
                                {
                                    // Add 10 Above Ground And Log Value
                                    LocationsY = Height - 10;
                                    break;
                                }
                            }

                            int[] TeamOneLocation = new int[10] { 1372, 1373, 1374, 1375, 1376, 1377, 1378, 1379, 1380, 1381 }; // Platofrom Locations Small World
                            foreach (int LocationsX in TeamOneLocation)
                            {
                                if (Terraria.Main.tile[LocationsX, LocationsY + 10].type != Terraria.ID.TileID.Platforms) // Skip Placing If Already Exists
                                {
                                    // Place Tiles Into World
                                    Terraria.Main.tile[LocationsX, LocationsY].type = 19; // 19 Platform ID
                                    Terraria.Main.tile[LocationsX, LocationsY].sTileHeader = PaintType; // Paint Block
                                    Terraria.Main.tile[LocationsX, LocationsY].active(true);
                                }
                                else
                                {
                                    // Change Color of Existing Platform
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].type = 19; // 19 Platform ID
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].sTileHeader = PaintType; // Paint Block
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].active(true);
                                }
                            }

                            // Teleport Player To Respected Platform
                            player.Teleport(GetMedian(TeamOneLocation) * 16, LocationsY * 16 - 48, 1);

                            // Enable Players PvP Status
                            player.SetPvP(true, false);
                            player.SendMessage("[TerraRoyale] Your PvP Was Enabled!", Microsoft.Xna.Framework.Color.Lime);

                            goto NextPlayer;
                        }
                        else if (WorldGen.lastMaxTilesX == 6400) // Medium World
                        {
                            // Get Platform Height
                            int LocationsY = 0;
                            for (int Height = 1; Height < 2350 + 1; Height++)
                            {
                                // Starting From Space, Search For Ground
                                if (Terraria.Main.tile[83, Height].active() || Terraria.Main.tile[83, Height].liquid != 0)
                                {
                                    // Add 10 Above Ground And Log Value
                                    LocationsY = Height - 10;
                                    break;
                                }
                            }

                            int[] TeamOneLocation = new int[10] { 2105, 2106, 2107, 2108, 2109, 2110, 2111, 2112, 2113, 2114 }; // Platofrom Locations Medium World
                            foreach (int LocationsX in TeamOneLocation)
                            {
                                if (Terraria.Main.tile[LocationsX, LocationsY + 10].type != Terraria.ID.TileID.Platforms) // Skip Placing If Already Exists
                                {
                                    // Place Tiles Into World
                                    Terraria.Main.tile[LocationsX, LocationsY].type = 19; // 19 Platform ID
                                    Terraria.Main.tile[LocationsX, LocationsY].sTileHeader = PaintType; // Paint Block
                                    Terraria.Main.tile[LocationsX, LocationsY].active(true);
                                }
                                else
                                {
                                    // Change Color of Existing Platform
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].type = 19; // 19 Platform ID
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].sTileHeader = PaintType; // Paint Block
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].active(true);
                                }
                            }

                            // Teleport Player To Respected Platform
                            player.Teleport(GetMedian(TeamOneLocation) * 16, LocationsY * 16 - 48, 1);

                            // Enable Players PvP Status
                            player.SetPvP(true, false);
                            player.SendMessage("[TerraRoyale] Your PvP Was Enabled!", Microsoft.Xna.Framework.Color.Lime);

                            goto NextPlayer;
                        }
                        else if (WorldGen.lastMaxTilesX == 8400) // Large World
                        {
                            // Get Platform Height
                            int LocationsY = 0;
                            for (int Height = 1; Height < 2350 + 1; Height++)
                            {
                                // Starting From Space, Search For Ground
                                if (Terraria.Main.tile[83, Height].active() || Terraria.Main.tile[83, Height].liquid != 0)
                                {
                                    // Add 10 Above Ground And Log Value
                                    LocationsY = Height - 10;
                                    break;
                                }
                            }

                            int[] TeamOneLocation = new int[10] { 2772, 2773, 2774, 2775, 2776, 2777, 2778, 2779, 2780, 2781 }; // Platofrom Locations Large World
                            foreach (int LocationsX in TeamOneLocation)
                            {
                                if (Terraria.Main.tile[LocationsX, LocationsY + 10].type != Terraria.ID.TileID.Platforms) // Skip Placing If Already Exists
                                {
                                    // Place Tiles Into World
                                    Terraria.Main.tile[LocationsX, LocationsY].type = 19; // 19 Platform ID
                                    Terraria.Main.tile[LocationsX, LocationsY].sTileHeader = PaintType; // Paint Block
                                    Terraria.Main.tile[LocationsX, LocationsY].active(true);
                                }
                                else
                                {
                                    // Change Color of Existing Platform
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].type = 19; // 19 Platform ID
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].sTileHeader = PaintType; // Paint Block
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].active(true);
                                }
                            }

                            // Teleport Player To Respected Platform
                            player.Teleport(GetMedian(TeamOneLocation) * 16, LocationsY * 16 - 48, 1);

                            // Enable Players PvP Status
                            player.SetPvP(true, false);
                            player.SendMessage("[TerraRoyale] Your PvP Was Enabled!", Microsoft.Xna.Framework.Color.Lime);

                            goto NextPlayer;
                        }
                    }
                    #endregion

                    #region Team Five
                    // Set This Teams Fixed Color
                    if (TeamFivesColor == 0)
                    {
                        TeamFivesColor = player.Team;
                    }
                    // Only This x Color Goes Here
                    if (player.Team == TeamFivesColor)
                    {
                        // Place Team Two's Platform
                        if (WorldGen.lastMaxTilesX == 4200) // Small World
                        {
                            // Get Platform Height
                            int LocationsY = 0;
                            for (int Height = 1; Height < 2350 + 1; Height++)
                            {
                                // Starting From Space, Search For Ground
                                if (Terraria.Main.tile[83, Height].active() || Terraria.Main.tile[83, Height].liquid != 0)
                                {
                                    // Add 10 Above Ground And Log Value
                                    LocationsY = Height - 10;
                                    break;
                                }
                            }

                            int[] TeamOneLocation = new int[10] { 2745, 2744, 2743, 2742, 2741, 2740, 2739, 2738, 2737, 2736 }; // Platofrom Locations Small World
                            foreach (int LocationsX in TeamOneLocation)
                            {
                                if (Terraria.Main.tile[LocationsX, LocationsY + 10].type != Terraria.ID.TileID.Platforms) // Skip Placing If Already Exists
                                {
                                    // Place Tiles Into World
                                    Terraria.Main.tile[LocationsX, LocationsY].type = 19; // 19 Platform ID
                                    Terraria.Main.tile[LocationsX, LocationsY].sTileHeader = PaintType; // Paint Block
                                    Terraria.Main.tile[LocationsX, LocationsY].active(true);
                                }
                                else
                                {
                                    // Change Color of Existing Platform
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].type = 19; // 19 Platform ID
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].sTileHeader = PaintType; // Paint Block
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].active(true);
                                }
                            }

                            // Teleport Player To Respected Platform
                            player.Teleport(GetMedian(TeamOneLocation) * 16, LocationsY * 16 - 48, 1);

                            // Enable Players PvP Status
                            player.SetPvP(true, false);
                            player.SendMessage("[TerraRoyale] Your PvP Was Enabled!", Microsoft.Xna.Framework.Color.Lime);

                            goto NextPlayer;
                        }
                        else if (WorldGen.lastMaxTilesX == 6400) // Medium World
                        {
                            // Get Platform Height
                            int LocationsY = 0;
                            for (int Height = 1; Height < 2350 + 1; Height++)
                            {
                                // Starting From Space, Search For Ground
                                if (Terraria.Main.tile[83, Height].active() || Terraria.Main.tile[83, Height].liquid != 0)
                                {
                                    // Add 10 Above Ground And Log Value
                                    LocationsY = Height - 10;
                                    break;
                                }
                            }

                            int[] TeamOneLocation = new int[10] { 4212, 4211, 4210, 4209, 4208, 4207, 4206, 4205, 4204, 4203 }; // Platofrom Locations Medium World
                            foreach (int LocationsX in TeamOneLocation)
                            {
                                if (Terraria.Main.tile[LocationsX, LocationsY + 10].type != Terraria.ID.TileID.Platforms) // Skip Placing If Already Exists
                                {
                                    // Place Tiles Into World
                                    Terraria.Main.tile[LocationsX, LocationsY].type = 19; // 19 Platform ID
                                    Terraria.Main.tile[LocationsX, LocationsY].sTileHeader = PaintType; // Paint Block
                                    Terraria.Main.tile[LocationsX, LocationsY].active(true);
                                }
                                else
                                {
                                    // Change Color of Existing Platform
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].type = 19; // 19 Platform ID
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].sTileHeader = PaintType; // Paint Block
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].active(true);
                                }
                            }

                            // Teleport Player To Respected Platform
                            player.Teleport(GetMedian(TeamOneLocation) * 16, LocationsY * 16 - 48, 1);

                            // Enable Players PvP Status
                            player.SetPvP(true, false);
                            player.SendMessage("[TerraRoyale] Your PvP Was Enabled!", Microsoft.Xna.Framework.Color.Lime);

                            goto NextPlayer;
                        }
                        else if (WorldGen.lastMaxTilesX == 8400) // Large World
                        {
                            // Get Platform Height
                            int LocationsY = 0;
                            for (int Height = 1; Height < 2350 + 1; Height++)
                            {
                                // Starting From Space, Search For Ground
                                if (Terraria.Main.tile[83, Height].active() || Terraria.Main.tile[83, Height].liquid != 0)
                                {
                                    // Add 10 Above Ground And Log Value
                                    LocationsY = Height - 10;
                                    break;
                                }
                            }

                            int[] TeamOneLocation = new int[10] { 5545, 5544, 5543, 5542, 5541, 5540, 5539, 5538, 5537, 5536 }; // Platofrom Locations Large World
                            foreach (int LocationsX in TeamOneLocation)
                            {
                                if (Terraria.Main.tile[LocationsX, LocationsY + 10].type != Terraria.ID.TileID.Platforms) // Skip Placing If Already Exists
                                {
                                    // Place Tiles Into World
                                    Terraria.Main.tile[LocationsX, LocationsY].type = 19; // 19 Platform ID
                                    Terraria.Main.tile[LocationsX, LocationsY].sTileHeader = PaintType; // Paint Block
                                    Terraria.Main.tile[LocationsX, LocationsY].active(true);
                                }
                                else
                                {
                                    // Change Color of Existing Platform
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].type = 19; // 19 Platform ID
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].sTileHeader = PaintType; // Paint Block
                                    Terraria.Main.tile[LocationsX, LocationsY + 10].active(true);
                                }
                            }

                            // Teleport Player To Respected Platform
                            player.Teleport(GetMedian(TeamOneLocation) * 16, LocationsY * 16 - 48, 1);

                            // Enable Players PvP Status
                            player.SetPvP(true, false);
                            player.SendMessage("[TerraRoyale] Your PvP Was Enabled!", Microsoft.Xna.Framework.Color.Lime);

                            goto NextPlayer;
                        }
                    }
                    #endregion

                    // Skip Next Player
                    NextPlayer:;
                }
                catch (Exception)
                {
                }
            }

            // Log Finished Actions
            TShock.Log.ConsoleInfo("[TerraRoyale] Command ran successfully.");
            args.Player.SendErrorMessage("[TerraRoyale] Command ran successfully.");
        }
    }
    
    // End Of Code

}
