using System;
using System.IO;
using System.Linq;
using System.Text.Json;
 
 namespace Nuclei.Features;
 
 public class WeatherRandomizerService
 {
     public const string MissionFolderSource = "NuclearOption-Missions";
     public static void RandomizeWeather(string missionName)
     {
         var currentMissionDir = Directory.GetDirectories(MissionFolderSource).First(x => x.Contains(missionName));
         Nuclei.Logger?.LogInfo($"currentMissionDir: {currentMissionDir} ");
         var currentMissionFile = $"{currentMissionDir}/{missionName}.json";
         Nuclei.Logger?.LogInfo($"currentMissionFile: {currentMissionFile} ");
         
         string json = File.ReadAllText(currentMissionFile); 
         var parsedJson = System.Text.Json.Nodes.JsonNode.Parse(json);
        
         var writerOptions = new JsonSerializerOptions()
         {
             WriteIndented = true
         };

         Random rnd = new Random();
         parsedJson["environment"]["timeOfDay"] = rnd.Next(3,13);
         parsedJson["timeFactor"] = 2.0;
         parsedJson["weatherIntensity"] = rnd.NextDouble() * 0.8;
         parsedJson["cloudAltitude"] = 500 + rnd.NextDouble() * 1000;
         parsedJson["cloudAltitude"] = 500 + rnd.NextDouble() * 1000;
         parsedJson["windSpeed"] = rnd.NextDouble() * 4;
         parsedJson["windTurbulence"] = rnd.NextDouble() * 1;
         parsedJson["windHeading"] = rnd.Next(0, 360);
         File.WriteAllText(currentMissionFile, parsedJson.ToJsonString(writerOptions)); 
     }
 }