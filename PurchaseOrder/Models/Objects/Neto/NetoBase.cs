using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace NetoDeveloper
{
    public class NetoBase
    {
        readonly string Endpoint_URL = "https://quality-deals.neto.com.au/do/WS/NetoAPI";
        readonly string API_KEY = "Tt4pfRj5bQYmNdQdNcBjXTSLCnPUtTOh";
        readonly string API_USERNAME = "tuko";

        public T Request<T>(string action, object data = null)
        {
            string result = "";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Endpoint_URL);
            request.ContentType = "application/json";
            request.Method = "post";
            request.Accept = "application/json";
            request.Headers.Add("NETOAPI_ACTION", action);
            request.Headers.Add("NETOAPI_KEY", API_KEY);
            request.Headers.Add("NETOAPI_USERNAME", API_USERNAME);
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;
            request.ServicePoint.ConnectionLimit = 1;

            HttpWebResponse httpResponse;

            try
            {
                if (data != null)
                {
                    var jsonSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                    string postData = JsonConvert.SerializeObject(data, jsonSetting);
                    using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(postData);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }

                httpResponse = (HttpWebResponse)request.GetResponse();
                var status = httpResponse.StatusCode;
                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return JsonConvert.DeserializeObject<T>(result, new JsonSerializerSettings { ContractResolver = new CustomDateContractResolver() });
        }
    }

    class CustomDateContractResolver : DefaultContractResolver
    {
        protected override JsonContract CreateContract(Type objectType)
        {
            JsonContract contract = base.CreateContract(objectType);
            if (objectType == typeof(DateTime))
            {
                contract.Converter = new ZerosIsoDateTimeConverter("yyyy-MM-dd HH:mm:ss", "0000-00-00 00:00:00");
            }
            else if (objectType == typeof(bool))
            {
                contract.Converter = new BooleanJsonConverter();
            }
            return contract;
        }
    }
    public class BooleanJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            switch (reader.Value.ToString().ToLower().Trim())
            {
                case "true":
                case "yes":
                case "y":
                case "1":
                    return true;
                case "false":
                case "no":
                case "n":
                case "0":
                    return false;
            }

            // If we reach here, we're pretty much going to throw an error so let's let Json.NET throw it's pretty-fied error message.
            return new JsonSerializer().Deserialize(reader, objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }
    }

    /// <summary>
    /// Custom IsoDateTimeConverter for DateTime strings with zeros.
    /// 
    /// Usage Sample
    ///  [JsonConverter(typeof(ZerosIsoDateTimeConverter), "yyyy-MM-dd hh:mm:ss", "0000-00-00 00:00:00")]
    ///  public DateTime Zerodate { get; set; }

    /// </summary>
    public class ZerosIsoDateTimeConverter : IsoDateTimeConverter
    {
        /// <summary>
        /// The string representing a datetime value with zeros. E.g. "0000-00-00 00:00:00"
        /// </summary>
        private readonly string _zeroDateString;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZerosIsoDateTimeConverter"/> class.
        /// </summary>
        /// <param name="dateTimeFormat">The date time format.</param>
        /// <param name="zeroDateString">The zero date string. 
        /// Please be aware that this string should match the date time format.</param>
        public ZerosIsoDateTimeConverter(string dateTimeFormat, string zeroDateString)
        {
            DateTimeFormat = dateTimeFormat;
            _zeroDateString = zeroDateString;
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// If a DateTime value is DateTime.MinValue than the zeroDateString will be set as output value.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime && (DateTime)value == DateTime.MinValue)
            {
                value = _zeroDateString;
                serializer.Serialize(writer, value);
            }
            else
            {
                base.WriteJson(writer, value, serializer);
            }
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// If  an input value is same a zeroDateString than DateTime.MinValue will be set as return value
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The object value.
        /// </returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            try
            {
                serializer.DateFormatString = DateTimeFormat;
                return reader.Value.ToString() == _zeroDateString
                    ? DateTime.MinValue
                    : base.ReadJson(reader, objectType, existingValue, serializer);
            }
            catch (Exception e)
            {
                var test = reader.Value.ToString();
                return DateTime.MinValue;
            }
        }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum YesNo { False, True }
}