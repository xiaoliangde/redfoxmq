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
using System;
using System.Collections.Concurrent;

// ReSharper disable once CheckNamespace
namespace RedFoxMQ.Tests
{
    class TestSubscriber : Subscriber
    {
        private readonly BlockingCollection<IMessage> _receivedMessages = new BlockingCollection<IMessage>();

        public TestSubscriber()
        {
            MessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(IMessage message)
        {
            _receivedMessages.Add(message);
        }

        public IMessage TestMustReceiveMessageWithin(int timeoutMillis)
        {
            IMessage receivedMessage;
            if (!_receivedMessages.TryTake(out receivedMessage, timeoutMillis))
                throw new Exception("Subscriber didn't receive message in time");

            return receivedMessage;
        }
    }
}
