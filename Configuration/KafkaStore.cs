using System.Collections.Concurrent;

public static class KafkaStore
    {
        private static ConcurrentDictionary<string, Type> kafkaHandlerStore = new();
 
        public static bool TryAdd(string topicName, Type type)
        {
            return kafkaHandlerStore.TryAdd(topicName, type);
        }
 
        public static bool TryGet(string topicName, out Type type)
        {
            return kafkaHandlerStore.TryGetValue(topicName, out type!);
        }
    }