﻿// ReSharper disable InconsistentNaming

using System;
using NUnit.Framework;

namespace EasyNetQ.Hosepipe.Tests
{
    [TestFixture]
    public class MessageReaderTests
    {
        private IMessageReader messageReader;

        [SetUp]
        public void SetUp()
        {
            messageReader = new MessageReader();
        }

        /// <summary>
        /// 1. Put some messages in C:\temp\MessageOutput
        /// 2. Run this test
        /// 3. Check the output, you should see your messages.
        /// </summary>
        [Test, Explicit(@"Needs message files in 'C:\temp\MessageOutput'")]
        public void Should_be_able_to_read_messages_from_disk()
        {
            var parameters = new QueueParameters
            {
                MessageFilePath = @"C:\temp\MessageOutput"
            };

            var messages = messageReader.ReadMessages(parameters);
            foreach (var message in messages)
            {
                Console.WriteLine(message);
            }
        } 
    }
}

// ReSharper restore InconsistentNaming