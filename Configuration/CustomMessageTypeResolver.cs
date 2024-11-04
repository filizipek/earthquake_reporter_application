using EarthquakeReporter.Models;
using KafkaFlow;
using KafkaFlow.Middlewares.Serializer.Resolvers;

public class CustomMessageTypeResolver : IMessageTypeResolver
{
     /*public Type OnConsume(IMessageContext context)
     {
         throw new NotImplementedException();
     }*/
 
     public ValueTask<Type> OnConsumeAsync(IMessageContext context)
     {
         
         return new ValueTask<Type>(typeof(EarthquakeEvent));
     }
 
     public void OnProduce(IMessageContext context)
     {
         throw new NotImplementedException();
     }
 
     public ValueTask OnProduceAsync(IMessageContext context)
     {
         throw new NotImplementedException();
     }
}