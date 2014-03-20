using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace ResourcesConverter
{
	internal static class Program
	{
		private const string ResourceTemplate = @"<?xml version=""1.0"" encoding=""utf-8""?>
<root>
  <xsd:schema id=""root"" xmlns="""" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:msdata=""urn:schemas-microsoft-com:xml-msdata"">
    <xsd:import namespace=""http://www.w3.org/XML/1998/namespace"" />
    <xsd:element name=""root"" msdata:IsDataSet=""true"">
      <xsd:complexType>
        <xsd:choice maxOccurs=""unbounded"">
          <xsd:element name=""metadata"">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name=""value"" type=""xsd:string"" minOccurs=""0"" />
              </xsd:sequence>
              <xsd:attribute name=""name"" use=""required"" type=""xsd:string"" />
              <xsd:attribute name=""type"" type=""xsd:string"" />
              <xsd:attribute name=""mimetype"" type=""xsd:string"" />
              <xsd:attribute ref=""xml:space"" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name=""assembly"">
            <xsd:complexType>
              <xsd:attribute name=""alias"" type=""xsd:string"" />
              <xsd:attribute name=""name"" type=""xsd:string"" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name=""data"">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name=""value"" type=""xsd:string"" minOccurs=""0"" msdata:Ordinal=""1"" />
                <xsd:element name=""comment"" type=""xsd:string"" minOccurs=""0"" msdata:Ordinal=""2"" />
              </xsd:sequence>
              <xsd:attribute name=""name"" type=""xsd:string"" use=""required"" msdata:Ordinal=""1"" />
              <xsd:attribute name=""type"" type=""xsd:string"" msdata:Ordinal=""3"" />
              <xsd:attribute name=""mimetype"" type=""xsd:string"" msdata:Ordinal=""4"" />
              <xsd:attribute ref=""xml:space"" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name=""resheader"">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name=""value"" type=""xsd:string"" minOccurs=""0"" msdata:Ordinal=""1"" />
              </xsd:sequence>
              <xsd:attribute name=""name"" type=""xsd:string"" use=""required"" />
            </xsd:complexType>
          </xsd:element>
        </xsd:choice>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>
  <resheader name=""resmimetype"">
    <value>text/microsoft-resx</value>
  </resheader>
  <resheader name=""version"">
    <value>2.0</value>
  </resheader>
  <resheader name=""reader"">
    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <resheader name=""writer"">
    <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
</root>";

		public static void Main(string[] args)
		{
			var fileName = args[0];
			var values = Load(fileName);
			var xml = XDocument.Load(new StringReader(ResourceTemplate));
			foreach (var value in values)
			{
				xml.Root.Add(new XElement("data",
					new XAttribute(XNamespace.Xml.GetName("space"), "preserve"),
					new XAttribute("name", value.Key),
					new XElement("value", value.Value),
					new XElement("comment", "")));
			}
			xml.Save(Path.GetFileNameWithoutExtension(fileName) + ".resx");
		}

		private static Dictionary<string, string> Load(string fileName)
		{
			using (StreamReader reader = File.OpenText(fileName))
			{
				return Load(reader);
			}
		}

		private static Dictionary<string, string> Load(Stream stream)
		{
			using (var reader = new StreamReader(stream))
			{
				return Load(reader);
			}
		}

		private static Dictionary<string, string> Load(TextReader reader)
		{
			var strings = new Dictionary<string, string>();
			string str;
			while ((str = reader.ReadLine()) != null)
			{
				if (string.IsNullOrWhiteSpace(str) || str.StartsWith("#"))
					continue;

				var split = str.Split(new[] { '=' }, 2);
				if (split.Length > 1)
				{
					var key = split[0].Trim();
					var message = split[1].Trim().Replace(@"\n", Environment.NewLine);
					while (message.EndsWith("\\"))
					{
						message = message.Remove(message.Length - 1);
						var newLine = reader.ReadLine();
						if (newLine != null)
						{
							message = message + newLine.Trim().Replace(@"\n", Environment.NewLine);
						}
					}
					strings[key] = message;
				}
			}
			return strings;
		}
	}
}
