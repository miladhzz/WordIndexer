using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace BaseIndexer
{
    [XmlRoot(ElementName = "zamir")]
    public class Zamir
    {
        [XmlAttribute(AttributeName = "index")]
        public int Index { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "marja")]
    public class Marja
    {
        [XmlElement(ElementName = "zamir")]
        public List<Zamir> Zamir { get; set; }
        [XmlAttribute(AttributeName = "index")]
        public int Index { get; set; }
        [XmlAttribute(AttributeName = "content")]
        public string Content { get; set; }
    }

    [XmlRoot(ElementName = "marjas")]
    public class Marjas
    {
        [XmlElement(ElementName = "marja")]
        public List<Marja> Marja { get; set; }
    }

    [XmlRoot(ElementName = "export")]
    public class Export
    {
        [XmlElement(ElementName = "marjas")]
        public Marjas Marjas { get; set; }
    }

}
