using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class DNAExporter
{
    public static void Export(this AICarController car)
    {
        string json = car.DNAToJson();

        using (StreamWriter writer = File.CreateText($"{Application.persistentDataPath}/Agents.json"))
            writer.Write(json);
    } 
}

