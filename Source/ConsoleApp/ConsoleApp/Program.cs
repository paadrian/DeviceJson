using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class Program
{
    const string _appEui = "5071C2FA781D1234";
    const string _lorawan_version = "MAC_V1_0_2";
    const string _lorawan_phy_version = "PHY_V1_0_2_REV_B";
    const string _frequency_plan_id = "EU_863_870_TTN";

    public static void Main()
    {
        var deviceIds = File.ReadAllLines("ids.txt");
        var deviceEui = File.ReadAllLines("deveui.txt");
        var deviceNames = File.ReadAllLines("names.txt");
        var deviceDescription = File.ReadAllLines("location.txt");
        var deviceKeys = File.ReadAllLines("appkeys.txt");

        if (deviceIds is null
            || deviceEui is null
            || deviceNames is null
            || deviceDescription is null
            || deviceKeys is null)
        {
            throw new Exception("One or more input files is empty");
        }

        if (deviceIds.Length != deviceEui.Length 
            || deviceIds.Length != deviceNames.Length
            || deviceIds.Length != deviceDescription.Length
            || deviceIds.Length != deviceKeys.Length)
        {
            throw new Exception("Your input files don't have same count of lines");
        }

        var result = new List<Device>();
        for (var i = 0; i < deviceIds.Length; i++)
        {
            result.Add(new Device
            (
                Ids: new Ids
                (
                    Device_id: deviceIds[i],
                    Dev_eui: deviceEui[i],
                    Join_eui: _appEui
                ),
                Name: deviceNames[i],
                Description: deviceDescription[i],
                Lorawan_version: _lorawan_version,
                Lorawan_phy_version: _lorawan_phy_version,
                Frequency_plan_id: _frequency_plan_id,
                Supports_join: true,
                RootKeys: new RootKeys
                (
                    AppKey: new AppKey(Key: deviceKeys[i]
                )
            )));
        }

        var jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        };
        var jsonResult = JsonConvert.SerializeObject(result, jsonSettings);
        File.WriteAllText("Output.json", jsonResult);
    }
}

[JsonObject]
public record Device
(
    [JsonProperty] Ids Ids,
    [JsonProperty] string Name,
    [JsonProperty] string Description,
    [JsonProperty] string Lorawan_version,
    [JsonProperty] string Lorawan_phy_version,
    [JsonProperty] string Frequency_plan_id,
    [JsonProperty] bool Supports_join,
    [JsonProperty] RootKeys RootKeys
);

[JsonObject]
public record Ids
(
    [JsonProperty] string Device_id,
    [JsonProperty] string Dev_eui,
    [JsonProperty] string Join_eui
);

[JsonObject]
public record RootKeys([JsonProperty] AppKey AppKey);

[JsonObject]
public record AppKey([JsonProperty] string Key);