using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Service.Infrastructure.Utils
{
    /// <summary>
    /// Timespan is not serializable with System.Text.Json for now. Here is the custom json convertor as workaround
    /// More details: https://github.com/dotnet/corefx/issues/38641
    /// </summary>
    public class TimeSpanJsonConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return TimeSpan.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
