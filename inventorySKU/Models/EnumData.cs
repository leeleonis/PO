using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace inventorySKU
{
    public static class EnumData
    {
        public enum TimeZone { UTC, EST, TST, PST, GMT, AEST, JST };
        public static Dictionary<TimeZone, string> TimeZoneList()
        {
            return new Dictionary<TimeZone, string>() { { TimeZone.UTC, "UTC" },
                { TimeZone.EST, "Eastern Standard Time" }, { TimeZone.TST, "Taipei Standard Time" }, { TimeZone.PST, "Pacific Standard Time" },
                { TimeZone.GMT, "Greenwich Mean Time" }, { TimeZone.AEST, "AUS Eastern Standard Time" }, { TimeZone.JST, "Tokyo Standard Time" }
            };
        }

        public static Dictionary<string, string> SystemLangList()
        {
            return new Dictionary<string, string>()
            {
                { "zh-TW", "繁體中文" },
                { "en-US", "English" }
            };
        }

        public static Dictionary<string, string> DataLangList()
        {
            return new Dictionary<string, string>()
            {
                { "en-US", "English" },
                { "ja", "日本語" }
            };
        }

        public enum YesNo { No, Yes }

        public enum AttributeProperty { Normal, YesNo, Dimension, Resolution}
        public enum SkuType { Single, Variation, Kit }
        public enum SkuStatus { Inactive, Active}
    }
}