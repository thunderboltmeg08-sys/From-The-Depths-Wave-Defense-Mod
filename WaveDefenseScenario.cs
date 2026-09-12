using System;
using System.IO;
using UnityEngine.SceneManagement;

namespace WaveDefense
{
    public static class WaveDefenseScenario
    {
        public const string StartingStructure = "Neter/Player/Foot Hold Base";
        public const string StartingStructureName = "Foot Hold Base";

        public static bool IsGameplayScene
        {
            get { return SceneManager.GetActiveScene().buildIndex > 0; }
        }

        public static readonly DefensePosition DefensePosition = new DefensePosition(68.73599, 0, 193.3501);

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
        public DefensePosition(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public override string ToString()
        {
            return "(" + X + ", " + Y + ", " + Z + ")";
        }
    }
}