using System.Xml.Linq;

namespace Sidebar.Infrastructure.Helpers
{
    class WebXmlHelper
    {
        // 从 XML URL 获取 version 信息
        public static async Task<string?> GetVersionFromXml(string url)
        {
            try
            {
                // 使用 HttpClient 获取 XML 内容
                using (HttpClient client = new HttpClient())
                {
                    var byteArray = System.Text.Encoding.ASCII.GetBytes("admin:admin");
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue(
                            "Basic",
                            Convert.ToBase64String(byteArray)
                        );

                    string xmlContent = await client.GetStringAsync(url);

                    // 解析 XML 内容
                    XDocument xmlDoc = XDocument.Parse(xmlContent);
                    if (xmlDoc == null) return null;

                    // 假设 XML 中的 version 位于 <version> 标签中
                    XElement? versionElement = xmlDoc.Descendants("version").FirstOrDefault();

                    if (versionElement != null)
                    {
                        return versionElement.Value;
                    }
                    else
                    {
                        Console.WriteLine("Version element not found in the XML.");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching or parsing XML: {ex.Message}");
                return null;
            }
        }
    }
}
