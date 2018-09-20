using System.Collections.Specialized;

namespace DBLayer.Core
{
    public class ConnectionString
    {
        public NameValueCollection Properties { get; set; }
        public string ConnectionToken { get; set; }

        private string connectionString { get; set; }

        /// <summary>
        /// Replace properties by their values in the given string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        private string ParsePropertyTokens(string str, NameValueCollection properties)
        {
            string OPEN = "${";
            string CLOSE = "}";

            string newString = str;
            if (newString != null && properties != null)
            {
                int start = newString.IndexOf(OPEN);
                int end = newString.IndexOf(CLOSE);

                while (start > -1 && end > start)
                {
                    string prepend = newString.Substring(0, start);
                    string append = newString.Substring(end + CLOSE.Length);

                    int index = start + OPEN.Length;
                    string propName = newString.Substring(index, end - index);
                    string propValue = properties.Get(propName);
                    if (propValue == null)
                    {
                        newString = prepend + propName + append;
                    }
                    else
                    {
                        newString = prepend + propValue + append;
                    }
                    start = newString.IndexOf(OPEN);
                    end = newString.IndexOf(CLOSE);
                }
            }
            return newString;
        }

        public override string ToString()
        {
            if(string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = ParsePropertyTokens(ConnectionToken, Properties);
            }
            return connectionString;
        }
    }
}
