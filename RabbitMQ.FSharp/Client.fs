namespace RabbitMQ.FSharp

open RabbitMQ.Client
open RabbitMQ.Client.Events
open System.Text

module Client =
    type Queue = { Name: string; Read: unit -> string; Publish: string -> unit }

    let readFromQueue (consumer:QueueingBasicConsumer) queueName =
        let ea = consumer.Queue.Dequeue()
        let body = ea.Body
        Encoding.UTF8.GetString(body)

    let publishToQueue (channel:IModel) queueName (message:string) =
        let body = Encoding.UTF8.GetBytes(message)
        channel.BasicPublish("", queueName, null, body)

    let connectToRabbitMq address =
        let factory = new ConnectionFactory(HostName = "localhost")
        use connection = factory.CreateConnection()
        use channel = connection.CreateModel()

        fun queueName ->
            channel.QueueDeclare( queueName, false, false, false, null ) |> ignore
            let consumer = new QueueingBasicConsumer(channel) 
            channel.BasicConsume(queueName, true, consumer) |> ignore

            {Name = queueName; 
            Read = (fun () -> readFromQueue consumer queueName); 
            Publish = (publishToQueue channel queueName)}