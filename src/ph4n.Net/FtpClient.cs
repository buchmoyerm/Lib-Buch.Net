using System.Collections.Generic;
using System.IO;
using System.Net;

namespace ph4n.Net
{
    public class FTPClient
    {
        // The hostname or IP address of the FTP server
        private string _remoteHost;

        // The remote username
        private string _remoteUser;

        // Password for the remote user
        private string _remotePass;

        public FTPClient(string remoteHost, string remoteUser, string remotePassword)
        {
            _remoteHost = remoteHost;
            _remoteUser = remoteUser;
            _remotePass = remotePassword;
        }

        public List<string> DirectoryListing()
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_remoteHost);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = new NetworkCredential(_remoteUser, _remotePass);
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);

            List<string> result = new List<string>();

            while (!reader.EndOfStream)
            {
                result.Add(reader.ReadLine());
            }

            reader.Close();
            response.Close();
            return result;
        }

        public byte[] GetFile(string file)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_remoteHost + file);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(_remoteUser, _remotePass);
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            MemoryStream memStream = new MemoryStream();
            responseStream.CopyTo(memStream);

            var bytes = memStream.ToArray();

            memStream.Close();
            responseStream.Close();
            response.Close();

            return bytes;
        }

        public List<string> GetTextFile(string file)
        {
            var lines = new List<string>();
            using (var stream = new MemoryStream(GetFile(file)))
            using (var reader = new StreamReader(stream))
            {
                while (reader.Peek() >= 0)
                {
                    lines.Add(reader.ReadLine());
                }
            }
            return lines;
        }

        public void Download(string file, string destination)
        {
            using (var stream = new MemoryStream(GetFile(file)))
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(destination))
            {
                writer.Write(reader.ReadToEnd());
            }
        }
    }

}
