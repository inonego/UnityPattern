using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace inonego.Serializable.Xml
{
    [Serializable]
    public class XmlDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    where TKey : IEquatable<TKey>
    where TValue : IKeyable<TKey>
    {

        public XmlDictionary() { }

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty) return;

            reader.MoveToContent();

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    // 현재 요소의 이름을 루트로 사용하여 역직렬화 (태그 이름에 상관없이 TValue 타입으로 파싱 가능)
                    XmlSerializer itemSerializer = new XmlSerializer(typeof(TValue), new XmlRootAttribute(reader.LocalName));
                    TValue item = (TValue)itemSerializer.Deserialize(reader);

                    if (item is IKeyable<TKey> keyableItem && keyableItem.HasKey)
                    {
                        this[keyableItem.Key] = item;
                    }
                }
                else
                {
                    reader.Read();
                }
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (var entry in this)
            {
                XmlSerializer itemSerializer = new XmlSerializer(typeof(TValue));
                itemSerializer.Serialize(writer, entry.Value);
            }
        }
    }
}

