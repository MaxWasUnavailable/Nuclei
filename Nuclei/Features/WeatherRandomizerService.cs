using System;
using System.IO;
using System.Linq;
using System.Text.Json;
 
 namespace Nuclei.Features;
 
 public class WeatherRandomizerService
 {
     internal static string MissionDir = null!;
     public static void RandomizeWeather(string missionName)
     {
         var currentMissionDir = Directory.GetDirectories(MissionDir).First(x => x.Contains(missionName));
         Nuclei.Logger?.LogInfo($"currentMissionDir: {currentMissionDir} ");
         var currentMissionFile = $"{currentMissionDir}/{missionName}.json";
         Nuclei.Logger?.LogInfo($"currentMissionFile: {currentMissionFile} ");
         
         string json = File.ReadAllText(currentMissionFile); 
         var parsedJson = System.Text.Json.Nodes.JsonNode.Parse(json)!;
        
         var writerOptions = new JsonSerializerOptions()
         {
             WriteIndented = true
         };

         Random rnd = new Random();
         parsedJson["environment"]!["timeOfDay"] = rnd.Next(3,13);
         parsedJson["environment"]!["timeFactor"] = 2.0;
         parsedJson["environment"]!["weatherIntensity"] = rnd.NextDouble() * 0.9;
         parsedJson["environment"]!["cloudAltitude"] = 500 + rnd.NextDouble() * 1000;
         parsedJson["environment"]!["cloudAltitude"] = 500 + rnd.NextDouble() * 1000;
         parsedJson["environment"]!["windSpeed"] = rnd.NextDouble() * 4;
         parsedJson["environment"]!["windTurbulence"] = rnd.NextDouble() * 1;
         parsedJson["environment"]!["windHeading"] = rnd.Next(0, 360);
         parsedJson["environment"]!["windRandomHeading"] = rnd.Next(0, 91);
         File.WriteAllText(currentMissionFile, parsedJson.ToJsonString(writerOptions)); 
     }
 }