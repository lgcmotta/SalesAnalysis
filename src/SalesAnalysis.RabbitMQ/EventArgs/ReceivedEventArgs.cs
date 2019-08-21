namespace SalesAnalysis.RabbitMQ.EventArgs
{
    public class ReceivedEventArgs : System.EventArgs
    {
        public byte[] RecivedObject { get; set; }
    }
}