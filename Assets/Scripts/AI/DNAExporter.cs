using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class DNAExporter
{
    public static void Serialise(this AICarController car)
    {
        AICarInstance carInstance = car.ToInstance();

        Serialise(carInstance);
    }
    
    public static void Serialise(this AICarInstance carInstance)
    {
        List<AICarInstance> carInstances = Deserialise();
        carInstances.Add(carInstance);

        using (StreamWriter writer = File.CreateText(GetPath()))
            writer.Write(JsonHelper.ArrayToJson(carInstances.ToArray(), true));
    }

    public static List<AICarInstance> Deserialise()
    {
        if (!File.Exists(GetPath()))
            File.Create(GetPath());

        AICarInstance[] carInstances;
        using (StreamReader reader = new StreamReader(GetPath()))
            carInstances = (JsonHelper.ArrayFromJson<AICarInstance>(reader.ReadToEnd()));

        return carInstances != null ? carInstances.ToList() : new List<AICarInstance>();
    }

    public static string GetPath() => $"{Application.persistentDataPath}/Agents.json";
}

