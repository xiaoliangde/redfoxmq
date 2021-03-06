﻿// 
// Copyright 2013 Hans Wolff
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
using NUnit.Framework;
using RedFoxMQ.Transports;
using System.Threading;

namespace RedFoxMQ.Tests
{
    [TestFixture]
    public class PublisherSubscriberTests
    {
        [TestCase(RedFoxTransport.Inproc)]
        [TestCase(RedFoxTransport.Tcp)]
        public void Subscribe_to_publisher_receive_single_broadcasted_message(RedFoxTransport transport)
        {
            using (var publisher = new Publisher())
            using (var subscriber = new TestSubscriber())
            {
                publisher.Bind(TestHelpers.TcpTestEndpoint);
                subscriber.ConnectAsync(TestHelpers.TcpTestEndpoint).Wait();

                Thread.Sleep(30);

                var broadcastedMessage = new TestMessage { Text = "Hello" };

                publisher.Broadcast(broadcastedMessage);

                Assert.AreEqual(broadcastedMessage, subscriber.TestMustReceiveMessageWithin(10000));
            }
        }

        [TestCase(RedFoxTransport.Inproc)]
        [TestCase(RedFoxTransport.Tcp)]
        public void Subscribe_to_publisher_receive_two_single_broadcasted_messages(RedFoxTransport transport)
        {
            using (var publisher = new Publisher())
            using (var subscriber = new TestSubscriber())
            {
                publisher.Bind(TestHelpers.TcpTestEndpoint);
                subscriber.ConnectAsync(TestHelpers.TcpTestEndpoint).Wait();

                Thread.Sleep(30);

                var broadcastedMessage = new TestMessage { Text = "Hello" };

                publisher.Broadcast(broadcastedMessage);
                publisher.Broadcast(broadcastedMessage);

                Assert.AreEqual(broadcastedMessage, subscriber.TestMustReceiveMessageWithin(10000));
                Assert.AreEqual(broadcastedMessage, subscriber.TestMustReceiveMessageWithin(10000));
            }
        }

        [TestCase(RedFoxTransport.Inproc)]
        [TestCase(RedFoxTransport.Tcp)]
        public void Subscribe_to_publisher_receive_two_broadcasted_messages_from_batch(RedFoxTransport transport)
        {
            using (var publisher = new Publisher())
            using (var subscriber = new TestSubscriber())
            {
                publisher.Bind(TestHelpers.TcpTestEndpoint);
                subscriber.ConnectAsync(TestHelpers.TcpTestEndpoint).Wait();

                Thread.Sleep(30);

                var broadcastedMessage = new TestMessage { Text = "Hello" };

                var batch = new[] {broadcastedMessage, broadcastedMessage};
                publisher.Broadcast(batch);

                Assert.AreEqual(broadcastedMessage, subscriber.TestMustReceiveMessageWithin(10000));
                Assert.AreEqual(broadcastedMessage, subscriber.TestMustReceiveMessageWithin(10000));
            }
        }

        [TestCase(RedFoxTransport.Inproc)]
        [TestCase(RedFoxTransport.Tcp)]
        public void one_subscriber_connects_to_one_publisher_receives_message_then_second_subscriber_connects_both_receive_message(RedFoxTransport transport)
        {
            using (var publisher = new Publisher())
            using (var subscriber1 = new TestSubscriber())
            using (var subscriber2 = new TestSubscriber())
            {
                publisher.Bind(TestHelpers.TcpTestEndpoint);
                subscriber1.ConnectAsync(TestHelpers.TcpTestEndpoint).Wait();

                Thread.Sleep(30);

                var broadcastMessage = new TestMessage { Text = "Hello" };
                publisher.Broadcast(broadcastMessage);

                Assert.AreEqual(broadcastMessage, subscriber1.TestMustReceiveMessageWithin(10000));

                subscriber2.ConnectAsync(TestHelpers.TcpTestEndpoint).Wait();

                Thread.Sleep(30);

                publisher.Broadcast(broadcastMessage);

                Assert.AreEqual(broadcastMessage, subscriber1.TestMustReceiveMessageWithin(10000));
                Assert.AreEqual(broadcastMessage, subscriber2.TestMustReceiveMessageWithin(10000));
            }
        }

        [SetUp]
        public void Setup()
        {
            TestHelpers.InitializeMessageSerialization();
        }
    }
}
