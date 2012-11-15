using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using ProblemLib.API;

namespace ProblemLib.Utilities
{
    public static class NetworkUtilities
    {
        /// <summary>
        /// Sends an HTTP request to remote server and reads response as string.
        /// </summary>
        /// <param name="url">Url of the request</param>
        /// <returns></returns>
        public static String Request(Uri url)
        {
            String result;

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new ProblemLib.ErrorHandling.ProblemLibException(
                        ErrorCodes.OsrmConnectionFailed,
                        new Exception(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription)));

                StreamReader reader = new StreamReader(response.GetResponseStream());
                result = reader.ReadToEnd();
            }

            return result;
        }
        /// <summary>
        /// Sends an HTTP request to remote server and deserializes response as an object.
        /// </summary>
        /// <param name="url">Url of the request</param>
        /// <returns></returns>
        public static T JsonRequest<T>(Uri url) where T : class
        {
            T result;

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new ProblemLib.ErrorHandling.ProblemLibException(
                        ErrorCodes.NetworkError,
                        new Exception(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription)));

                var serializer = new DataContractJsonSerializer(typeof(T));
                result = serializer.ReadObject(response.GetResponseStream()) as T;
            }

            if (result == null)
                throw new ProblemLib.ErrorHandling.ProblemLibException(
                    ErrorCodes.NetworkError,
                    new Exception("Failed to deserialize OSRM response data!"));

            return result;
        }

    }
}
