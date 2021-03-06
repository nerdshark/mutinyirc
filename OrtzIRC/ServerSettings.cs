﻿namespace OrtzIRC
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using System.Xml.Schema;
    using System.Xml;
    using OrtzIRC.Common;

    public class ServerSettings : IXmlSerializable, IEquatable<ServerSettings>
    {
        public ServerSettings(string url, string description, string ports, bool ssl)
        {
            Url = url;
            Description = description;
            Ports = ports;
            Ssl = ssl;
            AutoConnect = true;
        }

        public ServerSettings() { }

        public string Url { get; set; }
        public string Description { get; set; }
        public string Ports { get; set; }
        public bool Ssl { get; set; }
        public bool AutoConnect { get; set; }
        public string Nick { get; set; }
        public NetworkSettings Network { get; set; }

        public int RandomPort
        {
            get
            {
                Random r = new Random();
                return PortList[r.Next(0, PortList.Length - 1)];
            }
        }

        private int[] PortList
        {
            get
            {
                return PortsStringToArray(Ports);
            }
        }

        private static int[] PortsStringToArray(string ports)
        {
            if (ports == string.Empty)
                return null;

            string[] portListChunk = ports.Split(',');

            var portList = new List<int>(portListChunk.Length); // Maximum performance if there are no ranges

            foreach (string chunk in portListChunk)
            {
                if (chunk.Contains("-"))
                {
                    // Chunk is a range
                    string[] rangeParts = chunk.Split('-');

                    // User entered something stupid like "30-;"
                    if (rangeParts.Length != 2)
                        throw new FormatException();

                    int beginRange;
                    int endRange;

                    try
                    {
                        beginRange = Int32.Parse(rangeParts[0]);
                        endRange = Int32.Parse(rangeParts[1]);
                    }
                    catch (Exception)
                    {
                        throw new FormatException();
                    }

                    // User entered something dumb like "30-20" or "20-20"
                    if (endRange <= beginRange)
                        throw new FormatException();

                    // Add entire range to the list
                    while (beginRange <= endRange && !portList.Contains(beginRange))
                        portList.Add(beginRange++);
                }
                else // Chunk is a single port number
                {
                    int tempNum;

                    try
                    {
                        tempNum = Int32.Parse(chunk);
                    }
                    catch (Exception)
                    {
                        throw new FormatException();
                    }

                    if (!portList.Contains(tempNum))
                        portList.Add(tempNum);
                }
            }

            return portList.ToArray();
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Description = reader.GetAttribute("Description") ?? String.Empty;
            Url = reader.GetAttribute("Url");
            Ports = reader.GetAttribute("Ports");
            AutoConnect = reader.GetAttribute("AutoConnect") == "True";
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Description", Description);
            writer.WriteAttributeString("Url", Url);
            writer.WriteAttributeString("Ports", Ports);
            writer.WriteAttributeString("AutoConnect", AutoConnect.ToString());
        }

        public bool Equals(ServerSettings server)
        {
            return server != null && server.Url == Url;
        }
    }
}
