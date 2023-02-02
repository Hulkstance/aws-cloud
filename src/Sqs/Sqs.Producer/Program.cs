using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Sqs.Contracts;

var sqsClient = new AmazonSQSClient();

var customer = new CustomerCreated
{
    Id = Guid.NewGuid(),
    Email = "johnsmith@gmail.com",
    FullName = "John Smith",
    DateOfBirth = new DateTime(1990, 12, 12),
    GitHubUsername = "johnsmith"
};

var queueUrlResponse = await sqsClient.GetQueueUrlAsync("customers");

var sendMessageRequest = new SendMessageRequest
{
    QueueUrl = queueUrlResponse.QueueUrl,
    MessageBody = JsonSerializer.Serialize(customer),
    MessageAttributes = new Dictionary<string, MessageAttributeValue>
    {
        { "MessageType", new MessageAttributeValue { DataType = "String", StringValue = nameof(CustomerCreated) } }
    }
};

var response = await sqsClient.SendMessageAsync(sendMessageRequest);

Console.WriteLine($"Status Code = {response.HttpStatusCode}");
