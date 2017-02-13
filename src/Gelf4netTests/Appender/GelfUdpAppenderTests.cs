﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Gelf4net.Appender;
using log4net.Core;
using System.Security.Cryptography;
using System.IO;
using NUnit.Framework;
using log4net;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Threading;
using System.IO.Compression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using log4net.Appender;
using log4net.Repository;
using log4net.Util;
using System.Globalization;
using Gelf4net;

namespace Gelf4netTest
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestFixture]
    public class GelfUdpAppenderTest
    {
        [Test]
        public void GenerateMessageId_TestLength()
        {
            const int expectedLength = 8;
            var actual = GelfUdpAppender.GenerateMessageId();
            Assert.AreEqual(actual.Length, expectedLength);
        }

        [Test]
        public void GenerateMessageId_NoCollision()
        {
            // Arrange
            int maxIterations = 10000;
            var generatedIds = new Dictionary<long, long>();

            // Act
            for (var i = 0; i < maxIterations; i++)
            {
                var id = BitConverter.ToInt64(GelfUdpAppender.GenerateMessageId(), 0);
                generatedIds.Add(id, id);
            }

            // Assert
            Assert.That(generatedIds.Count, Is.EqualTo(maxIterations));
        }

        [Test]
        public void CreateChunkedMessagePart_StartsWithCorrectHeader()
        {
            // Arrange
            byte[] messageId = Encoding.UTF8.GetBytes("A1B2C3D4");
            int index = 1;
            int chunkCount = 1;

            // Act
            byte[] result = GelfUdpAppender.CreateChunkedMessagePart(messageId, index, chunkCount);

            // Assert
            Assert.That(result[0], Is.EqualTo(30));
            Assert.That(result[1], Is.EqualTo(15));
        }

        [Test]
        public void CreateChunkedMessagePart_ContainsMessageId()
        {
            // Arrange
            byte[] messageId = Encoding.UTF8.GetBytes("A1B2C3D4");
            int index = 1;
            int chunkCount = 1;

            // Act
            byte[] result = GelfUdpAppender.CreateChunkedMessagePart(messageId, index, chunkCount);

            // Assert
            Assert.That(result[2], Is.EqualTo((int)'A'));
            Assert.That(result[3], Is.EqualTo((int)'1'));
            Assert.That(result[4], Is.EqualTo((int)'B'));
            Assert.That(result[5], Is.EqualTo((int)'2'));
            Assert.That(result[6], Is.EqualTo((int)'C'));
            Assert.That(result[7], Is.EqualTo((int)'3'));
            Assert.That(result[8], Is.EqualTo((int)'D'));
            Assert.That(result[9], Is.EqualTo((int)'4'));
        }

        [Test]
        public void CreateChunkedMessagePart_EndsWithIndexAndCount()
        {
            // Arrange
            byte[] messageId = Encoding.UTF8.GetBytes("A1B2C3D4");
            int index = 1;
            int chunkCount = 2;

            // Act
            byte[] result = GelfUdpAppender.CreateChunkedMessagePart(messageId, index, chunkCount);

            // Assert
            Assert.That(result[10], Is.EqualTo(index));
            Assert.That(result[11], Is.EqualTo(chunkCount));
        }
    }
}