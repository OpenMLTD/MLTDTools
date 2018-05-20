using System;
using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;

namespace OpenMLTD.ThankYou {
    internal static class Lang {

        static Lang() {
            _languages = new Dictionary<string, Dictionary<string, string>> {
                ["zh-CN"] = new Dictionary<string, string> {
                    ["ip_not_found+tpl"] = "未找到 IP: {0}",
                    ["dns_started+tpl"] = "位于 {0} 上的 DNS 服务器已启动，按 Ctrl+C 或回车停止。",
                    ["processing_host_names+tpl"] = "将解析符合下列正则表达式的主机名称到本机 ({0})：" + Environment.NewLine + "{1}",
                    ["upstream_dns_address+tpl"] = "上游 DNS 地址: {0}",
                    ["dns_stopped"] = "DNS 服务器已停止。",
                    ["redirecting+tpl"] = "正在重定向 {0}"
                },
                ["en-US"] = new Dictionary<string, string> {
                    ["ip_not_found+tpl"] = "IP is not found: {0}",
                    ["dns_started+tpl"] = "DNS server started on {0}, press Ctrl+C or Enter to stop.",
                    ["processing_host_names+tpl"] = "Parsing hosts that match following regular expressions to localhost ({0})：" + Environment.NewLine + "{1}",
                    ["upstream_dns_address+tpl"] = "Upstream DNS address: {0}",
                    ["dns_stopped"] = "DNS server stopped.",
                    ["redirecting+tpl"] = "Redirecting {0}"
                }
            };
        }

        [NotNull]
        public static string Get([NotNull] string key) {
            var locale = GetLocaleName();
            if (!_languages.ContainsKey(locale)) {
                locale = "en-US";
            }
            _languages[locale].TryGetValue(key, out var str);
            if (str == null) {
                str = string.Empty;
            }
            return str;
        }

        private static string GetLocaleName() {
            if (_localeName != null) {
                return _localeName;
            }
            var culture = CultureInfo.CurrentUICulture;
            _localeName = culture.Name;
            return _localeName;
        }

        private static string _localeName;

        private static readonly Dictionary<string, Dictionary<string, string>> _languages;

    }
}