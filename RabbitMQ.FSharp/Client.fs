namespace RabbitMQ.FSharp

open RabbitMQ.Client
open RabbitMQ.Client.Events
open System.Text

module Client =
    type Queue = { Name: string; Read: unit -> string; Publish: string -> unit }

    let openConnection address = 
        let factory = new ConnectionFactory(HostName = address)
        factory.CreateConnection()

    // I need to declare the type for connection because F# can't infer types on classes
    let private openChannel (connection:IConnection) = connection.CreateModel()

    let readFromQueue (consumer:QueueingBasicConsumer) queueName =
        let ea = consumer.Queue.Dequeue()
        let body = ea.Body
        Encoding.UTF8.GetString(body)

    let publishToQueue (channel:IModel) queueName (message:string) =
        let body = Encoding.UTF8.GetBytes(message)
        channel.BasicPublish("", queueName, null, body)

    let connectToQueue connection name =            // I don't have to declare the type here, because F# can infer the type from my call to openChannel
        use channel = openChannel connection

        fun queueName ->
            channel.QueueDeclare( queueName, false, false, false, null ) |> ignore
            let consumer = new QueueingBasicConsumer(channel) 
            channel.BasicConsume(queueName, true, consumer) |> ignore

            {Name = queueName; 
            Read = (fun () -> readFromQueue consumer queueName); 
            Publish = (publishToQueue channel queueName)}