using System.Runtime.Serialization;
namespace Demo.Model
{
    [DataContract]
    public class Config
    {
        [DataMember]
        public Position transaction { get; set; }
        [DataMember]
        public Rect transactionRect { get; set; }
        [DataMember]
        public Position search { get; set; }
        [DataMember]
        public Position inputText { get; set; }
        [DataMember]
        public Position searchBtn { get; set; }
        [DataMember]
        public Position goodsBtn { get; set; }
        [DataMember]
        public Position FirstGoods { get; set; }
        [DataMember]
        public Rect goodsrect { get; set; }
        [DataMember]
        public Position BuyBtn { get; set; }
        [DataMember]
        public string goodsName { get; set; }
        [DataMember]
        public Position offset { get; set; }
        [DataMember]
        public Position price { get; set; }
        [DataMember]
        public Rect priceRect { get; set; }
    }
    [DataContract]
    public class Position
    {
        [DataMember]
        public string X { get; set; }
        [DataMember]
        public string Y { get; set; }
    }
}
