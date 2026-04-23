using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Mimir.backend.postgres
{
    class GetPostgres
    {
        static public string GetPostgresSettings()
        {

            using (JsonDocument settingDoc = JsonDocument.Parse(File.OpenRead("../../../backend/postgres/settings.json")))
            {
                JsonElement root = settingDoc.RootElement;
                return root.GetProperty("PGSSettings").GetString();
            }

            return null;
        }
    }
}
