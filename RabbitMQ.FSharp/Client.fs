namespace RabbitMQ.FSharp

open RabbitMQ.Client
open RabbitMQ.Client.Events
open System.Text

module Client =
    type Queue = { Name: string; Read: unit -> string option; Publish: string -> unit }

    let openConnection address = 
        let factory = new ConnectionFactory(HostName = address)
        factory.CreateConnection()

    // I need to declare the type for connection because F# can't infer types on classes
    let openChannel (connection:IConnection) = connection.CreateModel()

    let declareQueue (channel:IModel) queueName = channel.QueueDeclare( queueName, false, false, false, null )

    let readFromQueue (channel:IModel) queueName =
        declareQueue channel queueName |> ignore

        fun () -> 
            let ea = channel.BasicGet(queueName, true)
            if ea <> null then
                let body = ea.Body
                let message = Encoding.UTF8.GetString(body)
                Some message
            else
                None

    let publishToQueue (channel:IModel) queueName (message:string) =
        declareQueue channel queueName |> ignore
        let body = Encoding.UTF8.GetBytes(message)
        channel.BasicPublish("", queueName, null, body)

    let createQueueFuntions channel =
        (readFromQueue channel, publishToQueue channel)