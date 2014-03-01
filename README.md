FirstRabbitMQ
=============

A simple demonstration of RabbitMQ.


This consists of two projects:

1.  Sender - which creates a queue named "hello" and then publishes the message "Hello, World" onto this queue every two seconds.
1.  Receiver - which creates a consumer on the "hello" queue.  This consumer is wrapped into a sequence expression, which allows the queue to be treated as an enumerable (so things like map, iter, and zip can be applied to the queue)

