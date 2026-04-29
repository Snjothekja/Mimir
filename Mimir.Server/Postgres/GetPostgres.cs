using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace Mimir.backend.postgres
{
    class GetPostgres
    {
        static public string GetPostgresSettings()
        {

            using (JsonDocument settingDoc = JsonDocument.Parse(File.OpenRead("C:/Users/Lykaios/Desktop/Coding_Stuff/Mimir/Mimir.Server/Postgres/settings.json")))
            {
                JsonElement root = settingDoc.RootElement;
                return root.GetProperty("PGSSettings").GetString();
            }

            return null;
        }
    }
}
