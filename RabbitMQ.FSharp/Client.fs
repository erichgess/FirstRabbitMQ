﻿namespace RabbitMQ.FSharp

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
        fun () -> 
            let ea = channel.BasicGet(queueName, true)
            if ea <> null then
                let body = ea.Body
                let message = Encoding.UTF8.GetString(body)
                Some message
            else
                None

    let publishToQueue (channel:IModel) queueName (message:string) =
        let body = Encoding.UTF8.GetBytes(message)
        channel.BasicPublish("", queueName, null, body)
        
    let createQueueReader channel queue = 
        declareQueue channel queue |> ignore
        readFromQueue channel queue

    let createQueueWriter channel queue =
        declareQueue channel queue |> ignore
        publishToQueue channel queue

    let createQueueConsumer channel queueName =
        let consumer = new QueueingBasicConsumer(channel) 
        channel.BasicConsume(queueName, true, consumer) |> ignore

        fun () ->
            let ea = consumer.Queue.Dequeue()
            let body = ea.Body
            Encoding.UTF8.GetString(body)
