namespace Common.Events;
    public  class EventModel
    {
        public DateTime TimeStamp { get; set; }
        public  Guid AggregateIdentifier { get; set; }
        public string AggregateType { get; set; }
        public string EventType { get; set; }
        public DomainEventBase EventBaseData { get; set; }
    }
 