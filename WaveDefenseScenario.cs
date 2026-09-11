using System;
using System.IO;

namespace WaveDefense
{
    public static class WaveDefenseScenario
    {
        public const string StartingStructure = "Neter/Player/Foot Hold Base";

        public static readonly DefensePosition DefensePosition = new DefensePosition(0, 0, 0);

        public static bool HasStartingStructure(string gameRoot)
        {
            if (String.IsNullOrWhiteSpace(gameRoot))
            {
                return false;
            }

            string blueprintPath = Path.Combine(
                gameRoot,
                "From_The_Depths_Data",
                "StreamingAssets",
                "Blueprints",
                "Neter",
                "Player",
                "Foot Hold Base.blueprint");

            return File.Exists(blueprintPath);
        }
    }

    public struct DefensePosition
    {
        public DefensePosition(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public int X { get; }
        public int Y { get; }
        public int Z { get; }

        public override string ToString()
        {
            return "(" + X + ", " + Y + ", " + Z + ")";
        }
    }
}