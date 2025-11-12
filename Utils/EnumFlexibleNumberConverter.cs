using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Veterinaria.Web.Utils
{
    // Lee string ("VacunacionAnual") o número (1) y escribe siempre número.
    public class EnumFlexibleNumberConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
    {
        public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var s = reader.GetString();
                if (Enum.TryParse<TEnum>(s, true, out var byName)) return byName;
                if (int.TryParse(s, out var iFromStr)) return (TEnum)Enum.ToObject(typeof(TEnum), iFromStr);
                throw new JsonException($"Valor '{s}' inválido para {typeof(TEnum).Name}");
            }
            if (reader.TokenType == JsonTokenType.Number)
            {
                var i = reader.GetInt32();
                return (TEnum)Enum.ToObject(typeof(TEnum), i);
            }
            throw new JsonException($"Se esperaba string o number para {typeof(TEnum).Name}");
        }

        public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(Convert.ToInt32(value));
        }
    }
}
